using GameServer;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameFrontEndDebugger {
	public partial class InventoryForm : Form {

		public int width { get; }
		public int height { get;  }
		private Panel[] panels = new Panel[30];

		private int selectedPanel = -1;
		private int secondaryPanel = -1;


		public InventoryForm() {

			width = 50;
			height = 50;

			int index = 0;
			for (int x = 0; x < 5; x++) {
				for (int y = 0; y < 6; y++) {
					Panel p = new Panel();
					p.BackColor = Color.White;
					p.Location = new Point(x * width + x*5 + 5, y * height + y*5 + 5);
					p.Width = width;
					p.Height = height;
					p.MouseDown += select;
					p.DragOver += dragOver;
					p.DragDrop += dragComplete;
					p.AllowDrop = true;
					panels[index] = p;
					p.Tag = index;
					Controls.Add(p);
					index++;
                }
			}

			int titleHeight = RectangleToScreen(ClientRectangle).Top - Top;

			Size = new Size((width * 5) + 30, (height * 6) + 35 + titleHeight);
			InitializeComponent();
		}

		public void select(object sender, MouseEventArgs e) {
			selectedPanel = (int)((Panel)sender).Tag;
			((Panel)sender).DoDragDrop(sender, DragDropEffects.All);
        }

		public void dragOver(object sender, DragEventArgs e) {
			secondaryPanel = (int)((Panel)sender).Tag;
			e.Effect = DragDropEffects.Move;
		}

		public void dragComplete(object sender, DragEventArgs e) {
            if (selectedPanel != -1 && selectedPanel != secondaryPanel) {
				((GameForm)Owner).combineInventoryItems(selectedPanel, secondaryPanel);
			}
			selectedPanel = -1;
			secondaryPanel = -1;
		}

		public void update(Player active) {
			if (Visible) {
				Location = new Point(Owner.Location.X + Owner.Width, Owner.Location.Y);
			}

			if (active != null) {
				for (int i = 0; i < active.Inventory.Count; i++) {
					Item item = active.Inventory[i];
					Panel p = panels[i];

					if (item == null){
						p.BackColor = Color.White;
					} else if (item.id == 1000) {
						p.BackColor = Color.Brown;
					}
				}
			}

		}

		private void InventoryForm_FormClosing(object sender, FormClosingEventArgs e) {
			Hide();
			e.Cancel = true;
		}
	}
}
