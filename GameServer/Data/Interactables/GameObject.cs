using System;

namespace GameServer.Data.Interactables {


	public class GameObject {

		public int id { get; }
		public string name { get; }
		public double x { get; set; }
		public double y { get; set; }
        public double actionRadious { get; }

		public GameObject(int id, string name, int x, int y, int actionRadious=5) {
			this.id = id;
			this.name = name;
			this.x = x;
			this.y = y;

			this.actionRadious = actionRadious;
		}

		public virtual void interact() { }
		public virtual void update() { }

		public bool isInRange(Player p) {
			return isInRange(p.x, p.y);
		}

		public bool isInRange(double x, double y) {
			// Pythagoras
			return Math.Pow((x - this.x), 2) + Math.Pow((y - this.y), 2) < actionRadious;

		}
	}
}
