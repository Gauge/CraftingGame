using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameServer;
using GameServer.Networking;
using GameFrontEndDebugger.Controls;
using Message = GameServer.Networking.Message;
using Logger = GameFrontEndDebugger.Controls.Logger;
using System.Runtime.InteropServices;

namespace GameFrontEndDebugger {
	public partial class GameForm : Form {
		public const int UNIT = 15;
		private enum State {
			Connected, Disconnected, Dead
		};

		// connection data
		private int _id;
		private string _address;
		private int _port;
		private string _username;
		private string _password;
		private bool _attemptConnection;
		private Client _client;
		private State state;

		// game data
		private Game _game;

		// display data
		private bool _isInventoryOpen = false;
		private long _lastTime;
		private long _ping;
		private int _fps;

		private ChatDisplay chatDisplay;
		private ChatInput chatInput;
		private LoginForm loginForm;
		private Logger logger;
		public Bitmap display { get; set; }

		public GameForm() {
			DoubleBuffered = true;

			state = State.Disconnected;
			_game = new Game();
			_client = new Client();
			display = new Bitmap(Width, Height);
			loginForm = new LoginForm();
			logger = new Logger();
			chatInput = new ChatInput();
			chatDisplay = new ChatDisplay();

			chatInput.KeyDown += chatInput_SubmitChat;

			Controls.Add(chatInput);
			Controls.Add(chatDisplay);
			Controls.Add(logger);

			InitializeComponent();
			setControlSize();
			connectToServer();

			Application.Idle += new EventHandler(mainLoop);
		}

		#region game loop setup

		private bool AppStillIdle {
			get {
				NativeMessage msg;
				return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct NativeMessage {
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}

		[System.Security.SuppressUnmanagedCodeSecurity] // We won’t use this maliciously
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(out NativeMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		private void mainLoop(object sender, EventArgs e) {
			while (AppStillIdle && state != State.Dead) {
				if (_attemptConnection) {
					connect();
				}

				handleServerMessages();
				_game.update();
				draw();
			}
		}
		#endregion

		#region data

		private void connect() {
			_attemptConnection = false;
			_client.Address = _address;
			_client.Port = _port;
			_client.Start();

			_client.send(new Login(_username).toByteArray());
		}

		private void disconnect() {
			state = State.Disconnected;
			_game = new Game();
			_id = -1;
			_username = "";
			_client.Stop();
		}

		private void handleServerMessages() {
			List<Message> messages = _client.PendingMessages;

			foreach (Message m in messages) {
				BaseCommand com = BaseCommand.fromByteArray(m.data);
				if (com.type != ComType.Ping)
					logger.Log(com.ToString());

				switch (com.type) {
					case ComType.Login:
						serverLogin((Login)com);
						break;

					case ComType.Logout:
						serverLogout((Logout)com);
						break;

					case ComType.LoadGame:
						serverLoadGame((LoadGame)com);
						break;

					case ComType.Move:
						serverMove((Move)com);
						break;

					case ComType.Chat:
						serverChat((Chat)com);
						break;

					case ComType.Ping:
						serverPing((Ping)com);
						break;
				}
			}
		}

		private void serverLogin(Login com) {
			if (com.player == null) {
				throw new ArgumentNullException("Login command from the server returned null");
			}
			_game.addPlayer(com.player);
		}

		private void serverLogout(Logout com) {
			if (com.id == _id) {
				disconnect();
			}
			_game.removePlayer(com.id);
		}

		private void serverLoadGame(LoadGame com) {
			_game.loadPlayers(com.players);
			_id = com.id;
			state = State.Connected;
		}

		private void serverMove(Move com) {
			if (com.x != 0 && com.y != 0) {
				_game.setPlayerLocation(com.id, com.x, com.y);
			}
			_game.setPlayerMove(com.id, com.direction, com.isComplete);
		}

		private void serverChat(Chat com) {
			if (com.id != _id) {

				string message = "[" + com.chatType.ToString() + "] ";
				if (com.chatType == ChatType.Global || com.chatType == ChatType.Whisper) {
					message += com.sender + ": ";
				}
				message += com.message + "\n";
				chatDisplay.Text += message;
			}
		}

		private void serverPing(Ping com) {
			_ping = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - com.timestamp;
		}

		#endregion

		#region controller
		public void connect(string address, int port, string username, string password) {
			_address = address;
			_port = port;
			_username = username;
			_password = password;
			_attemptConnection = true;
		}

		private void sendMove(KeyEventArgs e, bool isComplete) {
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
				Direction dir = (Direction)Enum.Parse(typeof(Direction), e.KeyCode.ToString());
				if (_game.getPlayerById(_id).Moves[(int)dir] == isComplete) {
					_game.setPlayerMove(_id, dir, isComplete);
                    _client.send(new Move(_id, dir, isComplete).toByteArray());
				}
			}
		}

		private void GameApplication_FormClosed(object sender, FormClosedEventArgs e) {
			_client.send(new Logout().toByteArray());
			state = State.Dead;
		}

		private void GameApplication_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
				activateChat();
			}

			if (e.KeyCode == Keys.I) {
				_isInventoryOpen = !_isInventoryOpen;
			}

			sendMove(e, false);
		}

