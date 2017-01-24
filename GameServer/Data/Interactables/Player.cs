using System.Collections.Generic;
//using GameServer.Data.Resources;

namespace GameServer.Data.Interactables {
	public class Player : Pawn {
		//public Inventory Inventory { get; private set; }
		//public IMiniGame ActiveMiniGame = null;

		public Player(int id, string username/*, Inventory inv = null*/) : base(id, username, 0, 0) {

			/*if (inv == null) {
				Inventory = new Inventory();
			} else {
				Inventory = inv;
			}

			// temp test
			Inventory.PlaceInEmptySlot(new Ore(1000, "Copper Ore", 10, 20.5, "It's copper what else is there to say", 0.67335, 1085));
            */
		}

        /*
		public void MoveItem(IInventory inv1, int index1, IInventory inv2, int index2) {
			Item item1 = inv1.getItems()[index1];
			Item item2 = inv2.getItems()[index2];

			inv1.Replace(index2, item2);
			inv2.Replace(index1, item1);
			
		}
        */

		public override void update() {
			base.update();
        }

		public override string ToString() {
			return base.ToString();
		}
	}
}
