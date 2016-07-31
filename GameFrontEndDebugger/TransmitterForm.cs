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
		public TransmitterForm(Form owner) {
			Location = new Point(owner.Location.X - Width, owner.Location.Y);
			InitializeComponent();
		}

		private void transmitButton_Click(object sender, EventArgs e) {
			((GameForm)Owner).transmit(dataTextBox.Text);
			dataTextBox.SelectAll();
		}
	}
}
