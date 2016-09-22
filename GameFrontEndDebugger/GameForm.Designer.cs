namespace GameFrontEndDebugger
{
    partial class GameForm
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
			this.components = new System.ComponentModel.Container();
			this.pingger = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// pingger
			// 
			this.pingger.Enabled = true;
			this.pingger.Interval = 1000;
			this.pingger.Tick += new System.EventHandler(this.pingger_Tick);
			// 
			// GameForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(777, 609);
			this.Name = "GameForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Game Debugger";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameApplication_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameApplication_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameApplication_KeyUp);
			this.Resize += new System.EventHandler(this.GameForm_Resize);
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer pingger;
	}
}

