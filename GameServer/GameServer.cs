using System;
using System.Collections.Generic;
using System.Net;
using GameServer.Networking;
using GameServer.Data;
using Newtonsoft.Json.Linq;
using GameServer.Data.Interactables;

namespace GameServer
{
    public class GameServer
    {

        private class OutGoing
        {
            public IPEndPoint[] clients;
            public JObject data;

            public OutGoing(IPEndPoint[] recipients, JObject com)
            {
                clients = recipients;
                data = com;
            }

            public OutGoing(IPEndPoint recipient, JObject com)
            {
                clients = new IPEndPoint[] { recipient };
                data = com;
            }

            public OutGoing(List<User> recipients, JObject com)
            {
                clients = User.getEndPointArray(recipients);
                data = com;
            }

            public override string ToString()
            {
                return data.ToString() + " to " + clients.Length + " Clients";
            }
        }

        private class User
        {
            public IPEndPoint location;
            public int id;
            public long lastMessage;

            public bool TimedOut
            {
                get
                {
                    return Helper.getTimestamp() - lastMessage >= CLIENT_TIMEOUT;
                }
            }

            public User(int id, IPEndPoint location, long lastMessage)
            {
                this.location = location;
                this.id = id;
                this.lastMessage = lastMessage;
            }

            public static IPEndPoint[] getEndPointArray(List<User> users)
            {
                IPEndPoint[] endPoints = new IPEndPoint[users.Count];
                for (int i = 0; i < users.Count; i++)
                {
                    endPoints[i] = users[i].location;
                }
                return endPoints;
            }
        }

        public const long CLIENT_TIMEOUT = 120000;

        private List<User> _users;
        private Server _server;
        private Game _game;
        private Dictionary<IPEndPoint, byte[]> _inComing;
        private List<OutGoing> _outGoing;
        private List<User> _toRemove;


        public GameServer()
        {
            _users = new List<User>();
            _server = new Server(1234);
            _game = new Game();
            _inComing = new Dictionary<IPEndPoint, byte[]>();
            _outGoing = new List<OutGoing>();
            _toRemove = new List<User>();

            _server.Start();
            mainLoop();
        }

        private void mainLoop()
        {
            Console.WriteLine("Starting game loop...");
            while (true)
            {
                Helper.getDelta(true);
                handleRemovedClients();
                handleClientData();
                updateClientRequests();
                _game.update();
                sendDataToClients();
                //checkForDissconnects();

            }
        }

        private void checkForDissconnects()
        {
            foreach (User u in _users)
            {
                if (u.TimedOut)
                    _toRemove.Add(u);
            }
        }

        private void handleRemovedClients()
        {

            foreach (User user in _toRemove)
            {
                string username = "";
                if (user.id != -1)
                {
                    username = _game.Players.getPlayerById(user.id).Name;
                    _game.Players.removePlayer(user.id);
                    _outGoing.Add(new OutGoing(_users, Transmition.Logout.Create(user.id)));

                    Logger.Log(Logger.LogLevel.normal, Logger.Type.DISCONNECTED, string.Format("{0} | {1}", user.location, username));
                    _users.Remove(user);
                }
                else
                {
                    _users.Remove(user);
                }
            }
            _toRemove.Clear();
        }

        private void handleClientData()
        {
            Dictionary<IPEndPoint, byte[]> messages = _server.PendingMessages;

            foreach (KeyValuePair<IPEndPoint, byte[]> m in messages)
            {
                string errorMessage;
                JObject com = Transmition.Parse(m.Value, out errorMessage);
                if (com != null)
                {
                    // add new users
                    User temp = new User(-1, m.Key, Helper.getTimestamp());
                    if (!_users.Exists(u => u.location.Equals(temp.location)))
                    {
                        _users.Add(temp);
                    }
                    _inComing.Add(m.Key, m.Value);
                }
                else
                {
                    Logger.Log(Logger.LogLevel.normal, Logger.Type.ERROR, string.Format("TransmisionError: {0}", errorMessage));
                    JObject error = Transmition.Error.Create("TransmisionError", 605, errorMessage);
                    _outGoing.Add(new OutGoing(m.Key, error));
                }
            }
        }

