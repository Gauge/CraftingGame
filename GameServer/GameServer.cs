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
        private Queue<KeyValuePair<IPEndPoint, JObject>> _inComing;
        private Queue<OutGoing> _outGoing;
        private Queue<User> _toRemove;


        public GameServer()
        {
            _users = new List<User>();
            _server = new Server(1234);
            _game = new Game();
            _inComing = new Queue<KeyValuePair<IPEndPoint, JObject>>();
            _outGoing = new Queue<OutGoing>();
            _toRemove = new Queue<User>();

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
                updateVision();
                sendDataToClients();
                //checkForDissconnects();

            }
        }

        private void checkForDissconnects()
        {
            foreach (User u in _users)
            {
                if (u.TimedOut)
                    _toRemove.Enqueue(u);
            }
        }

        private void handleRemovedClients()
        {
            User user;
            while (_toRemove.Count > 0)
            {
                user = _toRemove.Dequeue();
                if (user.id != -1)
                {
                    _game.Players.removePlayer(user.id);
                    _outGoing.Enqueue(new OutGoing(_users, Transmition.Logout.Create(user.id)));
                    _users.Remove(user);
                }
                else
                {
                    _users.Remove(user);
                }
            }
        }

        private void handleClientData()
        {
            KeyValuePair<IPEndPoint, byte[]> m;
            while (_server.MessageQueue.Count > 0)
            {
                m = _server.MessageQueue.Dequeue();
                IList<string> errorMessage;
                JObject com = Transmition.Parse(m.Value, out errorMessage);
                if (com != null)
                {
                    // add new users
                    if (!_users.Exists(u => u.location.Equals(m.Key)))
                    {
                        _users.Add(new User(-1, m.Key, Helper.getTimestamp()));
                    }
                    _inComing.Enqueue(new KeyValuePair<IPEndPoint, JObject>(m.Key, com));
                }
                else
                {
                    JObject error = Transmition.Error.Create("TransmisionError", 605, (List<string>)errorMessage);
                    _outGoing.Enqueue(new OutGoing(m.Key, error));
                }

            }
        }

        private void updateClientRequests()
        {
            KeyValuePair<IPEndPoint, JObject> m;
            while (_inComing.Count > 0)
            {
                m = _inComing.Dequeue();
                int userIndex = _users.FindIndex(u => u.location.Equals(m.Key));
                JObject com = m.Value;

                User sender = _users[userIndex];
                string comType = com[Transmition.Base.TYPE].Value<string>();

                if (comType != Transmition.TransmitionTypes.LOGIN)
                {
                    sender.lastMessage = Helper.getTimestamp();
                }

                if (sender.id == -1 &&
                    comType != Transmition.TransmitionTypes.LOGIN &&
                    comType != Transmition.TransmitionTypes.LOGOUT &&
                    comType != Transmition.TransmitionTypes.PING)
                {

                    string message = string.Format("{0} | sent the command: {1} without first logging in", sender.location, comType);
                    JObject errorMessage = Transmition.Error.Create("LoginError", 1, message);
                    _outGoing.Enqueue(new OutGoing(sender.location, errorMessage));
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
                }
            }
        }

        private void login(User sender, JObject com)
        {
            int id;
            string username = com[Transmition.Login.USERNAME].Value<string>();


            // if the player is currently send an error and stop the login process
            if (sender.id != -1)
            {
                string name = _game.Players.getPlayerById(sender.id).Name;

                JObject error = Transmition.Error.Create("CurrentlyActive", 345, string.Format("This user is currently active as: {0}", name));
                _outGoing.Enqueue(new OutGoing(sender.location, error));
                return;
            }

            if ((id = _game.Players.addPlayer(username)) != -1)
            {
                // update user
                int sendersUserIndex = _users.FindIndex(u => u.location == sender.location);
                sender.id = id;
                _users[sendersUserIndex] = sender;

                // let the user know they are logged in
                com[Transmition.Base.ID] = id;
                _outGoing.Enqueue(new OutGoing(sender.location, com));

                // send to everyone accept the user logging in
                List<User> subUsers = _users.FindAll(u => u.id != id);
                if (subUsers.Count > 0)
                {
                    JObject response = Transmition.Login.Create(id, _game.Players.getPlayerById(id).Name);
                    _outGoing.Enqueue(new OutGoing(subUsers, com));
                }
            }
            else
            {
                JObject error = Transmition.Error.Create("NameInUse", 344, string.Format("The name '{0}' is currently in use. Please use a different name to login", username));
                _outGoing.Enqueue(new OutGoing(sender.location, error));
            }
        }

        private void logout(User sender)
        {
            _toRemove.Enqueue(sender);
        }

        private void ping(User sender)
        {
            if (sender.id != -1)
            {
                JObject ping = Transmition.Ping.Create(sender.id);
                _outGoing.Enqueue(new OutGoing(sender.location, ping));
            }
        }

        private void move(User sender, JObject com)
        {
            int direction = com[Transmition.Move.DIRECTION].Value<int>();
            bool isComplete = com[Transmition.Move.COMPLETE].Value<bool>();

            _game.Players.getPlayerById(sender.id).setMove(direction, isComplete);

            Player p = _game.Players.getPlayerById(sender.id);
            _outGoing.Enqueue(new OutGoing(_users, Transmition.Move.Create(sender.id, direction, isComplete, p.X, p.Y)));
        }

        private void place_turret(User sender, JObject com)
        {
            int x = com[Transmition.PlaceTurret.X].Value<int>();
            int y = com[Transmition.PlaceTurret.Y].Value<int>();

            string errorMessage;
            if (_game.placeBunker(sender.id, x, y, out errorMessage))
            {
                com[Transmition.Base.ID] = sender.id;
                _outGoing.Enqueue(new OutGoing(_users, com));
            }
            else
            {
                _outGoing.Enqueue(new OutGoing(sender.location, Transmition.Error.Create("PlaceTurretException", 2435, errorMessage)));
            }
        }

        private void updateVision()
        {
            Dictionary<int, JObject> transmisions = new Dictionary<int, JObject>();

            List<Player> players;
            foreach (Data.Action a in _game.Actions)
            {
                JObject jGameObject = Transmition.MapData.GameObject.Create(
                    a.Issuer.TypeID,
                    a.Issuer.CreationID,
                    a.Issuer.Name,
                    a.Issuer.X,
                    a.Issuer.Y,
                    a.Issuer.Width,
                    a.Issuer.Height,
                    a.Issuer.ActionRadious,
                    a.Issuer.Moves.ToArray()
                    );

                players = a.getEffectedPlayerIDs(_game);
                foreach (Player p in players)
                {
                    if (a.Type == Data.Action.ActionType.ENTER_PLAYER_VISION &&
                        a.Issuer.Observers.Find(o => o.CreationID == p.CreationID) != null)
                    {
                        continue;
                    }

                    if (!transmisions.ContainsKey(p.PlayerID))
                    {
                        transmisions.Add(p.PlayerID, Transmition.MapData.Create(p.PlayerID));
                    }

                    if (a.Issuer.TypeID == 1)
                    {
                        ((JArray)transmisions[p.PlayerID][Transmition.MapData.PLAYERS]).Add(jGameObject);
                    }
                    else if (a.Issuer.TypeID == 100)
                    {
                        ((JArray)transmisions[p.PlayerID][Transmition.MapData.TURRETS]).Add(jGameObject);
                    }
                    else
                    {
                        ((JArray)transmisions[p.PlayerID][Transmition.MapData.ENEMIES]).Add(jGameObject);
                    }
                }
                // updates the issuer so it doesn't keep spaming new unit found!
                a.Issuer.updateObservers(players);
            }

            foreach (KeyValuePair<int, JObject> pair in transmisions)
            {
                _outGoing.Enqueue(new OutGoing(_users.Find(u => u.id == pair.Key).location, pair.Value));
            }

            _game.Actions.Clear();
        }

        private void sendDataToClients()
        {
            OutGoing message;
            while (_outGoing.Count > 0)
            {
                message = _outGoing.Dequeue();
                byte[] data = Transmition.Serialize(message.data);
                _server.broadcast(message.clients, data);
                Logger.Log(Level.NORMAL, Transmition.GetDisplayString(message.data), message.data.ToString(Newtonsoft.Json.Formatting.None));
            }
        }

        static void Main(string[] args)
        {
            GameServer g = new GameServer();
        }
    }
}
