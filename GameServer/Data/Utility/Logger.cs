using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public enum Level { NORMAL, DEBUG }

    public static class Logger
    {

        private static StreamWriter file;
        public static Level Log_Level = Level.DEBUG;

        public static void Log(Level level, string message, object logs = null)
        {
            string output = string.Format("{0} {1}\t{2}", Log_Level.ToString(), Helper.getFormatedTimestamp(), message);

            if (file == null) openFile();

            file.Write(output + ((logs != null) ? "\n" + logs.ToString() : "") + "\n");
            Console.WriteLine(output);
        }

        private static void openFile()
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Logs\");
            file = new StreamWriter(Directory.GetCurrentDirectory() + @"\Logs\log_" + Helper.getTimestamp());
        }
    }
}
