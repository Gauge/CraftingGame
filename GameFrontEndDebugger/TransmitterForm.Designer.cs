namespace GameFrontEndDebugger {
	partial class TransmitterForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dataTextBox = new System.Windows.Forms.RichTextBox();
			this.transmitButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dataTextBox
			// 
			this.dataTextBox.Location = new System.Drawing.Point(13, 13);
			this.dataTextBox.Name = "dataTextBox";
			this.dataTextBox.Size = new System.Drawing.Size(465, 266);
			this.dataTextBox.TabIndex = 0;
			this.dataTextBox.Text = "";
			// 
			// transmitButton
			// 
			this.transmitButton.Location = new System.Drawing.Point(166, 290);
			this.transmitButton.Name = "transmitButton";
			this.transmitButton.Size = new System.Drawing.Size(159, 23);
			this.transmitButton.TabIndex = 1;
			this.transmitButton.Text = "Transmit";
			this.transmitButton.UseVisualStyleBackColor = true;
			this.transmitButton.Click += new System.EventHandler(this.transmitButton_Click);
			// 
			// TransmitterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 325);
			this.Controls.Add(this.transmitButton);
			this.Controls.Add(this.dataTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TransmitterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Transmitter";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox dataTextBox;
		private System.Windows.Forms.Button transmitButton;
	}
}