		private void GameApplication_KeyUp(object sender, KeyEventArgs e) {
			sendMove(e, true);
		}

		private void chatInput_SubmitChat(object sender, KeyEventArgs e) {
			if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && chatInput.Text != "") {
				Chat chat = new Chat(_id, _username, chatInput.Text);
				_client.send(chat.toByteArray());

				string data = "";
				if (chat.chatType == ChatType.Whisper) {
					data += "[To] " + chat.recipient + ": ";
				} else {
					data += "[Me] ";
				}
				data += chat.message + "\n";
				chatDisplay.Text += data;

				deactivateChat();

			} else if (e.KeyCode == Keys.Escape) {
				deactivateChat();
			}
		}

		private void GameForm_Resize(object sender, EventArgs e) {
			setControlSize();
		}

		private void pingger_Tick(object sender, EventArgs e) {
			_client.send(new Ping(_id).toByteArray());
		}

		#endregion

		#region view
		private void activateChat() {
			chatInput.Activate();
			chatDisplay.Activate();
		}

		private void deactivateChat() {
			chatInput.Deactivate();
			chatDisplay.Deactivate();
		}

		private void setControlSize() {

			Rectangle r = DisplayRectangle;

			display = new Bitmap(Width, Height);
			chatInput.Width = r.Width / 3;
			chatInput.Height = 50;
			chatInput.Location = new Point(0, r.Height - 50);

			chatDisplay.Width = r.Width / 3;
			chatDisplay.Height = r.Height / 2;
			chatDisplay.Location = new Point(0, r.Height - 55 - chatDisplay.Height);

			logger.Width = r.Width / 2;
			logger.Height = r.Height / 2;
			logger.Location = new Point(r.Width - logger.Width, 0);
		}

		private void connectToServer() {
			if (!loginForm.Visible) {
				loginForm.Show(this);
			}
		}

		private void draw() {
			if (state == State.Disconnected) {
				connectToServer();
				return;
			} else {
				loginForm.Hide();
			}

			Graphics g = Graphics.FromImage(display);

			g.FillRectangle(Brushes.Gray, new RectangleF(0, 0, Width, Height)); // create background
			drawPlayers(g);
			drawDebugger(g);

			g.Flush();
			g.Dispose();

			Graphics formGraphics = CreateGraphics();
			formGraphics.DrawImage(display, 0, 0);
			formGraphics.Flush();
			formGraphics.Dispose();

			chatInput.Refresh();
			chatDisplay.Refresh();
			logger.Refresh();
		}

		private void drawPlayers(Graphics g) {
			Font font = new Font("courier", 10);
			Player activePlayer = _game.getPlayerById(_id);

			float canvasCenter_x = Width / 2;
			float canvasCenter_y = Height / 2;
			float unitOffset = (UNIT / 2);
			float xOffset;
			float yOffset;
			float realPosition_x;
			float realPosition_y;
			float drawPosition_x;
			float drawPosition_y;
			SizeF textSize;

			float test = (float)(activePlayer.x - -20) * UNIT;
			float test2 = (float)(activePlayer.y - -20) * UNIT;
			g.FillRectangle(Brushes.Peru, new RectangleF((canvasCenter_x - test), (canvasCenter_y - test2), 40, 40));

			foreach (Player p in _game.Players) {
				xOffset = (float)((activePlayer.x - p.x) * UNIT);
				yOffset = (float)((activePlayer.y - p.y) * UNIT);

				realPosition_x = canvasCenter_x - xOffset;
				realPosition_y = canvasCenter_y - yOffset;

				drawPosition_x = realPosition_x - unitOffset;
				drawPosition_y = realPosition_y - unitOffset;

				// draw player
				RectangleF rect = new RectangleF(drawPosition_x, drawPosition_y, UNIT, UNIT);
				g.FillRectangle(Brushes.Red, rect);

				textSize = g.MeasureString(activePlayer.username, font);
				g.DrawString(p.username, font, Brushes.GreenYellow, (realPosition_x - (textSize.Width / 2)), (realPosition_y + (textSize.Height / 2) + unitOffset));
			}
		}

		private void drawDebugger(Graphics g) {
			// update delta
			long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			long delta = currentTime - _lastTime;
			_lastTime = currentTime;

			Font font = new Font("courier", 10);
			string[] debugStrings = new string[4];

			_fps = (int)((delta != 0) ? (1000 / delta) : _fps);

			debugStrings[0] = "Fps: " + _fps;
			debugStrings[1] = "Ping: " + _ping;
			debugStrings[2] = "Active Players: " + _game.Players.Count;
			debugStrings[3] = "Location: " + _game.getPlayerById(_id).x + ":" + _game.getPlayerById(_id).y;

			for (int i = 0; i < debugStrings.Length; i++) {
				string s = debugStrings[i];
				SizeF size = g.MeasureString(s, font);
				g.DrawString(s, font, Brushes.GreenYellow, 5, (i) * size.Height);
			}
		}
		#endregion

	}
}
