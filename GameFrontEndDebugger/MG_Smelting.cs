using System.Drawing;
using System.Windows.Forms;
using GameServer.Data.MiniGames;
using GameServer.Data;
using GameServer.Data.Resources;
using GameServer.Data.Interactables;
using Inv = GameServer.Data.Inventory;

namespace GameFrontEndDebugger {

	public partial class MG_Smelting : Form {
		private enum STATE { Forground, Background, Dead }
		public Smelting game { get; set; }
		private Player player;

		public MG_Smelting() {
			InitializeComponent();
			ResourcePanel.MouseDown += select;
			ResourcePanel.Tag = null;
		}

		public void update() {
			player = ((GameForm)Owner).activePlayer;

			if (player != null && player.ActiveMiniGame != null && !Visible) {
				this.Show(Owner);
			} else if (player != null && player.ActiveMiniGame == null && Visible) {
				this.Hide();
			}

			if (Visible) {
				Location = new Point(Owner.Location.X + Owner.Width, Owner.Location.Y + 375);
				Focus();
			}

			if (ResourcePanel.Tag == null) {
				ResourcePanel.BackgroundImage = null;
			} else {
				ResourcePanel.BackgroundImage = Assets.getIconById(((Item)ResourcePanel.Tag).id);
			}

			draw();
		}

		public void select(object sender, MouseEventArgs e) {
			((Panel)sender).DoDragDrop(sender, DragDropEffects.All);
		}

		public void dragOver(object sender, DragEventArgs e) {
			object o = ((Panel)e.Data.GetData("System.Windows.Forms.Panel")).Tag;
			if (o.GetType().ToString() == "System.Int32") {
				int index = (int)o;
				Item item = ((GameForm)Owner).activePlayer.Inventory[index];
				if (item is Ore) {
					e.Effect = DragDropEffects.Move;

				} else {
					e.Effect = DragDropEffects.None;

				}
			}
		}

		public void dragComplete(object sender, DragEventArgs e) {
			object o = ((Panel)sender).Tag;
			object o2 = ((Panel)e.Data.GetData("System.Windows.Forms.Panel")).Tag;

			if (o == null && o2 is int) {
				int index = (int)o2;
				Item item = ((GameForm)Owner).activePlayer.Inventory[index];
				if (item is Ore) {
					((Panel)sender).Tag = item;
					Inv inv = new Inv(Inv.TYPE.Remove, player.id, index);
					((GameForm)Owner).transmit(inv);
				}
			} else if (o is Ore && o2 is int) {
				((Panel)sender).Tag = null;
            }
		}

		private void draw() {
			if (game != null && game.state == Smelting.STATE.Active) {
				Graphics g = Display.CreateGraphics();

				g.DrawLine(Pens.Beige, 50, 50, 200, 200);

			}
			
		}
	}
}
