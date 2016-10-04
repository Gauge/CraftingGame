using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameServer.Data;
using GameServer.Networking;
using GameFrontEndDebugger.Controls;
using Message = GameServer.Networking.Message;
using Logger = GameFrontEndDebugger.Controls.Logger;
using System.Runtime.InteropServices;
using GameServer.Data.Interactables;
using DeltaTimer = GameServer.Data.Settings;

namespace GameFrontEndDebugger {
	public partial class GameForm : Form {
		private enum STATE {
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

		// game data
		private Game _game;
		private STATE state;
		public Player activePlayer;

		// display data
		private long _ping;
		private int _fps;

		private ChatDisplay chatDisplay;
		private ChatInput chatInput;
		private LoginForm loginForm;
		private Logger logger;
		private TransmitterForm transmitter;
		private InventoryForm inventoryForm;
		private MG_Smelting smeltingForm;
		public Bitmap display { get; set; }

		public GameForm() {
			DoubleBuffered = true;

			state = STATE.Disconnected;
			_game = new Game();
			_client = new Client();
			display = new Bitmap(Width, Height);
			logger = new Logger();
			chatInput = new ChatInput();
			chatDisplay = new ChatDisplay();

			// forms
			loginForm = new LoginForm();
			transmitter = new TransmitterForm();
			inventoryForm = new InventoryForm();
			smeltingForm = new MG_Smelting();

			chatInput.KeyDown += chatInput_SubmitChat;

			Controls.Add(chatInput);
			Controls.Add(chatDisplay);
			Controls.Add(logger);

			InitializeComponent();
			setControlSize();
			connectToServer();

			inventoryForm.Owner = this;
			smeltingForm.Owner = this;
			loginForm.Owner = this;
			transmitter.Owner = this;

			if (Settings.Debug) {
				transmitter.Show(this);
			} else {
				logger.Hide();
			}

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
			while (AppStillIdle && state != STATE.Dead) {
				DeltaTimer.getDelta(true);

				if (_attemptConnection) {
					connect();
				}

				handleServerMessages();
				_game.update();
				activePlayer = _game.Players.getPlayerById(_id);
				inventoryForm.update();
				smeltingForm.update();
				transmitter.update();
				draw();
			}

			if (state == STATE.Dead) {
				Dispose();
				Application.Exit();
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
			state = STATE.Disconnected;
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
					case ComType.Ping:
						serverPing((Ping)com);
						break;

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

					case ComType.Inventory:
						serverInventory((GameServer.Data.Inventory)com);
                        break;

					case ComType.Interact:
						_game.Players.setPlayer(((Interact)com).Player);
						break;
				}
			}
		}  

		private void serverLogin(Login com) {
			if (com.player == null) {
				throw new ArgumentNullException("Login command from the server returned null");
			}
			_game.Players.addPlayer(com.player);
		}

		private void serverLogout(Logout com) {
			if (com.id == _id) {
				disconnect();
			}
			_game.Players.removePlayer(com.id);
		}

		private void serverLoadGame(LoadGame com) {
			_game.Players.loadPlayers(com.players);
			_id = com.id;
			state = STATE.Connected;
		}

		private void serverMove(Move com) {
			if (com.x != 0 && com.y != 0) {
				_game.Players.setPlayerLocation(com.id, com.x, com.y);
			}
			_game.Players.setPlayerMove(com.id, com.direction, com.isComplete);
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

		private void serverInventory(GameServer.Data.Inventory com) {
			activePlayer.Inventory.initialize(com.updatedInventory);
		}

		#endregion

		#region controller
		public void transmit(string data) {
			_client.send(data);
		}

		public void transmit(BaseCommand com) {
			_client.send(com.toByteArray());
		}

		public void connect(string address, int port, string username, string password) {
			_address = address;
			_port = port;
			_username = username;
			_password = password;
			_attemptConnection = true;
		}

		private void sendMove(KeyEventArgs e, bool isComplete) {
			Direction dir = Settings.keyDirection(e.KeyCode);

			if (dir != Direction.None) {
				if (_game.Players.getPlayerById(_id).Moves[(int)dir] == isComplete) {
					_game.Players.setPlayerMove(_id, dir, isComplete);
					transmit(new Move(_id, dir, isComplete));
				}
			}
		}

		private void GameApplication_FormClosing(object sender, FormClosingEventArgs e) {
			transmit(new Logout());
			state = STATE.Dead;
		}

		private void GameApplication_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
				activateChat();
			}

			if (e.KeyCode == Keys.I) {

				if (!inventoryForm.Visible) {
					inventoryForm.Show();
					Focus();
				} else {
					inventoryForm.Hide();
				}
			}

