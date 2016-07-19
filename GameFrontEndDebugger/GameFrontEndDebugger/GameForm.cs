using System;
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

        public int UNIT = 10;

        private int _id = -1;
        private string _username;
        private bool _isKeyPressed = false;
        private long _lastTime;
        private long _ping;

        private Client _client;
        private Game _game;
        private LoginForm _loginForm;
        private LoggerForm _logger;
        
        public GameForm() {
            InitializeComponent();
        }

        private void connectToServer() {
            if (!_loginForm.Visible) {
                _id = -1;
                gameCanvas.BackgroundImage = null;
                _loginForm.Show(this);
            }
        }

        public void connect(string address, int port, string username, string password) {
            _username = username;
            _client.Address = address;
            _client.Port = port;
            _client.Start();

            sendMessage(new Login(username).toByteArray());
        }

        public void dissconnect() {
            _logger.Log("disconnecting from server...");
            _client.Stop();
            _id = -1;
            _username = "";
            _game = new Game();
        }

        private void Form1_Load(object sender, EventArgs e) {
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, gameCanvas, new object[] { true });

            _loginForm = new LoginForm();
            _logger = new LoggerForm();
            _game = new Game();
            _client = new Client();

            _logger.Show(this);
            connectToServer();
        }

        private void GameApplication_FormClosed(object sender, FormClosedEventArgs e) {
            sendMessage(new Logout().toByteArray());
            _client.Stop();
            Environment.Exit(0);
        }

        private void GameApplication_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
                chatInput.Enabled = true;
                chatInput.ReadOnly = false;
                chatInput.Focus();
            }

            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) && _isKeyPressed == false) {
            
                Direction dir;
                if (e.KeyCode == Keys.Left) {
                    dir = Direction.Left;
                } else if (e.KeyCode == Keys.Right) {
                    dir = Direction.Right;
                } else if (e.KeyCode == Keys.Up) {
                    dir = Direction.Up;
                } else {
                    dir = Direction.Down;
                }
                _game.setPlayerMove(_id, dir, false);

                sendMessage(new Move(_id, dir, false).toByteArray());
                _isKeyPressed = true;
            }
        }

        private void GameApplication_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                Direction dir;
                if (e.KeyCode == Keys.Left) {
                    dir = Direction.Left;
                } else if (e.KeyCode == Keys.Right) {
                    dir = Direction.Right;
                } else if (e.KeyCode == Keys.Up) {
                    dir = Direction.Up;
                } else {
                    dir = Direction.Down;
                }
                _game.setPlayerMove(_id, dir, true);

                sendMessage(new Move(_id, dir, true).toByteArray());
                _isKeyPressed = false;
            }
        }

        private void chatInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) {

                Chat chat = new Chat(_id, _username ,chatInput.Text);
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
        }

        private void serverLogin(Login com) {
            if (com.player == null) return;
            _game.addPlayer(com.player);
        }

        private void serverLogout(Logout com) {
            if (com.id == _id || com.id == -1) {
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
                //_logger.Log("Location Offset: " + (_game.getPlayerById(id).x - com.x) + ":" + (_game.getPlayerById(id).y - com.y));
                _game.setPlayerLocation(com.id, com.x, com.y);
            }
            _game.setPlayerMove(com.id, com.direction, com.isComplete);
        }

        private void serverChat(Chat com) {
            if (com.id != _id) { 

                string message = "[" + com.chatType.ToString() + "] ";
                if (com.chatType == ChatType.Global || com.chatType == ChatType.Wisper)
                {
                    message += com.sender + ": ";
                }
                message += com.message + "\n";
                chatDisplay.Text += message;
            }
        }

        private void serverPing(Ping com) {
            _ping = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - com.timestamp;
        }

        private void HandleServerMessages() {
            List<Message> messages = _client.PendingMessages;

            foreach (Message m in messages) {
                BaseCommand com = BaseCommand.fromByteArray(m.data);
                if (com.type != ComType.Ping) _logger.Log(Encoding.ASCII.GetString(m.data));

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

            int canvasCenter_x = width / 2;
            int canvasCenter_y = height / 2;
            int unitOffset = (UNIT / 2);
            int xOffset;
            int yOffset;
            int realPosition_x;
            int realPosition_y;
            int drawPosition_x;
            int drawPosition_y;
            SizeF textSize;

            int test = (int)(activePlayer.x - -20) * UNIT;
            int test2 = (int)(activePlayer.y - -20) * UNIT;
            g.FillRectangle(Brushes.Peru, new RectangleF((canvasCenter_x - test), (canvasCenter_y - test2), 40, 40));

            foreach (Player p in _game.Players) {
                xOffset = (int)((activePlayer.x - p.x) * UNIT);
                yOffset = (int)((activePlayer.y - p.y) * UNIT);
                
                realPosition_x = canvasCenter_x - xOffset;
                realPosition_y = canvasCenter_y - yOffset;

                drawPosition_x = realPosition_x - unitOffset;
                drawPosition_y = realPosition_y - unitOffset;

                // draw player
                RectangleF rect = new RectangleF(drawPosition_x, drawPosition_y, UNIT, UNIT);
                g.FillRectangle(Brushes.Red, rect);

                textSize = g.MeasureString(activePlayer.username, font);
                g.DrawString(p.username, font, Brushes.GreenYellow, (realPosition_x-(textSize.Width/2)), (realPosition_y + (textSize.Height/2) + unitOffset));
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
            debugStrings[3] = "Location: " +_game.getPlayerById(_id).x + ":" + _game.getPlayerById(_id).y;

            for (int i=0; i<debugStrings.Length; i++) {
                string s = debugStrings[i];
                SizeF size = g.MeasureString(s, font);
                g.DrawString(s, font, Brushes.GreenYellow, 5, (i) * size.Height);
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
