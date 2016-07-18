using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameServer;
using System.Text;
using System.Reflection;

namespace GameFrontEndDebugger
{
    public partial class GameApplication : Form {

        public int UNIT = 10;

        private int _userID = -1;
        private string _username;
        private bool _isKeyPressed = false;
        private long _lastTime;
        private long _ping;

        private Client _client;
        private Game _game;
        private Login _loginForm;
        private Logger _logger;
        
        public GameApplication() {
            InitializeComponent();
        }

        private void connectToServer() {
            if (!_loginForm.Visible) {
                _userID = -1;
                gameCanvas.BackgroundImage = null;
                _loginForm.Show(this);
            }
        }

        public void connect(string address, int port, string username, string password) {
            _username = username;
            _client.Address = address;
            _client.Port = port;
            _client.Start();

            sendMessage(Communication.login(username));
        }

        public void dissconnect() {
            _logger.Log("disconnecting from server...");
            _client.Stop();
            _userID = -1;
            _username = "";
            _game = new Game();
        }

        private void Form1_Load(object sender, EventArgs e) {
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, gameCanvas, new object[] { true });

            _loginForm = new Login();
            _logger = new Logger();
            _game = new Game();
            _client = new Client();

            _logger.Show(this);
            connectToServer();
        }

        private void GameApplication_FormClosed(object sender, FormClosedEventArgs e) {
            sendMessage(Communication.logout());
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
                Communication message = new Communication();
                message.type = ComType.MOVE;
                message.username = _username;

                Direction dir;
                if (e.KeyCode == Keys.Left) {
                    dir = Direction.LEFT;
                } else if (e.KeyCode == Keys.Right) {
                    dir = Direction.RIGHT;
                } else if (e.KeyCode == Keys.Up) {
                    dir = Direction.UP;
                } else {
                    dir = Direction.DOWN;
                }

                Move m = new Move(_userID, dir, false);
                message.moveAction = m;
                _game.setPlayerMove(m);

                sendMessage(message);
                _isKeyPressed = true;
            }
        }

        private void GameApplication_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                Communication message = new Communication();
                message.type = ComType.MOVE;
                message.username = _username;

                Direction dir;
                if (e.KeyCode == Keys.Left) {
                    dir = Direction.LEFT;
                } else if (e.KeyCode == Keys.Right) {
                    dir = Direction.RIGHT;
                } else if (e.KeyCode == Keys.Up) {
                    dir = Direction.UP;
                } else {
                    dir = Direction.DOWN;
                }

                Move m = new Move(_userID, dir, true);
                message.moveAction = m;
                _game.setPlayerMove(m);

                sendMessage(message);
                _isKeyPressed = false;
            }
        }

        private void chatInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) {
                Chat chat = new Chat(chatInput.Text);
                Communication com = new Communication();
                com.type = ComType.CHAT;
                com.username = _username;
                com.chat = chat;
                sendMessage(com);

                string data = "";
                if (chat.type == ChatType.Wisper) {
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

        private void HandleServerMessages() {
            List<GameServer.Message> messages = _client.PendingMessages;

            foreach (GameServer.Message m in messages) {
                Communication com = Communication.fromByteArray(m.data);
                if (com.type != ComType.PING) _logger.Log(com.ToString());

                switch (com.type) {

                    case ComType.LOGIN:
                        _game.addPlayer(com.newPlayer);
                        break;

                    case ComType.LOGOUT:
                        if (com.playerID == _userID || com.playerID == -1) {
                            dissconnect();
                        }
                        _game.removePlayer(com.playerID);
                        break;

                    case ComType.LOAD_GAME:
                        _game.loadPlayers(com.players);
                        _userID = com.playerID;
                        break;

                    case ComType.MOVE:
                        int id = _game.getIdByUsername(com.username);
                        if (com.x != 0 && com.y != 0) {
                            //_logger.Log("Location Offset: " + (_game.getPlayerById(id).x - com.x) + ":" + (_game.getPlayerById(id).y - com.y));
                            _game.setPlayerLocation(id, com.x, com.y);
                        }
                        _game.setPlayerMove(com.moveAction);
                        break;

                    case ComType.CHAT:
                        if (com.username == _username) break;

                        string message = "[" + com.chat.type.ToString() + "] ";
                        if (com.chat.type == ChatType.Global || com.chat.type == ChatType.Wisper) {
                            message += com.username + ": ";
                        }
                        message += com.chat.message + "\n";
                        chatDisplay.Text += message;
                        break;

                    case ComType.PING:
                        _ping = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - com.timestamp;
                        break;
                }
            }
        }

        private void UpdateCanvas() {
            if (_userID == -1) return;

            int width = gameCanvas.Width;
            int height = gameCanvas.Height;

            Bitmap drawCanvas = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(drawCanvas);
            Font font = new Font("courier", 10);
            
            // create background
            g.FillRectangle(Brushes.Gray, new RectangleF(0, 0, width, height));

            Player activePlayer = _game.getPlayerById(_userID);

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
            debugStrings[3] = "Location: " +_game.getPlayerById(_userID).x + ":" + _game.getPlayerById(_userID).y;

            for (int i=0; i<debugStrings.Length; i++) {
                string s = debugStrings[i];
                SizeF size = g.MeasureString(s, font);
                g.DrawString(s, font, Brushes.GreenYellow, 5, (i) * size.Height);
            }
        }

        private void sendMessage(Communication com) {
            if (_client.IsConnected) {
                _client.send(Communication.toByteArray(com));
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
            sendMessage(Communication.ping());
        }
    }
}