			if (e.KeyCode == Keys.E) {
				transmit(new Interact());
            }

			sendMove(e, false);
		}

		private void GameApplication_KeyUp(object sender, KeyEventArgs e) {
			sendMove(e, true);
		}

		private void chatInput_SubmitChat(object sender, KeyEventArgs e) {
			if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && chatInput.Text != "") {
				Chat chat = new Chat(_id, _username, chatInput.Text);
				transmit(chat);

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
			transmit(new Ping(_id));
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


		private RectangleF translateToCameraView(double x, double y, double width, double height) {
			RectangleF r = new RectangleF();
			r.X = ((Width / 2) - ((float)(activePlayer.x - x) * Settings.UNIT)) - (float)(Settings.UNIT* width / 2);
			r.Y = ((Height / 2) - ((float)(activePlayer.y - y) * Settings.UNIT)) - (float)(Settings.UNIT* height / 2);
			r.Width = (float)(Settings.UNIT * width);
			r.Height = (float)(Settings.UNIT * height);
			return r;

		}

		private Font _font = new Font("courier", 10);
		private SolidBrush _highlightBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0));
		private SolidBrush _nonHighlightBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
		private Pen _highlightPen = new Pen(Color.FromArgb(50, 0, 255, 0), 3);
		private Pen _nonHighlightPen = new Pen(Color.FromArgb(25, 0, 0, 0), 3);
		private void draw() {
			if (state == STATE.Disconnected) {
				connectToServer();
				return;
			} else {
				loginForm.Hide();
			}

			Graphics g = Graphics.FromImage(display);

			g.FillRectangle(Brushes.Gray, new RectangleF(0, 0, Width, Height)); // create background
			drawInteractables(g);
			drawPlayers(g);

			if (Settings.Debug) {
				drawDebugger(g);
			}

			g.Flush();
			g.Dispose();

			Graphics formGraphics = CreateGraphics();
			formGraphics.DrawImage(display, 0, 0);
			formGraphics.Flush();
			formGraphics.Dispose();

			chatInput.Refresh();
			chatDisplay.Refresh();

			if (Settings.Debug) {
				logger.Refresh();
			}
		}

		private void drawPlayers(Graphics g) {
            //g.FillRectangle(Brushes.Peru, translateToCameraView(-5, -5, 2, 2));

			foreach (Player p in _game.Players) {
				RectangleF rect = translateToCameraView(p.x, p.y, 1, 1);
                g.FillRectangle(Brushes.Red, rect);

				SizeF textSize = g.MeasureString(activePlayer.name, _font);
				g.DrawString(p.name, _font, Brushes.GreenYellow, (rect.X - (textSize.Width / 2)) + (Settings.UNIT/2), (rect.Y + (textSize.Height / 2)) + (Settings.UNIT/2));
			}
		}

		private void drawInteractables(Graphics g) {
			foreach (GameObject obj in _game.GameObjects) {
				g.DrawImage(Assets.getIconById(obj.id), translateToCameraView(obj.x, obj.y, 2, 2));

				if (Settings.Debug) {
					if (obj.isInRange(activePlayer)) {
						g.FillEllipse(_highlightBrush, translateToCameraView(obj.x, obj.y, obj.actionRadious, obj.actionRadious));
						g.DrawEllipse(_highlightPen, translateToCameraView(obj.x, obj.y, obj.actionRadious, obj.actionRadious));
					} else {
						g.FillEllipse(_nonHighlightBrush, translateToCameraView(obj.x, obj.y, obj.actionRadious, obj.actionRadious));
						g.DrawEllipse(_nonHighlightPen, translateToCameraView(obj.x, obj.y, obj.actionRadious, obj.actionRadious));
					}
				}
			}
		}

		private void drawDebugger(Graphics g) {

			long delta = DeltaTimer.getDelta();

			Font font = new Font("courier", 10);
			string[] debugStrings = new string[4];

			_fps = (int)((delta != 0) ? (1000 / delta) : _fps);

			debugStrings[0] = "Fps: " + _fps;
			debugStrings[1] = "Ping: " + _ping;
			debugStrings[2] = "Active Players: " + _game.Players.Count;
			debugStrings[3] = "Location: " + _game.Players.getPlayerById(_id).x + ":" + _game.Players.getPlayerById(_id).y;

			for (int i = 0; i < debugStrings.Length; i++) {
				string s = debugStrings[i];
				SizeF size = g.MeasureString(s, font);
				g.DrawString(s, font, Brushes.GreenYellow, 5, (i) * size.Height);
			}
		}
		#endregion

	}
}
