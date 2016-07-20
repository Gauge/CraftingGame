﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameServer;
using GameServer.Networking;
using Message = GameServer.Networking.Message;
using System.Reflection;
using System.Text;

namespace GameFrontEndDebugger {
	public partial class GameForm : Form {

		public int UNIT = 15;
		private int _id = -1;
		private string _username;
		private bool[] _moves;
		private long _lastTime;
		private long _ping;

		private Client _client;
		private Game _game;
		private LoginForm _loginForm;
		private LoggerForm _logger;

		public GameForm() {
			InitializeComponent();
		}

		public void connect(string address, int port, string username, string password) {
			_username = username;
			_client.Address = address;
			_client.Port = port;
			_client.Start();

			sendMessage(new Login(username).toByteArray());
		}

		private void connectToServer() {
			if (!_loginForm.Visible) {
				_id = -1;
				gameCanvas.BackgroundImage = null;
				_loginForm.Show(this);
			}
		}

		public void dissconnect() {
			_logger.Log("disconnecting from server...");
			_client.Stop();
			_id = -1;
			_username = "";
			_game = new Game();
			connectToServer();
		}

		private void Form1_Load(object sender, EventArgs e) {
			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, gameCanvas, new object[] { true });

			_loginForm = new LoginForm();
			_logger = new LoggerForm();
			_game = new Game();
			_client = new Client();
			_moves = new bool[] { false, false, false, false };

			_logger.Show(this);
			connectToServer();
		}

		private void GameApplication_FormClosed(object sender, FormClosedEventArgs e) {
			sendMessage(new Logout().toByteArray());
			dissconnect();
		}

		private void GameApplication_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
				chatInput.Enabled = true;
				chatInput.ReadOnly = false;
				chatInput.Focus();
			}
			sendMove(e, false);
		}

		private void GameApplication_KeyUp(object sender, KeyEventArgs e) {
			sendMove(e, true);
		}

		private void chatInput_KeyDown(object sender, KeyEventArgs e) {
			if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && chatInput.Text != "") {
				Chat chat = new Chat(_id, _username, chatInput.Text);
				sendMessage(chat.toByteArray());

				string data = "";
				if (chat.chatType == ChatType.Wisper) {
					data += "[To] " + chat.recipient + ": ";
				} else {
					data += "[Me] ";
				}
				data += chat.message + "\n";
				chatDisplay.Text += data;

				chatInput.Clear();
				chatInput.Enabled = false;
				chatInput.ReadOnly = true;
			}

			if (e.KeyCode == Keys.Escape) {
				chatInput.Clear();
				chatInput.Enabled = false;
				chatInput.ReadOnly = true;
			}
		}

		private void HandleServerMessages() {
			List<Message> messages = _client.PendingMessages;

			foreach (Message m in messages) {
				BaseCommand com = BaseCommand.fromByteArray(m.data);
				if (com.type != ComType.Ping)
					_logger.Log(com.ToString());

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
				dissconnect();
			}
			_game.removePlayer(com.id);
		}

		private void serverLoadGame(LoadGame com) {
			_game.loadPlayers(com.players);
			_id = com.id;
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
				if (com.chatType == ChatType.Global || com.chatType == ChatType.Wisper) {
					message += com.sender + ": ";
				}
				message += com.message + "\n";
				chatDisplay.Text += message;
			}
		}

		private void serverPing(Ping com) {
			_ping = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - com.timestamp;
		}

		private void UpdateCanvas() {
			if (_id == -1) return;

			int width = gameCanvas.Width;
			int height = gameCanvas.Height;

			Bitmap drawCanvas = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(drawCanvas);
			Font font = new Font("courier", 10);

			// create background
			g.FillRectangle(Brushes.Gray, new RectangleF(0, 0, width, height));

			Player activePlayer = _game.getPlayerById(_id);

			float canvasCenter_x = width / 2;
			float canvasCenter_y = height / 2;
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

			updateDebugger(g);

			g.Flush();
			gameCanvas.BackgroundImage = drawCanvas;
		}

		private void updateDebugger(Graphics g) {
			// update delta
			long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			long delta = currentTime - _lastTime;
			_lastTime = currentTime;

			Font font = new Font("courier", 10);
			string[] debugStrings = new string[4];

			debugStrings[0] = "Fps: " + (1000 / delta);
			debugStrings[1] = "Ping: " + _ping;
			debugStrings[2] = "Active Players: " + _game.Players.Count;
			debugStrings[3] = "Location: " + _game.getPlayerById(_id).x + ":" + _game.getPlayerById(_id).y;

			for (int i = 0; i < debugStrings.Length; i++) {
				string s = debugStrings[i];
				SizeF size = g.MeasureString(s, font);
				g.DrawString(s, font, Brushes.GreenYellow, 5, (i) * size.Height);
			}
		}


		private void sendMove(KeyEventArgs e, bool isComplete) {
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
				Direction dir = (Direction)Enum.Parse(typeof(Direction), e.KeyCode.ToString());
				if (_moves[(int)dir] == isComplete) {
					_moves[(int)dir] = _game.setPlayerMove(_id, dir, isComplete);
					sendMessage(new Move(_id, dir, isComplete ).toByteArray());
				}
			}
		}

		private void sendMessage(byte[] com) {
			if (_client.IsConnected) {
				_client.send(com);
			}

		}

		private void mainLoop_Tick(object sender, EventArgs e) {
			if (!_client.IsConnected) {
				connectToServer();
			}

			HandleServerMessages();
			_game.update();
			UpdateCanvas();
		}

		private void pingger_Tick(object sender, EventArgs e) {
			sendMessage(new Ping(_id).toByteArray());
		}
	}
}
