using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace GameServer.Data
{
    public static class Transmition
    {

        internal static class Base
        {
            public const string ID = "id";
            public const string TYPE = "type";
            public const string TIME_STAMP = "timestamp";

            public static string IncomingValidate(JObject obj)
            {
                JToken type = obj[TYPE];

                if (type == null)
                {
                    return "Missing required field: type";
                }

                if (type.Type != JTokenType.String)
                {
                    return "type field is not a string: " + type.Type + " Value:" + type;
                }

                if (!TransmitionTypes.isTransmitionType(type.ToString()))
                {
                    return "Invalid transmition type Expected: login Actual: " + type;
                }

                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                JToken id = obj[ID];
                JToken type = obj[TYPE];
                JToken timestamp = obj[TIME_STAMP];

                if (id == null)
                {
                    return "Missing required field: id";
                }

                if (id.Type != JTokenType.Integer)
                {
                    return "id field is not an integer value. Was instead: " + id.Type + " Value: " + id;
                }

                if (id.Value<int>() < 0)
                {
                    return "id field must be larger than 0. Currently is: " + id.Value<int>();
                }

                if (timestamp == null)
                {
                    return "Missing required field: timestamp";
                }

                if (timestamp.Type != JTokenType.Integer)
                {
                    return "timestamp field is not an integer value. Was instead: " + timestamp.Type + " Value: " + timestamp;
                }

                return IncomingValidate(obj);
            }
        }

        internal static class Error
        {
            public const string NAME = "name";
            public const string CODE = "code";
            public const string MESSAGE = "message";

            public static JObject Create(string name, int code, string message)
            {
                JObject obj = new JObject();
                obj.Add(Base.ID, -1);
                obj.Add(Base.TYPE, TransmitionTypes.ERROR);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());
                obj.Add(NAME, name);
                obj.Add(CODE, code);
                obj.Add(MESSAGE, message);

                string errorMessage;
                if (Validate(obj, out errorMessage))
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static bool Validate(JObject obj, out string errorMessage)
            {
                errorMessage = "";
                return true;
            }

        }

        internal static class Login
        {
            public const string USERNAME = "username";

            public static JObject Create(int id, string username)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.LOGIN);
                obj.Add(Base.ID, id);
                obj.Add(USERNAME, username);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                string errorMessage = OutGoingValidate(obj);
                if (errorMessage == "")
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static string IncomingValidate(JObject obj)
            {
                JToken username = obj[USERNAME];

                if (username == null)
                {
                    return "Missing required field: 'username'";
                }

                if (username.Type != JTokenType.String)
                {
                    return "username field is not a string: " + username.Type + " Value:" + username;
                }

                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                if (IncomingValidate(obj) != "")
                {
                    return IncomingValidate(obj);
                }
                return Base.OutGoingValidate(obj);
            }
        }

        internal static class Logout
        {
            public static JObject Create(int id)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.LOGOUT);
                obj.Add(Base.ID, id);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                string errorMessage = OutGoingValidate(obj);
                if (errorMessage == "")
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static string IncomingValidate(JObject obj)
            {
                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                return Base.OutGoingValidate(obj);
            }
        }

        internal static class Ping
        {
            public static JObject Create(int id)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.PING);
                obj.Add(Base.ID, id);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                string errorMessage = OutGoingValidate(obj);
                if (errorMessage == "")
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static string IncomingValidate(JObject obj)
            {
                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                return Base.OutGoingValidate(obj);
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
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.MOVE);
                obj.Add(Base.ID, id);
                obj.Add(DIRECTION, direction);
                obj.Add(COMPLETE, complete);
                obj.Add(X, x);
                obj.Add(Y, y);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                string errorMessage = OutGoingValidate(obj);
                if (errorMessage == "")
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static string IncomingValidate(JObject obj)
            {
                JToken direction = obj[DIRECTION];
                JToken complete = obj[COMPLETE];
                JToken x = obj[X];
                JToken y = obj[Y];

                if (direction == null)
                {
                    return "Missing required field: 'direction'";
                }

                if (direction.Type != JTokenType.Integer)
                {
                    return "direction field is not an integer: " + direction.Type + " Value:" + direction;
                }

                if (!(direction.Value<int>() >= 0 && direction.Value<int>() <= 3))
                {
                    return "The direction value must be one of the following 0:Up, 1:Down, 2:Left, 3:Right";
                }

                if (complete == null)
                {
                    return "Missing required field: 'complete'";
                }

                if (complete.Type != JTokenType.Boolean)
                {
                    return "complete field is not an boolean: " + complete.Type + " Value:" + complete;
                }

                if (x != null)
                {
                    if (x.Type != JTokenType.Float)
                    {
                        return "x field is not an float: " + x.Type + " Value:" + x;
                    }
                }

                if (y != null)
                {
                    if (y.Type != JTokenType.Float)
                    {
                        return "y field is not an float: " + y.Type + " Value:" + y;
                    }
                }
                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                if (IncomingValidate(obj) != "")
                {
                    return IncomingValidate(obj);
                }
                return Base.OutGoingValidate(obj);
            }
        }

        internal static class PlaceTurret
        {
            public static string X = "x";
            public static string Y = "y";

            public static JObject Create(int id, int x, int y)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.PLACE_TURRET);
                obj.Add(Base.ID, id);
                obj.Add(X, x);
                obj.Add(Y, y);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                string errorMessage = OutGoingValidate(obj);
                if (errorMessage == "")
                {
                    return obj;
                }
                else
                {
                    throw new Exception(errorMessage);
                }
            }

            public static string IncomingValidate(JObject obj)
            {
                JToken x = obj[X];
                JToken y = obj[Y];

                if (x.Type != JTokenType.Float && x.Type != JTokenType.Integer)
                {
                    return "x field is not an float: " + x.Type + " Value:" + x;
                }

                if (y.Type != JTokenType.Float && y.Type != JTokenType.Integer)
                {
                    return "y field is not an float: " + y.Type + " Value:" + y;
                }

                return "";
            }

            public static string OutGoingValidate(JObject obj)
            {
                if (IncomingValidate(obj) != "")
                {
                    return IncomingValidate(obj);
                }
                return Base.OutGoingValidate(obj);
            }
        }

        internal static class MapData
        {
            public const string PLAYERS = "players";
            public const string TURRETS = "turrets";
            public const string ENEMIES = "enemies";

            public const string TYPE_ID = "type_id";
            public const string CREATION_ID = "creation_id";
            public const string NAME = "name";
            public const string X = "x";
            public const string Y = "y";
            public const string WIDTH = "width";
            public const string HEIGHT = "height";
            public const string ACTION_RADIOUS = "action_radious";
            public const string MOVES = "moves";


            public static JObject Create(int id) {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.MAP_DATA);
                obj.Add(Base.ID, id);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());
                obj.Add(PLAYERS, new JArray());
                obj.Add(TURRETS, new JArray());
                obj.Add(ENEMIES, new JArray());

                return obj;
            }

            public static JObject CreateGameObject(int type_id, int creation_id, string name, double x, double y, int width, int height, double action_radious, bool[] moves) {
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

        public static JObject Parse(byte[] data, out string errorMessage)
        {
            JObject jdata = JObject.Parse(Encoding.ASCII.GetString(data));

            if ((errorMessage = Base.IncomingValidate(jdata)) == "")
            {

                switch ((string)jdata[Base.TYPE])
                {
                    case TransmitionTypes.LOGIN:
                        errorMessage = Login.IncomingValidate(jdata);
                        break;

                    case TransmitionTypes.LOGOUT:
                        errorMessage = Logout.IncomingValidate(jdata);
                        break;

                    case TransmitionTypes.PING:
                        errorMessage = Ping.IncomingValidate(jdata);
                        break;

                    case TransmitionTypes.MOVE:
                        errorMessage = Move.IncomingValidate(jdata);
                        break;

                    case TransmitionTypes.PLACE_TURRET:
                        errorMessage = PlaceTurret.IncomingValidate(jdata);
                        break;
                }
            }

            if (errorMessage == "")
            {
                return jdata;
            }
            else
            {
                return null;
            }
        }

        public static byte[] Serialize(JObject obj)
        {
            return Encoding.ASCII.GetBytes(string.Join("", obj.ToString().Split('\n', '\t', '\r', ' ')));
        }
    }
}
