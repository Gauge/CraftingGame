using GameServer.Data.Interactables;
using GameServer.Data.Interactables.Bunkers;
using GameServer.Data.Interactables.Enemies;
using GameServer.Data.Interactables.Missiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer.Data
{
    public static class Transmition
    {
        private static Dictionary<string, JSchema> SchemaLookup = new Dictionary<string, JSchema>();

        internal static class TransmitionTypes
        {
            public const string LOGIN = "login";
            public const string LOGOUT = "logout";
            public const string PING = "ping";
            public const string ERROR = "error";
            public const string MOVE = "move";
            public const string PLACE_BUNKER = "place_bunker";
            public const string MAP_DATA = "map_data";

            public static bool isTransmitionType(string type)
            {
                return
                    (
                    type.Equals(LOGIN) ||
                    type.Equals(LOGOUT) ||
                    type.Equals(PING) ||
                    type.Equals(ERROR) ||
                    type.Equals(MOVE) ||
                    type.Equals(PLACE_BUNKER) ||
                    type.Equals(MAP_DATA)
                    );
            }
        }

        internal static class GameObject
        {
            public const string TYPE_ID = "type_id";
            public const string CREATION_ID = "creation_id";
            public const string NAME = "name";
            public const string X = "x";
            public const string Y = "y";
            public const string WIDTH = "width";
            public const string HEIGHT = "height";
            public const string ACTION_RADIOUS = "action_radious";


            public static JObject Create(Interactables.GameObject gObj)
            {
                JObject jGameObject = new JObject();
                jGameObject.Add(TYPE_ID, gObj.TypeID);
                jGameObject.Add(CREATION_ID, gObj.CreationID);
                jGameObject.Add(NAME, gObj.Name);
                jGameObject.Add(X, gObj.X);
                jGameObject.Add(Y, gObj.Y);
                jGameObject.Add(WIDTH, gObj.Width);
                jGameObject.Add(HEIGHT, gObj.Height);
                jGameObject.Add(ACTION_RADIOUS, gObj.ActionRadious);

                return jGameObject;
            }
        }

        internal static class Pawn
        {
            public const string MOVES = "moves";
            public const string STATS = "stats";

            public const string HEALTH = "health";
            public const string DAMAGE = "damage";
            public const string MOVE_SPEED = "move_speed";

            public static JObject Create(Interactables.Pawn p)
            {
                JObject obj = GameObject.Create(p);
                obj.Add(MOVES, new JArray(p.Moves));

                JObject obj2 = new JObject();
                obj2.Add(HEALTH, p.Stats.Health);
                obj2.Add(DAMAGE, p.Stats.Damage);
                obj2.Add(MOVE_SPEED, p.Stats.MoveSpeed);

                obj.Add(STATS, obj2);

                return obj;

            }

            public static JObject Create(Interactables.Missiles.Missile p)
            {
                JObject obj = GameObject.Create(p);

                JObject obj2 = new JObject();
                obj2.Add(HEALTH, p.Stats.Health);
                obj2.Add(DAMAGE, p.Stats.Damage);
                obj2.Add(MOVE_SPEED, p.Stats.MoveSpeed);

                obj.Add(STATS, obj2);

                return obj;

            }
        }

        internal static class Base
        {
            public const string ID = "id";
            public const string TYPE = "type";
            public const string TIME_STAMP = "timestamp";

            public static JObject Create(int id, string type)
            {
                JObject obj = new JObject();
                obj.Add(ID, id);
                obj.Add(TYPE, type);
                obj.Add(TIME_STAMP, Helper.getTimestamp());

                return obj;
            }
        }

        internal static class Error
        {
            public const string NAME = "name";
            public const string CODE = "code";
            public const string MESSAGES = "messages";

            public static JObject Create(string name, int code, string message)
            {
                JObject obj = Base.Create(-1, TransmitionTypes.ERROR);
                obj.Add(NAME, name);
                obj.Add(CODE, code);
                obj.Add(MESSAGES, new JArray(new string[] { message }));
                return obj;
            }

            public static JObject Create(string name, int code, List<string> messages)
            {
                JObject obj = Base.Create(-1, TransmitionTypes.ERROR);
                obj.Add(NAME, name);
                obj.Add(CODE, code);
                obj.Add(MESSAGES, new JArray(messages.ToArray()));
                return obj;
            }
        }

        internal static class Login
        {
            public const string USERNAME = "username";
            public static JObject Create(int id, string username)
            {
                JObject obj = Base.Create(id, TransmitionTypes.LOGIN);
                obj.Add(USERNAME, username);

                return obj;
            }
        }

        internal static class Logout
        {
            public static JObject Create(int id)
            {
                return Base.Create(id, TransmitionTypes.LOGOUT);
            }
        }

        internal static class Ping
        {
            public static JObject Create(int id)
            {
                return Base.Create(id, TransmitionTypes.PING);
            }
        }

        internal static class Move
        {
            public static string DIRECTION = "direction";
            public static string COMPLETE = "complete";
            public static string X = "x";
            public static string Y = "y";

            public static JObject Create(int id, int direction, bool complete, double x, double y)
            {
                JObject obj = Base.Create(id, TransmitionTypes.MOVE);
                obj.Add(DIRECTION, direction);
                obj.Add(COMPLETE, complete);
                obj.Add(X, x);
                obj.Add(Y, y);

                return obj;
            }
        }

        internal static class PlaceBunker
        {
            public static string X = "x";
            public static string Y = "y";

            public static JObject Create(int id, int x, int y)
            {
                JObject obj = Base.Create(id, TransmitionTypes.PLACE_BUNKER);
                obj.Add(X, x);
                obj.Add(Y, y);

                return obj;
            }
        }

        internal static class MapData
        {
            public const string PLAYERS = "players";
            public const string BUNKERS = "bunkers";
            public const string ENEMIES = "enemies";
            public const string MISSILES = "missiles";

            public static JObject Create(int id, Interactables.Pawn[] players, Interactables.Pawn[] bunkers, Interactables.Pawn[] enemies, Missile[] missiles)
            {
                JObject obj = Base.Create(id, TransmitionTypes.MAP_DATA);

                JArray jPlayers = new JArray();
                foreach (Player p in players)
                {
                    jPlayers.Add(Pawn.Create(p));
                }

                JArray jBunker = new JArray();
                foreach (Bunker b in bunkers)
                {
                    jBunker.Add(Pawn.Create(b));
                }

                JArray jEnemies = new JArray();
                foreach (Enemy e in enemies)
                {
                    jEnemies.Add(Pawn.Create(e));
                }

                JArray jMissiles = new JArray();
                foreach (Missile m in missiles)
                {
                    jEnemies.Add(Pawn.Create(m));
                }

                obj.Add(PLAYERS, jPlayers);
                obj.Add(BUNKERS, jBunker);
                obj.Add(ENEMIES, jEnemies);
                obj.Add(MISSILES, jMissiles);
                return obj;
            }
        }



        public static JObject Parse(byte[] data, out IList<string> errorMessage)
        {
            try
            {
                JObject jdata = JObject.Parse(Encoding.ASCII.GetString(data));
                if (Validate(jdata, out errorMessage))
                {
                    return jdata;
                }
            }
            catch(Exception e)
            {
                errorMessage = new List<string>();
                errorMessage.Add("Failed to parse client data");
                Logger.Log(Level.NORMAL, e.ToString());
            }
            return null;
        }

        public static byte[] Serialize(JObject obj)
        {
            return Encoding.ASCII.GetBytes(string.Join("", obj.ToString(Formatting.None)));
        }

        public static bool Validate(JObject obj, out IList<string> errorMessages)
        {
            if (obj["type"] != null && obj["type"].Type == JTokenType.String && TransmitionTypes.isTransmitionType((string)obj["type"]))
            {
                string type = (string)obj["type"];
                if (!SchemaLookup.ContainsKey(type))
                {
                    SchemaLookup.Add(type, JSchema.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + @"\Data\TransmitionData\" + (string)obj["type"] + ".json")));
                }

                return obj.IsValid(SchemaLookup[type], out errorMessages);
            }
            else
            {
                errorMessages = new List<string>();
                errorMessages.Add("'type' element could not be found, parsed, or recognised");
                return false;
            }
        }

        public static string GetDisplayString(JObject obj)
        {
            string baseString = string.Format("ID: {0}\t{1}", obj[Base.ID], ((string)obj[Base.TYPE]).ToUpper());
            string output;
            switch ((string)obj[Base.TYPE])
            {
                case TransmitionTypes.LOGIN:
                    output = string.Format("{0}\tName: {1}", baseString, obj[Login.USERNAME]);
                    break;

                case TransmitionTypes.LOGOUT:
                    output = baseString;
                    break;

                case TransmitionTypes.PING:
                    output = string.Format("{0}\tt-: {1}", baseString, obj[Base.TIME_STAMP]);
                    break;

                case TransmitionTypes.MOVE:
                    string direction = "";
                    if ((int)obj[Move.DIRECTION] == 0)
                    {
                        direction = "UP";
                    }
                    if ((int)obj[Move.DIRECTION] == 1)
                    {
                        direction = "DOWN";
                    }
                    if ((int)obj[Move.DIRECTION] == 2)
                    {
                        direction = "LEFT";
                    }
                    if ((int)obj[Move.DIRECTION] == 3)
                    {
                        direction = "RIGHT";
                    }

                    output = string.Format("{0}\t{3}\t{4}\tLocation: {1} : {2}", baseString, obj[Move.X], obj[Move.Y], direction, (bool)obj[Move.COMPLETE] ? "FINISH" : "START");
                    break;

                case TransmitionTypes.ERROR:
                    output = string.Format("{0}\t{1}\t{2}\n{3}", baseString, obj[Error.CODE], obj[Error.NAME], obj[Error.MESSAGES]);
                    break;

                case TransmitionTypes.MAP_DATA:
                    output = string.Format("{0}\tPlayers: {1}\tBunkers: {2}\tEnemies: {3}", baseString, ((JArray)obj[MapData.PLAYERS]).Count, ((JArray)obj[MapData.BUNKERS]).Count, ((JArray)obj[MapData.ENEMIES]).Count);
                    break;

                case TransmitionTypes.PLACE_BUNKER:
                    output = string.Format("{0}\tX|Y: {1} | {2}", baseString, obj[Move.X], obj[Move.Y]);
                    break;

                default:
                    output = obj.ToString(Formatting.None);
                    break;
            }
            return output;
        }

    }
}
