using System;
using System.Windows.Forms;

namespace GameFrontEndDebugger
{
    public partial class LoggerForm : Form
    {
        public LoggerForm() {
            InitializeComponent();
        }

        public void Log(string message) {
            log.Text += message + "\n";
        }

        private void Logger_ResizeEnd(object sender, EventArgs e) {
            log.Size = this.Size;
        }

        private void Logger_Load(object sender, EventArgs e) {
            log.Size = this.Size;
        }

        private void log_TextChanged(object sender, EventArgs e) {
            log.SelectionStart = log.Text.Length;
            log.ScrollToCaret();
        }
    }
}
