using System;
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

			ReadOnly = true;
			Enabled = false;
			BorderStyle = BorderStyle.None;
			ScrollBars = RichTextBoxScrollBars.None;
			BackColor = Color.FromArgb(100, 0, 0, 0);
			ForeColor = Color.FromArgb(130, 255, 255, 255);
			backColor = new SolidBrush(BackColor);
			foreColor = new SolidBrush(ForeColor);
		}

		public void Activate() {
			ForeColor = Color.FromArgb(255, 255, 255, 255);
		}

		public void Deactivate() {
			ForeColor = Color.FromArgb(130, 255, 255, 255);
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
		Graphics g;
		Point locationOnForm;
		RectangleF imageSelect;
		Rectangle textLoc;
        protected override void OnPaint(PaintEventArgs e) {
            RectangleF r = DisplayRectangle;

			backgroundImage = new Bitmap((int)r.Width, (int)r.Height);
			parentImage = ((GameForm)Parent).display;
			g = Graphics.FromImage(backgroundImage);

			GraphicsUnit gu = GraphicsUnit.Pixel;
			locationOnForm = Parent.PointToClient(Parent.PointToScreen(Location));
			imageSelect = new RectangleF(locationOnForm.X, locationOnForm.Y, Width, Height);

			g.DrawImage(parentImage, r, imageSelect, gu);
			g.FillRectangle(backColor, r);

			textLoc = new Rectangle(0, Height, Width, 0);
			for (int i = Lines.Length-1; i > 0; i--) {
				if (textLoc.Y < 0) {
					break;
				} else {
					textLoc.Height = (int)g.MeasureString(Lines[i], Font, Width).Height;
					textLoc.Y -= textLoc.Height;
					g.DrawString(Lines[i], Font, foreColor, textLoc);
				}
			}
			g.Flush();
			g.Dispose();

			Graphics drawer = e.Graphics;
			drawer.DrawImage(backgroundImage, 0, 0);
		}
	}
}
