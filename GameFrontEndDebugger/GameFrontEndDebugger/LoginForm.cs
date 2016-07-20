using System;
using System.Windows.Forms;

namespace GameFrontEndDebugger {

	public partial class LoginForm : Form {

		public LoginForm() {
			InitializeComponent();
		}

		private void buttonConnect_Click(object sender, EventArgs e) {
			try {
				((GameForm)Owner).connect(tbIP.Text, int.Parse(tbPort.Text), tbUsername.Text, tbPassword.Text);
			} catch (Exception er) {
				Console.WriteLine(er.ToString());
				Console.WriteLine("failed to parse port number");
			}
			Hide();
		}

		private void Login_Load(object sender, EventArgs e) {
			tbUsername.Text = "Debugger" + (int)(new Random().NextDouble() * 10000);
		}

	}
}