        private void updateClientRequests()
        {

            foreach (KeyValuePair<IPEndPoint, byte[]> m in _inComing)
            {
                int userIndex = _users.FindIndex(u => u.location.Equals(m.Key));
                string emessage;
                JObject com = Transmition.Parse(m.Value, out emessage);
                if (com != null)
                {
                    User sender = _users[userIndex];
                    string comType = com[Transmition.Base.TYPE].Value<string>();

                    if (comType != Transmition.TransmitionTypes.LOGIN)
                    {
                        sender.lastMessage = com[Transmition.Base.TIME_STAMP].Value<long>();
                    }

                    if (sender.id == -1 &&
                        comType != Transmition.TransmitionTypes.LOGIN &&
                        comType != Transmition.TransmitionTypes.LOGOUT &&
                        comType != Transmition.TransmitionTypes.PING)
                    {

                        string message = string.Format("{0} | sent the command: {1} without first logging in", sender.location, comType);
                        Logger.Log(Logger.LogLevel.normal, Logger.Type.ERROR, message);
                        JObject errorMessage = Transmition.Error.Create("LoginError", 1, message);
                        _outGoing.Add(new OutGoing(sender.location, errorMessage));
                        logout(sender);
                        continue;

                    }

                    switch (comType)
                    {
                        case Transmition.TransmitionTypes.PING:
                            ping(sender);
                            break;

                        case Transmition.TransmitionTypes.LOGIN:
                            login(sender, com);
                            break;

                        case Transmition.TransmitionTypes.LOGOUT:
                            logout(sender);
                            break;

                        case Transmition.TransmitionTypes.MOVE:
                            move(sender, com);
                            break;

                        case Transmition.TransmitionTypes.PLACE_TURRET:
                            place_turret(sender, com);
                            break;

                            /*case ComType.Chat:
                                //userChat(sender, (Chat)com);
                                break;

                            case ComType.Inventory:
                                //userInventory(sender, (Data.Inventory)com);
                                break;

                            case ComType.Interact:
                                //userInteract(sender, (Interact)com);
                                break;
                            */
                    }
                }
            }
            _inComing.Clear();
        }

        private void login(User sender, JObject com)
        {
            int id;
            string username = com[Transmition.Login.USERNAME].Value<string>();


            // if the player is currently send an error and stop the login process
            if (sender.id != -1)
            {
                string name = _game.Players.getPlayerById(sender.id).Name;

                Logger.Log(Logger.LogLevel.normal, Logger.Type.ERROR, string.Format("Multiple login attemps {0} | {1}", sender.location, name));
                JObject error = Transmition.Error.Create("CurrentlyActive", 345, string.Format("This user is currently active as: {0}", name));
                _outGoing.Add(new OutGoing(sender.location, error));
                return;
            }

            if ((id = _game.Players.addPlayer(username)) != -1)
            {
                // update user
                int sendersUserIndex = _users.FindIndex(u => u.location == sender.location);
                sender.id = id;
                _users[sendersUserIndex] = sender;
                Logger.Log(Logger.LogLevel.normal, Logger.Type.CONNECTED, string.Format("{0} | {1}", sender.location, username));

                // let the user know they are logged in
                com[Transmition.Base.ID] = id;
                _outGoing.Add(new OutGoing(sender.location, com));

                // send to everyone accept the user logging in
                List<User> subUsers = _users.FindAll(u => u.id != id);
                if (subUsers.Count > 0)
                {
                    JObject response = Transmition.Login.Create(id, _game.Players.getPlayerById(id).Name);
                    _outGoing.Add(new OutGoing(subUsers, com));
                }

            }
            else
            {
                JObject error = Transmition.Error.Create("NameInUse", 344, string.Format("The name '{0}' is currently in use. Please use a different name to login", username));
                _outGoing.Add(new OutGoing(sender.location, error));

                Logger.Log(Logger.LogLevel.normal, Logger.Type.ERROR, string.Format("The name '{0}' is already in use", username));
            }
        }

        private void logout(User sender)
        {
            _toRemove.Add(sender);
        }

