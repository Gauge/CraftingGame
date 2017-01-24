using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data {
	public static class Helper {
		private static long lastTime = -1;
		private static long currentTime = 0;
		private static long delta = 0;

		public static long getDelta(bool update = false) {
			if (lastTime == -1) { // if first time run
				lastTime = getTimestamp();
				return 0;
			}

			if (update) {
				currentTime = getTimestamp();
				delta = currentTime - lastTime;
				lastTime = currentTime;
				return delta;
			} else {
				return delta;
			}
		}

		public static long getTimestamp() {
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

        public static string getFormatedTimestamp() {
            return DateTime.Now.ToString("MM/dd/yy HH:mm:ss:fff");
        }
	}
}
