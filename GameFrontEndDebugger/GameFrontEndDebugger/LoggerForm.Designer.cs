namespace GameFrontEndDebugger
{
    partial class LoggerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.chatDisplay1 = new GameFrontEndDebugger.ChatDisplay();
			this.chatInput1 = new GameFrontEndDebugger.Controls.ChatInput();
			this.SuspendLayout();
			// 
			// chatDisplay1
			// 
			this.chatDisplay1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.chatDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.chatDisplay1.Enabled = false;
			this.chatDisplay1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.chatDisplay1.Location = new System.Drawing.Point(175, 83);
			this.chatDisplay1.Name = "chatDisplay1";
			this.chatDisplay1.ReadOnly = true;
			this.chatDisplay1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.chatDisplay1.Size = new System.Drawing.Size(100, 96);
			this.chatDisplay1.TabIndex = 0;
			this.chatDisplay1.Text = "";
			// 
			// chatInput1
			// 
			this.chatInput1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.chatInput1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.chatInput1.Enabled = false;
			this.chatInput1.ForeColor = System.Drawing.Color.White;
			this.chatInput1.Location = new System.Drawing.Point(119, 114);
			this.chatInput1.Multiline = true;
			this.chatInput1.Name = "chatInput1";
			this.chatInput1.ReadOnly = true;
			this.chatInput1.Size = new System.Drawing.Size(100, 20);
			this.chatInput1.TabIndex = 1;
			// 
			// LoggerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(477, 491);
			this.Controls.Add(this.chatInput1);
			this.Controls.Add(this.chatDisplay1);
			this.Name = "LoggerForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Logger";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private ChatDisplay chatDisplay1;
		private Controls.ChatInput chatInput1;
	}
}