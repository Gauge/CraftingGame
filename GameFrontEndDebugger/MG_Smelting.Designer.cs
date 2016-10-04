namespace GameFrontEndDebugger {
	partial class MG_Smelting {
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
			this.ResourcePanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.Display = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// ResourcePanel
			// 
			this.ResourcePanel.AllowDrop = true;
			this.ResourcePanel.BackColor = System.Drawing.Color.White;
			this.ResourcePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResourcePanel.Location = new System.Drawing.Point(46, 181);
			this.ResourcePanel.Name = "ResourcePanel";
			this.ResourcePanel.Size = new System.Drawing.Size(50, 50);
			this.ResourcePanel.TabIndex = 0;
			this.ResourcePanel.Tag = "";
			this.ResourcePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.dragComplete);
			this.ResourcePanel.DragOver += new System.Windows.Forms.DragEventHandler(this.dragOver);
			this.ResourcePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.select);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 234);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Drag and Drop Resource";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(427, 234);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Press Enter to Begin";
			// 
			// Display
			// 
			this.Display.BackColor = System.Drawing.Color.White;
			this.Display.Location = new System.Drawing.Point(12, 12);
			this.Display.Name = "Display";
			this.Display.Size = new System.Drawing.Size(518, 163);
			this.Display.TabIndex = 3;
			// 
			// MG_Smelting
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(542, 256);
			this.Controls.Add(this.Display);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ResourcePanel);
			this.Name = "MG_Smelting";
			this.Text = "MG_Smelting";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel ResourcePanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel Display;
	}
}