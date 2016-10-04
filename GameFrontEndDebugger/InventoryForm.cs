using GameServer.Data;
using GameServer.Data.Interactables;
using System.Drawing;
using System.Windows.Forms;
using Inv = GameServer.Data.Inventory;

namespace GameFrontEndDebugger {
	public partial class InventoryForm : Form {

		public int width { get; }
		public int height { get; }
		private Panel[] panels = new Panel[30];
		private Player player;

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
					p.BackgroundImageLayout = ImageLayout.Stretch;
					Controls.Add(p);
					index++;
                }
			}

			int titleHeight = RectangleToScreen(ClientRectangle).Top - Top;
			InitializeComponent();
		}

		public void select(object sender, MouseEventArgs e) {
			//selectedPanel = (int)((Panel)sender).Tag;
			((Panel)sender).DoDragDrop(sender, DragDropEffects.All);
        }

		public void dragOver(object sender, DragEventArgs e) {
			//secondaryPanel = (int)((Panel)sender).Tag;

			e.Effect = DragDropEffects.Move;
		}

		public void dragComplete(object sender, DragEventArgs e) {
			object o = ((Panel)sender).Tag;
			object o2 = ((Panel)e.Data.GetData("System.Windows.Forms.Panel")).Tag;

			if (o is int && o2 is Item) {
				Item eItem = (Item)((Panel)e.Data.GetData("System.Windows.Forms.Panel")).Tag;
				Inv inv = new Inv(Inv.TYPE.Add, player.id, (int)o, item: eItem);
				((GameForm)Owner).transmit(inv);


			} else if (o is int && o2 is int && !o.Equals(o2)) {
				Inv inv = new Inv(Inv.TYPE.Move, player.id, (int)o, (int)o2);
				((GameForm)Owner).transmit(inv);
			}
			//selectedPanel = secondaryPanel = -1;
		}



		public void update() {
			player = ((GameForm)Owner).activePlayer;

			if (Visible) {
				Location = new Point(Owner.Location.X + Owner.Width, Owner.Location.Y);
			}

			if (player != null) {
				for (int i = 0; i < player.Inventory.Count; i++) {
					Item item = player.Inventory[i];
					Panel p = panels[i];

					if (item == null) {
						p.BackgroundImage = null;
					} else {
						p.BackgroundImage = Assets.getIconById(item.id);
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
