using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;
using GameServer.Data.Resources;

namespace GameServer.Data.Interactables {
	public class Player : Controllable {
		public const int inventorySize = 30;
		public List<Item> Inventory { get; set; }

		public Player(int id, string username) : base(id, username, 0, 0) {

			Inventory = new List<Item>();
			// temp test
			Inventory.Add(new Ore(1000, "Copper Ore", 10, 20.5, "It's copper what else is there to say", 0.67335, 1085));
		}

		public void resizeInventory() {
			for (int i = Inventory.Count - 1; i < inventorySize-1; i++) {
				Inventory.Add(null);
			}
		}

		public void moveCombineItem(int index1, int index2) {
			resizeInventory();
			// this will need to change when more than ore is added
			Ore item1 = (Ore)Inventory[index1];
			Ore item2 = (Ore)Inventory[index2];

			if (item1 != null && item2 != null && item1.id == item2.id) {
				item2.add(item1.weight, item2.purity);
				Inventory[index1] = null;
			} else {
				// switch locations if they arn't compatable
				Inventory[index1] = item2;
				Inventory[index2] = item1;
			}
		}

		public override string ToString() {
			return base.ToString();
		}
	}
}
