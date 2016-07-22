using System.Collections.Generic;

namespace GameServer {

	public abstract class Item {
		public const double MAX_WEIGHT = 100; // weight is in kg

		public int id { get; }
		public string name { get; }
		public int count { get; }
		public double weight { get; }
		public int value { get; }
		public string description { get; }
		public bool isStackable { get; }

		public Item(int id, string name, double weight, int value, string description, bool stackable, int count = 1) {
			this.id = id;
			this.name = name;
			this.value = value;
			this.weight = weight;
			this.description = description;
			isStackable = stackable;
		}
	}

	public class Ore : Item {
		private double _weight;
		public new double weight { get { return _weight; } }

		private double _purity; // this is the % of pure metal
		public double purity { get { return _purity; } }

		public Ore(int id, string name, int value, double weight, string description, double purity) : base(id, name, weight, value, description, false) {
			_purity = purity;
			_weight = weight;
		}

		public bool add(double weight, double purity) {
			if (weight > 0 && purity >= 0 && purity <= 1) {
				_purity = weight / (_weight + weight) * _purity;
				_weight += weight;
				return true;
			}
			return false;
		}

		public bool remove(double weight) {
			if (weight >= 0 && _weight >= weight) {
				_weight -= weight;
				return true;
			}
			return false;
		}
		
	}

	public class Player {
		public static int movementSpeed = 750; // this is temp till i get other matter settled with
		public static int speedPerUnit = 100;

		public int id { get; set; }
		public string username { get; set; }
		public double x { get; set; }
		public double y { get; set; }

		private Item[] _inventory = new Item[30];
		private Item[] Inventory { get { return _inventory; } }


		public Player(string u) {
			id = idGenerator();
			username = u;
			x = 0;
			y = 0;

			// temp test
			_inventory[0] = new Ore(1, "Copper Ore", 10, 20.5, "It's copper what else is there to say", 0.67335);
		}

		public override string ToString() {
			return "ID: " + id + " Username: " + username + " Location: " + x + ":" + y;
		}

		private static int idGeneration = 0;
		private static int idGenerator() {
			return ++idGeneration;
		}
	}
}
