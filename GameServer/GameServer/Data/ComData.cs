using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace GameServer {

	public enum ComType {
		Login, Logout, LoadGame, Chat, Move, Ping
	};
	public enum Direction {
		Up, Down, Left, Right
	};
	public enum ChatType {
		System, Global, Whisper
	};

	public abstract class BaseCommand {
		public ComType type { get; }
		public int id { get; set; }
		public long timestamp { get; }

		public BaseCommand(ComType type, int id) {
			this.type = type;
			this.id = id;
			timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public BaseCommand(ComType type, int id, long timestamp) {
			this.type = type;
			this.id = id;

			this.timestamp = (timestamp != 0) ? timestamp : DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public override string ToString() {
			return "ID:" + id + " " + type.ToString();
		}

		public byte[] toByteArray() {
			Type t = Type.GetType("GameServer." + (type).ToString());
			return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this, t, new JsonSerializerSettings()));
		}

		public static BaseCommand fromByteArray(byte[] c) {
			string data = Encoding.ASCII.GetString(c);
			string[] list = data.Split(new char[] { ',', ':', '{', '}' });
			string comName = "";
			for (int i = 0; i < list.Length; i++) {
				if (list[i] == "\"type\"") {
					comName = ((ComType)int.Parse(list[i + 1])).ToString();
					break;
				}
			}
			if (comName == "")
				return null;

			Type t = Type.GetType("GameServer." + comName);
			MethodInfo[] methods = typeof(JsonConvert).GetMethods(BindingFlags.Public | BindingFlags.Static);
			// drat... one short of 42. the magic number 41 was found with the commented code below
			return (BaseCommand)methods[41].MakeGenericMethod(t).Invoke(null, new object[] { data });

			/*for (int i=0; i<methods.Length; i++) {
                if (methods[i].Name == "DeserializeObject" && methods[i].IsGenericMethod) {
                    return (BaseCommand)methods[i].MakeGenericMethod(t).Invoke(null, new object[] { data });
                }
            }
            return null;*/
		}
	}

	public class Login : BaseCommand {
		public string username { get; set; }
		public Player player { get; set; }

		// client to server
		[JsonConstructor()]
		public Login(string username, int id = -1, Player player = null) : base(ComType.Login, id) {
			this.username = username;
			this.player = player;
		}

		public Login(int id, string username, Player player) : base(ComType.Login, id) {
			this.player = player;
		}

		public override string ToString() {
			return base.ToString() + "\t Name: " + username + ((player != null) ? "\t player: " + player : "");
		}
	}

	public class Logout : BaseCommand {
		[JsonConstructor()]
		public Logout(int id = -1) : base(ComType.Logout, id) { }
	}

	public class LoadGame : BaseCommand {
		public List<Player> players { get; set; }

		[JsonConstructor()]
		public LoadGame(int id, List<Player> players) : base(ComType.LoadGame, id) {
			this.players = players;
		}

		public override string ToString() {
			return base.ToString() + " Player Count: " + players.Count + " " + string.Join<Player>("\n", players.ToArray());
		}

	}

	public class Chat : BaseCommand {
		public ChatType chatType { get; set; }
		public string sender { get; set; }
		public string recipient { get; set; }
		public string message { get; set; }

		[JsonConstructor]
		private Chat(int id = -1, ChatType chatType = ChatType.System, string message = null, string sender = null, string recipient = null) : base(ComType.Chat, id) {
			this.chatType = chatType;
			this.message = message;
			this.sender = sender;
			this.recipient = recipient;
		}

		public Chat(string message) : base(ComType.Chat, -1) {
			chatType = ChatType.System;
			this.message = message;
		}

		public Chat(int id, string sender, string message) : base(ComType.Chat, id) {
			chatType = ChatType.Global;
			this.sender = sender;
			this.message = message;

			string[] list = message.Split(' ');
			if (list.Length > 0) {
				if (list[0] == "/w" || list[0] == "/whisper") {
					string m = "";
					for (int i = 2; i < list.Length; i++) {
						m += list[i] + " ";
					}
					chatType = ChatType.Whisper;
					if (list.Length > 1)
						recipient = list[1];
					this.message = m;
				}
			}
		}

		public override string ToString() {
			return base.ToString() + "\t " + chatType.ToString() + ((sender != null) ? "\t From: " + sender : "") + ((recipient != null) ? "\t To: " + recipient: "") + "\t Msg: " + message;
		}
	}

	public class Move : BaseCommand {
		public Direction direction;
		public bool isComplete;
		public double x;
		public double y;

		[JsonConstructor]
		public Move(int id, Direction direction, bool isComplete, double x = 0, double y = 0) : base(ComType.Move, id) {
			this.direction = direction;
			this.isComplete = isComplete;
			this.x = x;
			this.y = y;
		}

		public override string ToString() {
			return base.ToString() + "\t " + direction.ToString() + "\t " + x.ToString("0.00") + ":" + y.ToString("0.00") + "\t " + ((isComplete)? "START" : "END");
			;
		}
	}

	public class Ping : BaseCommand {
		public Ping(int id, long timestamp = 0) : base(ComType.Ping, id, timestamp) { }

		public override string ToString() {
			return base.ToString() + " " + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - timestamp);
		}
	}
}
