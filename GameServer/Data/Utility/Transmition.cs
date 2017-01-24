using GameServer.Data.Errors;
using Newtonsoft.Json.Linq;
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

            public static bool Validate(JObject obj)
            {
                int temp;
                long temp2;
                JToken id = obj[ID];
                JToken type = obj[TYPE];
                JToken timestamp = obj[TIME_STAMP];

                if (id == null)
                {
                    throw new TransmitionValidationException("Missing required field: id");
                }

                if (!int.TryParse((string)id, out temp))
                {
                    throw new TransmitionValidationException("Id field is not an integer value. Was instead: " + id.Type + " Value: " + id);
                }

                if (type == null)
                {
                    throw new TransmitionValidationException("Missing required field: type");
                }

                if (!TransmitionTypes.isTransmitionType(type.ToString()))
                {
                    throw new TransmitionValidationException("Invalid transmition type Expected: login Actual: " + type);
                }

                if (type.Type != JTokenType.String)
                {
                    throw new TransmitionValidationException("type field is not a string: " + id.Type + " Value:" + id);
                }

                if (timestamp == null)
                {
                    throw new TransmitionValidationException("Missing required field: timestamp");
                }

                if (!long.TryParse((string)timestamp, out temp2))
                {
                    throw new TransmitionValidationException("timestamp field is not an long time format value. Was instead: " + timestamp.Type + " Value: " + timestamp);
                }

                return true;
            }
        }

        internal static class Error
        {
            public const string NAME = "name";
            public const string CODE = "code";
            public const string MESSAGE = "message";
            public const string STACK_TRACE = "stacktrace";

            public static JObject Create(string name, int code, string message = "", string stackTrace = "")
            {
                JObject obj = new JObject();
                obj.Add(Base.ID, -1);
                obj.Add(Base.TYPE, TransmitionTypes.ERROR);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());
                obj.Add(NAME, name);
                obj.Add(CODE, code);
                obj.Add(MESSAGE, message);
                obj.Add(STACK_TRACE, stackTrace);

                Validate(obj);
                return obj;
            }

            public static bool Validate(JObject obj)
            {
                return true;
            }

        }

        internal static class Login
        {
            public const string USERNAME = "username";
            public const string PASSWORD = "password";

            public static JObject Create(int id, string username)
            {
                JObject obj = new JObject();
                obj.Add(Base.TYPE, TransmitionTypes.LOGIN);
                obj.Add(Base.ID, id);
                obj.Add(USERNAME, username);
                obj.Add(Base.TIME_STAMP, Helper.getTimestamp());

                Validate(obj);
                return obj;
            }

            public static bool Validate(JObject obj)
            {
                JToken username = obj[USERNAME];
                JToken password = obj[PASSWORD];

                if (username == null)
                {
                    throw new TransmitionValidationException("Missing required field: 'username'");
                }

                if (username.Type != JTokenType.String)
                {
                    throw new TransmitionValidationException("username field is not a string: " + username.Type + " Value:" + username);
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

                Validate(obj);
                return obj;
            }

            public static bool Validate(JObject obj)
            {
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

                Validate(obj);
                return obj;
            }

            public static bool Validate(JObject obj)
            {
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

                Validate(obj);
                return obj;
            }

            public static bool Validate(JObject obj)
            {

                JToken direction = obj[DIRECTION];
                JToken complete = obj[COMPLETE];
                JToken x = obj[X];
                JToken y = obj[Y];

                if (direction == null)
                {
                    throw new TransmitionValidationException("Missing required field: 'direction'");
                }

                if (direction.Type != JTokenType.Integer)
                {
                    throw new TransmitionValidationException("direction field is not an integer: " + direction.Type + " Value:" + direction);
                }

                if (!(direction.Value<int>() >= 0 && direction.Value<int>() <= 3))
                {
                    throw new TransmitionValidationException("The direction value must be one of the following 0:Up, 1:Down, 2:Left, 3:Right");
                }

                if (complete == null)
                {
                    throw new TransmitionValidationException("Missing required field: 'complete'");
                }

                if (complete.Type != JTokenType.Boolean)
                {
                    throw new TransmitionValidationException("complete field is not an boolean: " + complete.Type + " Value:" + complete);
                }

                if (x != null)
                {
                    if (x.Type != JTokenType.Float)
                    {
                        throw new TransmitionValidationException("x field is not an float: " + x.Type + " Value:" + x);
                    }
                }

                if (y != null)
                {
                    if (y.Type != JTokenType.Float)
                    {
                        throw new TransmitionValidationException("y field is not an float: " + y.Type + " Value:" + y);
                    }
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

            public static bool isTransmitionType(string type)
            {
                return
                    (
                    type.Equals(LOGIN) ||
                    type.Equals(LOGOUT) ||
                    type.Equals(PING) ||
                    type.Equals(ERROR) ||
                    type.Equals(MOVE)
                    );
            }
        }

        public static JObject Parse(byte[] data)
        {
            JObject jdata = JObject.Parse(Encoding.ASCII.GetString(data));

            if (Base.Validate(jdata))
            {

                switch ((string)jdata[Base.TYPE])
                {

                    case TransmitionTypes.LOGIN:
                        Login.Validate(jdata);
                        break;

                    case TransmitionTypes.LOGOUT:
                        Logout.Validate(jdata);
                        break;
                }
            }
            return jdata;
        }

        public static byte[] Serialize(JObject obj)
        {
            return Encoding.ASCII.GetBytes(obj.ToString());
        }
    }
}
