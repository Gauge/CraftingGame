namespace GameFrontEndDebugger.Controls {
	public partial class Logger : ChatDisplay {

		public Logger() {
		}

		public void Log(string message) {
			Text += message + "\n";
		}
	}
}
