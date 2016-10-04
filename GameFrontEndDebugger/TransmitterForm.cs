using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameFrontEndDebugger {
	public partial class TransmitterForm : Form {
		public TransmitterForm() {
			InitializeComponent();
		}

		private void transmitButton_Click(object sender, EventArgs e) {
			((GameForm)Owner).transmit(dataTextBox.Text);
			dataTextBox.SelectAll();
		}

		public void update() {
			Location = new Point(Owner.Location.X - Width, Owner.Location.Y);
		}
	}
}
