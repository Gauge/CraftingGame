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
        private static JSchema _schema;

        internal static class TransmitionTypes
        {
            public const string LOGIN = "login";
            public const string LOGOUT = "logout";
            public const string PING = "ping";
            public const string ERROR = "error";
            public const string MOVE = "move";
            public const string PLACE_TURRET = "place_turret";
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
                    type.Equals(PLACE_TURRET) ||
                    type.Equals(MAP_DATA)
                    );
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

        internal static class PlaceTurret
        {
            public static string X = "x";
            public static string Y = "y";

            public static JObject Create(int id, int x, int y)
            {
                JObject obj = Base.Create(id, TransmitionTypes.PLACE_TURRET);
                obj.Add(X, x);
                obj.Add(Y, y);

                return obj;
            }
        }

        internal static class MapData
        {
            public const string PLAYERS = "players";
            public const string TURRETS = "turrets";
            public const string ENEMIES = "enemies";

            public static JObject Create(int id)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.MAP_DATA);
                obj.Add(Base.ID, id);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());
                obj.Add(PLAYERS, new JArray());
                obj.Add(TURRETS, new JArray());
                obj.Add(ENEMIES, new JArray());

                return obj;
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
                public const string MOVES = "moves";


                public static JObject Create(int type_id, int creation_id, string name, double x, double y, int width, int height, double action_radious, bool[] moves)
                {
                    JObject jGameObject = new JObject();
                    jGameObject.Add(TYPE_ID, type_id);
                    jGameObject.Add(CREATION_ID, creation_id);
                    jGameObject.Add(NAME, name);
                    jGameObject.Add(X, x);
                    jGameObject.Add(Y, y);
                    jGameObject.Add(WIDTH, width);
                    jGameObject.Add(HEIGHT, height);
                    jGameObject.Add(ACTION_RADIOUS, action_radious);
                    jGameObject.Add(MOVES, new JArray(moves));

                    return jGameObject;
                }
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
            catch
            {
                errorMessage = new List<string>();
                errorMessage.Add("Failed to parse client data");
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
                JSchema schema = JSchema.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + @"\Data\TransmitionData\" + (string)obj["type"] + ".json"));
                return obj.IsValid(schema, out errorMessages);
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
                    output = string.Format("{0}\tPlayers: {1}\tBunkers: {2}\tEnemies: {3}", baseString, ((JArray)obj[MapData.PLAYERS]).Count, ((JArray)obj[MapData.TURRETS]).Count, ((JArray)obj[MapData.ENEMIES]).Count);
                    break;

                case TransmitionTypes.PLACE_TURRET:
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
