using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GameFrontEndDebugger.Controls {
	public partial class ChatInput : TextBox {

		[DllImport("user32.dll")]
		static extern bool HideCaret(IntPtr hWnd);

		public ChatInput() {
			SetStyle(ControlStyles.SupportsTransparentBackColor |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint,
				true);

			ReadOnly = true;
			Enabled = false;
			Multiline = true;
			BorderStyle = BorderStyle.None;
			BackColor = Color.FromArgb(100, 0, 0, 0);
			ForeColor = Color.White;
			backColor = new SolidBrush(BackColor);
			foreColor = new SolidBrush(ForeColor);
		}

		public void Activate() {
			ReadOnly = false;
			Enabled = true;
			Focus();
		}

		public void Deactivate() {
			ReadOnly = true;
			Enabled = false;
			Clear();
		}

		protected override void OnGotFocus(EventArgs e) {
			HideCaret(Handle);
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
		RectangleF r;
		Graphics g;
		Point locationOnForm;
		RectangleF imageSelect;
        protected override void OnPaint(PaintEventArgs e) {
			r = DisplayRectangle;

			backgroundImage = new Bitmap((int)r.Width, (int)r.Height);
			parentImage = ((GameForm)Parent).display;
			g = Graphics.FromImage(backgroundImage);

			GraphicsUnit gu = GraphicsUnit.Pixel;
			locationOnForm = Parent.PointToClient(Parent.PointToScreen(Location));
			imageSelect = new RectangleF(locationOnForm.X, locationOnForm.Y, Width, Height);

			g.DrawImage(parentImage, r, imageSelect, gu);
			g.FillRectangle(backColor, r);

			g.FillRectangle(new SolidBrush(BackColor), DisplayRectangle);
			g.DrawString(Text, Font, foreColor, DisplayRectangle);

			g.Flush();
			g.Dispose();

			Graphics drawer = e.Graphics;
			drawer.DrawImage(backgroundImage, 0, 0);
		}
	}
}
