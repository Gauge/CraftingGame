using System.Collections.Generic;
using GameServer.Data.Resources;

namespace GameServer.Data.Interactables {
	public class Player : Pawn {
		public Inventory Inventory { get; set; }
		public IMiniGame ActiveMiniGame = null;

		public Player(int id, string username) : base(id, username, 0, 0) {

			Inventory = new Inventory();
			// temp test
			Inventory.Add(new Ore(1000, "Copper Ore", 10, 20.5, "It's copper what else is there to say", 0.67335, 1085));
		}

		public override string ToString() {
			return base.ToString();
		}

		public override void update() {
			base.update();
			Inventory.update();
        }
	}
}
