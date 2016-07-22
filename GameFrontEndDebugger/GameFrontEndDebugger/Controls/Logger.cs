using System.Windows.Forms;

namespace GameFrontEndDebugger.Controls {
	public partial class Logger : ChatDisplay {

		private string toAdd;

		public Logger() {
			toAdd = "";
		}

		public void Log(string message) {
			toAdd += message + "\n";
		}

		protected override void OnPaint(PaintEventArgs e) {
			Text += toAdd;
			toAdd = "";
			base.OnPaint(e);
		}
	}
}
