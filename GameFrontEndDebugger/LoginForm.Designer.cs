namespace GameFrontEndDebugger
{
    partial class LoginForm
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
			this.tbIP = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbPort = new System.Windows.Forms.TextBox();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.buttonConnect = new System.Windows.Forms.Button();
			this.tbPassword = new System.Windows.Forms.MaskedTextBox();
			this.SuspendLayout();
			// 
			// tbIP
			// 
			this.tbIP.Location = new System.Drawing.Point(81, 10);
			this.tbIP.Name = "tbIP";
			this.tbIP.Size = new System.Drawing.Size(125, 20);
			this.tbIP.TabIndex = 0;
			this.tbIP.Text = "127.0.0.1";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Server IP:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Server Port:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Username:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(21, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Password:";
			// 
			// tbPort
			// 
			this.tbPort.Location = new System.Drawing.Point(81, 36);
			this.tbPort.Name = "tbPort";
			this.tbPort.Size = new System.Drawing.Size(125, 20);
			this.tbPort.TabIndex = 6;
			this.tbPort.Text = "1234";
			// 
			// tbUsername
			// 
			this.tbUsername.Location = new System.Drawing.Point(81, 62);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(125, 20);
			this.tbUsername.TabIndex = 7;
			// 
			// buttonConnect
			// 
			this.buttonConnect.Location = new System.Drawing.Point(12, 126);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(194, 23);
			this.buttonConnect.TabIndex = 10;
			this.buttonConnect.Text = "Connect";
			this.buttonConnect.UseVisualStyleBackColor = true;
			this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(81, 88);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.Size = new System.Drawing.Size(125, 20);
			this.tbPassword.TabIndex = 9;
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(218, 161);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.buttonConnect);
			this.Controls.Add(this.tbUsername);
			this.Controls.Add(this.tbPort);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbIP);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Login";
			this.Load += new System.EventHandler(this.Login_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.MaskedTextBox tbPassword;
    }
}