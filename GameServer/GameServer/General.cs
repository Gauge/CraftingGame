using System;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameServer {

    public enum ComType { LOGIN, LOGOUT, LOAD_GAME, CHAT, MOVE, PING};
    public enum Direction { UP, DOWN, LEFT, RIGHT };
    public enum ChatType { System, Global, Wisper };


    public struct Chat {
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

    public struct Message {
        public byte[] data;
        public IPEndPoint sender;
        public Communication parsed;

        public Message(byte[] d, IPEndPoint s) {
            data = d;
            sender = s;
            parsed = null;
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

        public static byte[] toByteArray(Communication c) {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(c));
        }

        public static Communication fromByteArray(byte[] c) {
            try {
                return JsonConvert.DeserializeObject<Communication>(Encoding.ASCII.GetString(c));
            } catch (Exception e) {
                throw e;
            }
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

        public static Communication startMove(string username, Move m) {
            Communication c = new Communication();
            c.type = ComType.MOVE;
            c.username = username;
            c.moveAction = m;
            return c;
        }

        public static Communication endMove(string username, Move m, int x, int y) {
            Communication c = new Communication();
            c.type = ComType.MOVE;
            c.username = username;
            c.moveAction = m;
            c.x = x;
            c.y = y;
            return c;
        }

        public static Communication ping() {
            Communication c = new Communication();
            c.type = ComType.PING;
            return c;
        }
    }

    public class Player {
        public static int movementSpeed = 750; // this is temp till i get other matter settled with
        public static int speedPerUnit = 100;

        public int id;
        public string username;
        public double x;
        public double y;

        public Player(string u) {
            id = idGenerator();
            username = u;
            x = 0;
            y = 0;
        }

        public override string ToString() {
            return "ID: " + id + " Username: " + username + " Location: " + x + ":" + y;
        }

        private static int idGeneration = 0;
        private static int idGenerator()
        {
            return ++idGeneration;
        }
    }

}
