using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace GameServer {

    public enum ComType { Login, Logout, LoadGame, Chat, Move, Ping};
    public enum Direction { Up, Down, Left, Right };
    public enum ChatType { System, Global, Wisper };

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
            
            this.timestamp = (timestamp == 0) ? timestamp : DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public override string ToString() {
            return type.ToString() + " ID:" + id;
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
            if (comName == "") return null;

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
            //this.id = id;
            this.player = player;
        }

        public Login(int id, string username, Player player) : base(ComType.Login, id) {
            this.player = player;
        }

        public override string ToString()
        {
            return base.ToString() + " name: " + username + " player: " + player;
        }
    }

    public class Logout : BaseCommand {
        [JsonConstructor()]
        public Logout() : base(ComType.Logout, -1) {}
    }

    public class LoadGame : BaseCommand {
        public List<Player> players { get; set; }

        [JsonConstructor()]
        public LoadGame(int id, List<Player> players) : base(ComType.LoadGame, id){
            this.players = players;
        }

    }

    public class Chat : BaseCommand {
        public ChatType chatType { get; set; }
        public string sender { get; set; }
        public string recipient { get; set; }
        public string message { get; set; }

        [JsonConstructor]
        private Chat(int id = -1, ChatType chatType = ChatType.System, string message = null, string sender = null, string recipient = null): base(ComType.Chat, id) {
            this.chatType = chatType;
            this.message = message;
            this.sender = sender;
            this.recipient = recipient;
        }

        public Chat(string message) : base(ComType.Chat, -1) {
            chatType = ChatType.System;
            this.message = message;
        }

        public Chat(int id, string sender, string message) : base (ComType.Chat, id) {
            chatType = ChatType.Global;
            this.sender = sender;
            this.message = message;

            string[] list = message.Split(' ');
            if (list.Length > 0) {
                if (list[0] == "/w" || list[0] == "/wisper") {
                    string m = "";
                    for (int i = 2; i < list.Length; i++) {
                        m += list[i] + " ";
                    }
                    chatType = ChatType.Wisper;
                    if (list.Length > 1) recipient = list[1];
                    this.message = m;
                }
            }
        }
    }

    public class Move : BaseCommand {
        public Direction direction;
        public bool isComplete;
        public double x;
        public double y;

        [JsonConstructor]
        public Move(int id, Direction direction, bool isComplete) : base(ComType.Move, id) {
            this.direction = direction;
            this.isComplete = isComplete;
        }

        public Move(int id, Direction direction, bool isComplete, double x, double y) : base(ComType.Move, id) {
            this.direction = direction;
            this.isComplete = isComplete;
            this.x = x;
            this.y = y;
        }
    }

    public class Ping : BaseCommand {
        public Ping(int id, long timestamp = 0) : base(ComType.Ping, id, timestamp) { }
    }

    /*public class Chat {
        public ChatType type;
        public string recipient; // username
        public string message;

        public Chat(string text) {
            type = ChatType.Global;
            recipient = "";
            message = text;

            string[] list = text.Split(' ');
            if (list.Length > 0) {
                if (list[0] == "/w" || list[0] == "/wisper") {
                    string m = "";
                    for (int i = 2; i < list.Length; i++) {
                        m += list[i];
                    }
                    type = ChatType.Wisper;
                    recipient = list[1];
                    message = m;
                }
            }
        }

        public override string ToString() {

            string data = type.ToString();
            switch (type) {
                case ChatType.Wisper:
                    data += " Recipient: " + recipient;
                    break;
            }
            data += " Message: " + message;
            return data;
        }
    }

    public struct Move {
        public int playerID;
        public Direction direction;
        public bool isComplete;

        public Move(int id, Direction d, bool isComplete) {
            playerID = id;
            direction = d;
            this.isComplete = isComplete;
        }

        public override string ToString() {
            return "ID: " + playerID + " DIR: " + direction.ToString() + " Done: " + isComplete;
        }
    }

    public class Communication {

        public Communication() {
            timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        // required fields
        public ComType type;
        public string username;
        public long timestamp;

        // server loading data
        public int playerID; // login
        public Player newPlayer; // login
        public List<Player> players; // Load Game
        public Chat chat;

        // game functions
        public Move moveAction;
        public double x;
        public double y;

        public override string ToString() {
            string data = "Type: " + type.ToString() + " ";

            switch (type) {

                case ComType.LOGIN:
                    data += "ID: " + playerID + " User: " + username + " Player: " + ((newPlayer != null) ? newPlayer.ToString() : "N/A");
                    break;

                case ComType.LOAD_GAME:
                    data += "ID: " + playerID + " Player Count: " + players.Count;
                    foreach (Player p in players) {
                        data += "\n" + p.ToString();
                    }
                    break;

                case ComType.LOGOUT:
                    data += "ID: " + playerID;
                    break;

                case ComType.MOVE:
                    data += "Location: " + x + ":" + y + " User: " + username + " " + moveAction.ToString();
                    break;

                case ComType.CHAT:
                    data += "User: " + username + " " + chat.ToString();
                    break;
            }

            return data;
        }



        public static Communication login(string username) {
            Communication c = new Communication();
            c.type = ComType.LOGIN;
            c.username = username;
            return c;
        }

        public static Communication login(string username, int id, Player player) {
            Communication c = new Communication();
            c.type = ComType.LOGIN;
            c.username = username;
            c.playerID = id;
            c.newPlayer = player;
            return c;
        }

        public static Communication loadGame(int id, List<Player> players) {
            Communication c = new Communication();
            c.type = ComType.LOAD_GAME;
            c.playerID = id;
            c.players = players;
            return c;
        }

        public static Communication logout() {
            Communication c = new Communication();
            c.type = ComType.LOGOUT;
            return c;
        }

        public static Communication logout(int id) {
            Communication c = new Communication();
            c.type = ComType.LOGOUT;
            c.playerID = id;
            return c;
        }

        public static Communication move(string username, Move m) {
            Communication c = new Communication();
            c.type = ComType.MOVE;
            c.username = username;
            c.moveAction = m;
            return c;
        }

        public static Communication move(string username, Move m, int x, int y) {
            Communication c = new Communication();
            c.type = ComType.MOVE;
            c.username = username;
            c.moveAction = m;
            c.x = x;
            c.y = y;
            return c;
        }

        public static Communication _chat(string message) {
            Communication c = new Communication();
        }

        public static Communication ping() {
            Communication c = new Communication();
            c.type = ComType.PING;
            return c;
        }
    }*/

}
