namespace GameFrontEndDebugger
{
    partial class GameApplication
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
            this.chatInput = new System.Windows.Forms.TextBox();
            this.chatDisplay = new System.Windows.Forms.RichTextBox();
            this.gameCanvas = new System.Windows.Forms.Panel();
            this.mainLoop = new System.Windows.Forms.Timer(this.components);
            this.pingger = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // chatInput
            // 
            this.chatInput.Enabled = false;
            this.chatInput.Location = new System.Drawing.Point(552, 586);
            this.chatInput.Name = "chatInput";
            this.chatInput.Size = new System.Drawing.Size(225, 20);
            this.chatInput.TabIndex = 0;
            this.chatInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatInput_KeyDown);
            // 
            // chatDisplay
            // 
            this.chatDisplay.Enabled = false;
            this.chatDisplay.Location = new System.Drawing.Point(552, 1);
            this.chatDisplay.Name = "chatDisplay";
            this.chatDisplay.ReadOnly = true;
            this.chatDisplay.Size = new System.Drawing.Size(225, 579);
            this.chatDisplay.TabIndex = 1;
            this.chatDisplay.Text = "";
            // 
            // gameCanvas
            // 
            this.gameCanvas.Location = new System.Drawing.Point(0, 1);
            this.gameCanvas.Name = "gameCanvas";
            this.gameCanvas.Size = new System.Drawing.Size(552, 605);
            this.gameCanvas.TabIndex = 2;
            // 
            // mainLoop
            // 
            this.mainLoop.Enabled = true;
            this.mainLoop.Interval = 1;
            this.mainLoop.Tick += new System.EventHandler(this.mainLoop_Tick);
            // 
            // pingger
            // 
            this.pingger.Enabled = true;
            this.pingger.Interval = 1000;
            this.pingger.Tick += new System.EventHandler(this.pingger_Tick);
            // 
            // GameApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 609);
            this.Controls.Add(this.gameCanvas);
            this.Controls.Add(this.chatDisplay);
            this.Controls.Add(this.chatInput);
            this.Name = "GameApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Debugger";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameApplication_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameApplication_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameApplication_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatInput;
        private System.Windows.Forms.RichTextBox chatDisplay;
        private System.Windows.Forms.Panel gameCanvas;
        private System.Windows.Forms.Timer mainLoop;
        private System.Windows.Forms.Timer pingger;
    }
}

