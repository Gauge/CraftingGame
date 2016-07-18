using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace GameServer {
    public class GameServer {

        private class OutGoing {
            public IPEndPoint[] clients;
            public Communication data;

            public OutGoing(IPEndPoint[] recipients, Communication com) {
                clients = recipients;
                data = com;
            }

            public OutGoing(IPEndPoint recipient, Communication com) {
                clients = new IPEndPoint[] { recipient };
                data = com;
            }

            public OutGoing(List<User> recipients, Communication com) {
                clients = User.getEndPointArray(recipients);
                data = com;
            }

            public override string ToString() {
                return "Message Type: " + data.type.ToString() + " to " + clients.Length + " Clients"; 
            }
        }

        private class User {
            public string name;
            public IPEndPoint location;
            public int id;

            public User(string n, IPEndPoint l) {
                name = n;
                location = l;
                id = -1;
            }

            public static IPEndPoint[] getEndPointArray(List<User> users) {
                IPEndPoint[] endPoints = new IPEndPoint[users.Count];
                for (int i = 0; i < users.Count; i++) {
                    endPoints[i] = users[i].location;
                }
                return endPoints;
            }
        }

        private List<User> _users;
        private Server _server;
        private Game _game;
        private List<Message> _inComing;
        private List<OutGoing> _outGoing;
        private List<User> _toRemove;


        public GameServer() {
            _users = new List<User>();
            _server = new Server(1234);
            _server.Start();
            _game = new Game();
            _inComing = new List<Message>();
            _outGoing = new List<OutGoing>();
            _toRemove = new List<User>();
            mainLoop();
        }

        private void mainLoop() {
            Console.WriteLine("Starting game loop...");
            while (true) {
                handleRemovedClients();
                handleClientData();
                updateClientRequests();
                _game.update();
                sendDataToClients();

            }
        }

        private void handleRemovedClients() {

            foreach (User user in _toRemove) {
                if (user.id != -1) {
                    Console.WriteLine("User: {0} has disconnected", _game.getPlayerById(user.id).username);
                    _game.removePlayer(user.id);
                }
                _users.Remove(user);
                _server.removeClientByEndPoint(user.location);

                _outGoing.Add(new OutGoing(_users, Communication.logout(user.id)));
            }
            _toRemove.Clear();

            IPEndPoint[] dcClients = _server.DisconnectedClients;
            foreach (IPEndPoint loc in dcClients) {
                User dcUser = _users.Find(u => u.location.Equals(loc));
                if (dcUser != null) {
                    string tempName = _game.getPlayerById(dcUser.id).username;
                    _game.removePlayer(dcUser.id);
                    _users.Remove(dcUser);

                    Console.WriteLine("User: {0} has disconnected", tempName);
                    _outGoing.Add(new OutGoing(_users, Communication.logout(dcUser.id)));

                }
            }
        }

        private void handleClientData() {
            List<Message> messages = _server.PendingMessages;

            for (int i = 0; i < messages.Count; i++) {
                Message m = messages[i];
                try {
                    Communication com = Communication.fromByteArray(m.data);
                    if (com.type != ComType.PING) Console.WriteLine(com.ToString());

                    // add new users
                    User temp = new User(com.username, m.sender);
                    Predicate < User > match = u => u.location.Equals(temp.location);
                    if (!_users.Exists(match)) {
                        Console.WriteLine("User {0} has joined the server", temp.name);
                        _users.Add(temp);
                    }

                    m.parsed = com;
                    _inComing.Add(m);

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    continue;
                }
            }
        }

        private void updateClientRequests() {

            foreach (Message m in _inComing) {
                int userIndex = _users.FindIndex(u => u.location.Equals(m.sender));
                User sender = _users[userIndex];
                Communication com = m.parsed;

                if (sender.id == -1 && com.type != ComType.LOGIN) {
                    Console.WriteLine("{0} | {1} has sent the command: {2} without first logging in", m.sender, com.username, com.type.ToString());
                    _toRemove.Add(sender);
                    _outGoing.Add(new OutGoing(m.sender, Communication.logout(sender.id)));
                    continue;
                }

                switch (com.type) {

                    case ComType.LOGIN:
                        sender.id = _game.addPlayer(com.username);
                        _users[userIndex] = sender;

                        Communication comToSend = Communication.loadGame(sender.id, _game.Players);
                        _outGoing.Add(new OutGoing(new IPEndPoint[] { sender.location }, comToSend));

                        com.playerID = sender.id;
                        com.newPlayer = _game.getPlayerById(sender.id);

                        // send to everyone accept the user logging in
                        List<User> subUsers = _users.FindAll(u => u.GetHashCode() != sender.GetHashCode());
                        if (subUsers.Count > 0) {
                            _outGoing.Add(new OutGoing(subUsers, com));
                        }
                        break;

                    case ComType.LOGOUT:
                        _toRemove.Add(sender);
                        break;

                    case ComType.CHAT:
                        if (com.chat.type == ChatType.Global) {
                            _outGoing.Add(new OutGoing(_users, com));

                        } else if (com.chat.type == ChatType.Wisper) {
                            int id = _game.getIdByUsername(com.chat.recipient);
                            User recipient = _users.Find(u => u.id == id);
                            if (recipient == null) {
                                Chat c = new Chat("Recipient: " + com.chat.recipient + "does not exist");
                                c.type = ChatType.System;
                                com.username = "";
                                com.chat = c;
                            }

                            _outGoing.Add(new OutGoing(recipient.location, com));
                        }
                        break;

                    case ComType.MOVE:
                        _game.setPlayerMove(com.moveAction);

                        Player p = _game.getPlayerById(sender.id);
                        com.x = p.x;
                        com.y = p.y;
                        _outGoing.Add(new OutGoing(_users, com));
                        break;

                    case ComType.PING:
                        _outGoing.Add(new OutGoing(sender.location, com));
                        break;
                }
            }
            _inComing.Clear();
        }
        
        private void sendDataToClients() {
            foreach (OutGoing message in _outGoing) {
                byte[] data = Communication.toByteArray(message.data);
                _server.broadcast(message.clients, data);
            }
            _outGoing.Clear();
        }    

        static void Main(string[] args) {
            GameServer g = new GameServer();
        }
    }
}
