using System;

namespace GameServer.Data.Interactables {


	public class Interactable {

		public int id { get; }
		public string name { get; }
		public double x { get; set; }
		public double y { get; set; }
        public double actionRadious { get; }

		public Interactable(int id, string name, int x, int y, int actionRadious=5) {
			this.id = id;
			this.x = x;
			this.y = y;

			this.actionRadious = actionRadious;
		}

		public virtual void update() { }

		public bool isInRange(Player p) {
			// Pythagoras
			return Math.Pow((p.x - x), 2) + Math.Pow((p.y - y), 2) < actionRadious;

		}
	}
}
