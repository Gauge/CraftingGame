using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public static class Logger
    {
        public enum LogLevel { normal, debug }
        public enum Type {INTERNAL, CONNECTED, DISCONNECTED, ERROR, LOGIN, LOGOUT, PING, MOVE }

        public static LogLevel Log_Level = LogLevel.debug;

        public static void Log(LogLevel level, Type type, string message) {
            if (Log_Level == LogLevel.debug || level == LogLevel.normal)
            {
                Console.WriteLine(string.Format("{0}\t{1}\t{2}", Helper.getFormatedTimestamp(), type.ToString(), message));
            }
        }
    }
}
