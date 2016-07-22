﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameFrontEndDebugger {
	public partial class ChatDisplay : RichTextBox {

		public ChatDisplay() {
			SetStyle(ControlStyles.SupportsTransparentBackColor |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint,
				true);

			Enabled = false;
			BorderStyle = BorderStyle.None;
			ScrollBars = RichTextBoxScrollBars.None;
			BackColor = Color.FromArgb(100, 0, 0, 0);
			ForeColor = Color.FromArgb(130, 255, 255, 255);
			backColor = new SolidBrush(BackColor);
			foreColor = new SolidBrush(ForeColor);
		}

		protected override void OnBackColorChanged(EventArgs e) {
			backColor = new SolidBrush(BackColor);
		}

		protected override void OnForeColorChanged(EventArgs e) {
			foreColor = new SolidBrush(ForeColor);
		}

		Bitmap backgroundImage;
		Bitmap parentImage;
		SolidBrush foreColor;
		SolidBrush backColor;
		protected override void OnPaint(PaintEventArgs e) {
            RectangleF r = DisplayRectangle;

			backgroundImage = new Bitmap((int)r.Width, (int)r.Height);
			parentImage = ((GameForm)Parent).display;
			Graphics g = Graphics.FromImage(backgroundImage);

			GraphicsUnit gu = GraphicsUnit.Pixel;
			Point locationOnForm = Parent.PointToClient(Parent.PointToScreen(Location));
			RectangleF imageSelect = new RectangleF(locationOnForm.X, locationOnForm.Y, Width, Height);

			g.DrawImage(parentImage, r, imageSelect, gu);
			g.FillRectangle(backColor, r);

			Rectangle textLoc = new Rectangle(0, Height, Width, 0);
			for (int i = Lines.Length-1; i > 0; i--) {
				textLoc.Height = (int)g.MeasureString(Lines[i], Font, Width).Height;
				textLoc.Y -= textLoc.Height;
				g.DrawString(Lines[i], Font, foreColor, textLoc);
			}
			g.Flush();
			g.Dispose();

			Graphics drawer = e.Graphics;
			drawer.DrawImage(backgroundImage, 0, 0);
		}
	}
}
