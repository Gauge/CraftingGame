using System;
using System.Collections.Generic;
using System.Net;
using GameServer.Networking;

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

			public User(int id, IPEndPoint location) {
				this.location = location;
				this.id = id;
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
				string formattedName = "";
				if (user.id != -1) {
					formattedName = "| " + _game.getPlayerById(user.id).username;
					_game.removePlayer(user.id);
					_outGoing.Add(new OutGoing(_users, new Logout(user.id)));
				}
				Console.WriteLine("DISSCONECTED\t {0} {1}", user.location, formattedName);
				_users.Remove(user);
				_server.removeClientByEndPoint(user.location);
			}
			_toRemove.Clear();

			IPEndPoint[] dcClients = _server.DisconnectedClients;
			foreach (IPEndPoint loc in dcClients) {
				User dcUser = _users.Find(u => u.location.Equals(loc));
				if (dcUser != null) {
					Console.WriteLine("DISSCONECTED\t {0} | {1}", dcUser.location, _game.getPlayerById(dcUser.id).username);
					_game.removePlayer(dcUser.id);
					_users.Remove(dcUser);
					_outGoing.Add(new OutGoing(dcUser.location, new Logout(dcUser.id)));

				}
			}
		}

		private void handleClientData() {
			List<Message> messages = _server.PendingMessages;

			for (int i = 0; i < messages.Count; i++) {
				Message m = messages[i];
				try {
					BaseCommand com = BaseCommand.fromByteArray(m.data);
					// add new users
					User temp = new User(-1, m.sender);
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
				User sender = _users[userIndex];
				BaseCommand com = m.parsed;

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
				_outGoing.Add(new OutGoing(sender.location, new LoadGame(sender.id, _game.Players)));

				// send to everyone accept the user logging in
				List<User> subUsers = _users.FindAll(u => u.id != id);
				if (subUsers.Count > 0) {
					Login response = new Login(id, com.username, _game.getPlayerById(id));
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
				//	Console.WriteLine(message.ToString());
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
