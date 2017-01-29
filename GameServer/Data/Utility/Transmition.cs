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

            public static bool Validate(JObject obj, out string errorMessage)
            {
                errorMessage = "";
                int temp;
                long temp2;
                JToken id = obj[ID];
                JToken type = obj[TYPE];
                JToken timestamp = obj[TIME_STAMP];

                if (id == null)
                {
                    errorMessage = "Missing required field: id";
                    return false;
                }

                if (!int.TryParse((string)id, out temp))
                {
                    errorMessage = "Id field is not an integer value. Was instead: " + id.Type + " Value: " + id;
                    return false;
                }

                if (type == null)
                {
                    errorMessage = "Missing required field: type";
                    return false;
                }

                if (!TransmitionTypes.isTransmitionType(type.ToString()))
                {
                    errorMessage = "Invalid transmition type Expected: login Actual: " + type;
                    return false;
                }

                if (type.Type != JTokenType.String)
                {
                    errorMessage = "type field is not a string: " + id.Type + " Value:" + id;
                    return false;
                }

                if (timestamp == null)
                {
                    errorMessage = "Missing required field: timestamp";
                    return false;
                }

                if (!long.TryParse((string)timestamp, out temp2))
                {
                    errorMessage = "timestamp field is not an long time format value. Was instead: " + timestamp.Type + " Value: " + timestamp;
                    return false;
                }

                return true;
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
                else {
                    throw new Exception(errorMessage);
                }
            }

            public static bool Validate(JObject obj,  out string errorMessage)
            {
                errorMessage = "";
                return true;
            }

        }

        internal static class Login
        {
            public const string USERNAME = "username";
            //public const string PASSWORD = "password";

            public static JObject Create(int id, string username)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.LOGIN);
                obj.Add(Base.ID, id);
                obj.Add(USERNAME, username);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

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
                JToken username = obj[USERNAME];
                //JToken password = obj[PASSWORD];

                if (username == null)
                {
                    errorMessage = "Missing required field: 'username'";
                    return false;
                }

                if (username.Type != JTokenType.String)
                {
                    errorMessage ="username field is not a string: " + username.Type + " Value:" + username;
                    return false;
                }

                return true;
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

        internal static class Ping
        {
            public static JObject Create(int id)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.PING);
                obj.Add(Base.ID, id);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

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
                JToken direction = obj[DIRECTION];
                JToken complete = obj[COMPLETE];
                JToken x = obj[X];
                JToken y = obj[Y];

                if (direction == null)
                {
                    errorMessage ="Missing required field: 'direction'";
                    return false;
                }

                if (direction.Type != JTokenType.Integer)
                {
                    errorMessage ="direction field is not an integer: " + direction.Type + " Value:" + direction;
                    return false;
                }

                if (!(direction.Value<int>() >= 0 && direction.Value<int>() <= 3))
                {
                    errorMessage ="The direction value must be one of the following 0:Up, 1:Down, 2:Left, 3:Right";
                    return false;
                }

                if (complete == null)
                {
                    errorMessage ="Missing required field: 'complete'";
                    return false;
                }

                if (complete.Type != JTokenType.Boolean)
                {
                    errorMessage ="complete field is not an boolean: " + complete.Type + " Value:" + complete;
                    return false;
                }

                if (x != null)
                {
                    if (x.Type != JTokenType.Float)
                    {
                        errorMessage ="x field is not an float: " + x.Type + " Value:" + x;
                        return false;
                    }
                }

                if (y != null)
                {
                    if (y.Type != JTokenType.Float)
                    {
                        errorMessage ="y field is not an float: " + y.Type + " Value:" + y;
                        return false;
                    }
                }

                return true;
            }
        }

        internal static class PlaceTurret {
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
                JToken x = obj[X];
                JToken y = obj[Y];

                if (x.Type != JTokenType.Float && x.Type != JTokenType.Integer)
                {
                    errorMessage ="x field is not an float: " + x.Type + " Value:" + x;
                }

                if (y.Type != JTokenType.Float && y.Type != JTokenType.Integer)
                {
                    errorMessage ="y field is not an float: " + y.Type + " Value:" + y;
                }

                return true;
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

            public static bool isTransmitionType(string type)
            {
                return
                    (
                    type.Equals(LOGIN) ||
                    type.Equals(LOGOUT) ||
                    type.Equals(PING) ||
                    type.Equals(ERROR) ||
                    type.Equals(MOVE) ||
                    type.Equals(PLACE_TURRET)
                    );
            }
        }

        public static JObject Parse(byte[] data, out string errorMessage)
        {
            JObject jdata = JObject.Parse(Encoding.ASCII.GetString(data));

            if (Base.Validate(jdata, out errorMessage))
            {

                switch ((string)jdata[Base.TYPE])
                {
                    case TransmitionTypes.LOGIN:
                        Login.Validate(jdata, out errorMessage);
                        break;

                    case TransmitionTypes.LOGOUT:
                        Logout.Validate(jdata, out errorMessage);
                        break;

                    case TransmitionTypes.PING:
                        Ping.Validate(jdata, out errorMessage);
                        break;

                    case TransmitionTypes.MOVE:
                        Move.Validate(jdata, out errorMessage);
                        break;

                    case TransmitionTypes.PLACE_TURRET:
                        PlaceTurret.Validate(jdata, out errorMessage);
                        break;
                }
            }

            if (string.IsNullOrEmpty(errorMessage))
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
            return Encoding.ASCII.GetBytes(obj.ToString());
        }
    }
}