        private void ping(User sender)
        {
            if (sender.id != -1)
            {
                JObject ping = Transmition.Ping.Create(sender.id);
                Logger.Log(Logger.LogLevel.debug, Logger.Type.PING, string.Format("{0} | {1}", _game.Players.getPlayerById(sender.id).Name, ping[Transmition.Base.TIME_STAMP]));
                _outGoing.Add(new OutGoing(sender.location, ping));
            }
        }

        private void move(User sender, JObject com)
        {
            int direction = com[Transmition.Move.DIRECTION].Value<int>();
            bool isComplete = com[Transmition.Move.COMPLETE].Value<bool>();

            _game.Players.getPlayerById(sender.id).setMove(direction, isComplete);

            Player p = _game.Players.getPlayerById(sender.id);
            Logger.Log(Logger.LogLevel.normal, Logger.Type.GAME_ACTION, string.Format("{0}\tMove: {1:F2}:{2:F2}", ((!isComplete) ? "START" : "FINISH"), p.X, p.Y));
            _outGoing.Add(new OutGoing(_users, Transmition.Move.Create(sender.id, direction, isComplete, p.X, p.Y)));
        }

        private void place_turret(User sender, JObject com)
        {
            int x = com[Transmition.PlaceTurret.X].Value<int>();
            int y = com[Transmition.PlaceTurret.Y].Value<int>();

            string errorMessage;
            if (_game.placeTurret(sender.id, x, y, out errorMessage))
            {
                Logger.Log(Logger.LogLevel.normal, Logger.Type.GAME_ACTION, string.Format("Place Turret: {0:F2}:{1:F2}", x, y));
                com[Transmition.Base.ID] = sender.id;
                _outGoing.Add(new OutGoing(_users, com));
            }
            else
            {
                Logger.Log(Logger.LogLevel.normal, Logger.Type.ERROR, string.Format("PlaceTurretException: {0}", errorMessage));
                _outGoing.Add(new OutGoing(sender.location, Transmition.Error.Create("PlaceTurretException", 2435, errorMessage)));
            }
        }

        /*private void userChat(User sender, Chat com) {
			if (com.chatType == ChatType.Global) {
				_outGoing.Add(new OutGoing(_users, com));

			} else if (com.chatType == ChatType.Whisper) {
				int id = _game.Players.getIdByUsername(com.recipient);
				User recipient = _users.Find(u => u.id == id);
				if (recipient == null) {
					Chat c = new Chat("Recipient: " + com.recipient + "does not exist");
					_outGoing.Add(new OutGoing(sender.location, c));
				} else {
					_outGoing.Add(new OutGoing(recipient.location, com));
				}
			}
		}

		private void userInventory(User sender, Data.Inventory com) {
			Player p = _game.Players.getPlayerById(sender.id);
			switch (com.invType) {

				case Data.Inventory.TYPE.Move:
					p.Inventory.move(com.itemIndex1, com.itemIndex2);
					break;

				case Data.Inventory.TYPE.Add:
					p.Inventory.add(com.itemIndex1, com.item);
					break;

				case Data.Inventory.TYPE.Remove:
					p.Inventory.remove(com.itemIndex1);
					break;

				case Data.Inventory.TYPE.Combine:
					break;

				case Data.Inventory.TYPE.Split:
					break;

			}
			com.updatedInventory = p.Inventory.getArray();
			_outGoing.Add(new OutGoing(sender.location, com));
		}

		private void userInteract(User sender, Interact com) {
			Player p = _game.Players.getPlayerById(sender.id);

			List<GameObject> objects = _game.GameObjects.getGameObjectsInRange(p);

			foreach (GameObject thing in objects) {
				if (thing.name == "Kiln") {
					p.ActiveMiniGame = ((Kiln)thing).game;
                }
			}

			com.Player = p;
			_outGoing.Add(new OutGoing(sender.location, com));
		}*/

        private void sendDataToClients()
        {
            foreach (OutGoing message in _outGoing)
            {
                byte[] data = Transmition.Serialize(message.data);
                _server.broadcast(message.clients, data);
            }
            _outGoing.Clear();
        }

        static void Main(string[] args)
        {
            GameServer g = new GameServer();
        }
    }
}
