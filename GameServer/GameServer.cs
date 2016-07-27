using System;
using System.Collections.Generic;
using System.Net;
using GameServer.Networking;
using Newtonsoft.Json;
using System.Text;

namespace GameServer {
	public class GameServer {

		private class OutGoing {
			public IPEndPoint[] clients;
			public BaseCommand data;

			public OutGoing(IPEndPoint[] recipients, BaseCommand com) {
				clients = recipients;
				data = com;
			}

			public OutGoing(IPEndPoint recipient, BaseCommand com) {
				clients = new IPEndPoint[] { recipient };
				data = com;
			}

			public OutGoing(List<User> recipients, BaseCommand com) {
				clients = User.getEndPointArray(recipients);
				data = com;
			}

			public override string ToString() {
				return data.ToString() + " to " + clients.Length + " Clients";
			}
		}

		private class User {
			public string name;
			public IPEndPoint location;
			public int id;
			public long lastMessage;

			public bool TimedOut {
				get {
					return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lastMessage >= CLIENT_TIMEOUT;
				}
			}

			public User(int id, IPEndPoint location, long lastMessage) {
				this.location = location;
				this.id = id;
				this.lastMessage = lastMessage;
			}

			public static IPEndPoint[] getEndPointArray(List<User> users) {
				IPEndPoint[] endPoints = new IPEndPoint[users.Count];
				for (int i = 0; i < users.Count; i++) {
					endPoints[i] = users[i].location;
				}
				return endPoints;
			}
		}

		public const long CLIENT_TIMEOUT = 120000;

		private List<User> _users;
		private Server _server;
		private Game _game;
		private List<Message> _inComing;
		private List<OutGoing> _outGoing;
		private List<User> _toRemove;


		public GameServer() {
			_users = new List<User>();
			_server = new Server(1234);
			_game = new Game();
			_inComing = new List<Message>();
			_outGoing = new List<OutGoing>();
			_toRemove = new List<User>();

			_server.Start();
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
				checkForDissconnects();

			}
		}

		private void checkForDissconnects() {
			foreach (User u in _users) {
				if (u.TimedOut)
					_toRemove.Add(u);
			}
		}

		private void handleRemovedClients() {

			foreach (User user in _toRemove) {
				string formattedName = "";
				if (user.id != -1) {
					formattedName = "| " + _game.getPlayerById(user.id).username;
					_game.removePlayer(user.id);
					_outGoing.Add(new OutGoing(_users, new Logout(user.id)));
				}
				Console.WriteLine("DISSCONECTED\t {0} {1}", user.location, formattedName);
				_users.Remove(user);
			}
			_toRemove.Clear();
		}

		private void handleClientData() {
			List<Message> messages = _server.PendingMessages;

			for (int i = 0; i < messages.Count; i++) {
				Message m = messages[i];
				try {
					BaseCommand com = BaseCommand.fromByteArray(m.data);
					// add new users
					User temp = new User(-1, m.sender, com.timestamp);
					if (!_users.Exists(u => u.location.Equals(temp.location))) {
						_users.Add(temp);
					}

					m.parsed = com;
					_inComing.Add(m);

				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
			}
		}

		private void updateClientRequests() {

			foreach (Message m in _inComing) {
				int userIndex = _users.FindIndex(u => u.location.Equals(m.sender));
				BaseCommand com = m.parsed;
				User sender = _users[userIndex];

				if (com.type != ComType.Ping)
					sender.lastMessage = com.timestamp;

				if (sender.id == -1 && (com.type != ComType.Login && com.type != ComType.Logout && com.type != ComType.Ping)) {
					Console.WriteLine("{0} | sent the command: {2} without first logging in", m.sender, com.type.ToString());
					userLogout(sender, (Logout)com);
					continue;
				}

				if (com.type != ComType.Ping && com.type != ComType.Login && com.type != ComType.Logout)
					Console.WriteLine(com.ToString());

				switch (com.type) {
					case ComType.Login:
						userLogin(sender, (Login)com);
						break;

					case ComType.Logout:
						userLogout(sender, (Logout)com);
						break;

					case ComType.Chat:
						userChat(sender, (Chat)com);

						break;

					case ComType.Move:
						userMove(sender, (Move)com);
						break;

					case ComType.Ping:
						userPing(sender, (Ping)com);
						break;
				}
			}
			_inComing.Clear();
		}

		private void userLogin(User sender, Login com) {
			int id;

			if ((id = _game.addPlayer(com.username)) != -1) {
				// update user
				int sendersUserIndex = _users.FindIndex(u => u.location == sender.location);
				sender.id = id;
				sender.name = com.username;
				_users[sendersUserIndex] = sender;
				Console.WriteLine("CONNECTED\t {0} | {1}", sender.location, com.username);

				// let the user know they are logged in
				LoadGame lg = new LoadGame(sender.id, _game.Players);
				byte[] bytes = lg.toByteArray();
				//lg = (LoadGame)BaseCommand.fromByteArray(bytes);
				lg = JsonConvert.DeserializeObject<LoadGame>(Encoding.ASCII.GetString(bytes));

				_outGoing.Add(new OutGoing(sender.location, new LoadGame(sender.id, _game.Players)));

				// send to everyone accept the user logging in
				List<User> subUsers = _users.FindAll(u => u.id != id);
				if (subUsers.Count > 0) {
					Login response = new Login(id, _game.getPlayerById(id));
					_outGoing.Add(new OutGoing(subUsers, response));
				}

			} else {
				Console.WriteLine("Username needs to be unique. Eventually there will be an error message");
			}
		}

		private void userLogout(User sender, Logout com) {
			_toRemove.Add(sender);
		}

		private void userChat(User sender, Chat com) {
			if (com.chatType == ChatType.Global) {
				_outGoing.Add(new OutGoing(_users, com));

			} else if (com.chatType == ChatType.Whisper) {
				int id = _game.getIdByUsername(com.recipient);
				User recipient = _users.Find(u => u.id == id);
				if (recipient == null) {
					Chat c = new Chat("Recipient: " + com.recipient + "does not exist");
					_outGoing.Add(new OutGoing(sender.location, c));
				} else {
					_outGoing.Add(new OutGoing(recipient.location, com));
				}
			}
		}

		private void userMove(User sender, Move com) {
			_game.setPlayerMove(com.id, com.direction, com.isComplete);

			Player p = _game.getPlayerById(sender.id);
			_outGoing.Add(new OutGoing(_users, new Move(com.id, com.direction, com.isComplete, p.x, p.y)));
		}

		private void userPing(User sender, Ping com) {
			_outGoing.Add(new OutGoing(sender.location, com));
		}

		private void sendDataToClients() {
			foreach (OutGoing message in _outGoing) {
				//if (message.data.type != ComType.Ping)
					//Console.WriteLine(message.data.ToString());
				byte[] data = message.data.toByteArray();
				_server.broadcast(message.clients, data);
			}
			_outGoing.Clear();
		}

		static void Main(string[] args) {
			GameServer g = new GameServer();
		}
	}
}
