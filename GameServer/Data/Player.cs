using System;
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
			this.count = count;
			this.value = (value > 0) ? value : 0;
			this.weight = weight;
			this.description = description;
			isStackable = stackable;
		}
	}

	public class Ore : Item {

		private double _purity;
		public double purity {
			get { return _purity; }
			private set {
				if (value >= 0 && value <= 1) {
					_purity = value;
				}
			}
		}

		private double _weight;
		public new double weight {
			get { return _weight; }
			private set {
				if (value >= 0) {
					_weight = value;
				}
			}
		}

		public Ore(int id, string name, int value, double weight, string description, double purity) : base(id, name, weight, value, description, false) {
			this.purity = purity;
			this.weight = weight;
		}

		public bool add(double weight, double purity) {
			if (weight > 0 && purity >= 0 && purity <= 1) {
				this.purity = (this.purity + purity) / 2;
				this.weight = this.weight + weight;
				return true;
			}
			return false;
		}

		public bool remove(double weight) {
			if (weight >= 0 && this.weight >= weight) {
				this.weight -= weight;
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
		public List<bool> Moves { get; private set; }

		//private Item[] _inventory = new Item[30];
		//public Item[] Inventory { get { return _inventory; } }


		public Player(int id, string username) {
			this.id = id;
			this.username = username;
			x = 0;
			y = 0;
			Moves = new List<bool> { false, false, false, false };

			// temp test
			//_inventory[0] = new Ore(1, "Copper Ore", 10, 20.5, "It's copper what else is there to say", 0.67335);
		}

		public bool setMove(Direction d, bool isComplete) {
			Console.WriteLine((int)d + " " + (((int)d == 0 && (int)d == 2) ? (int)d + 1 : (int)d - 1));
 			int index = (((int)d == 0 || (int)d == 2) ? (int)d+1 : (int)d-1); // this index ensures RIGHT and LEFT or UP and DOWN can not activate at the same time
			if (!isComplete && !Moves[index]) {
				return Moves[(int)d] = true;
			} else if (isComplete) {
				Moves[(int)d] = false;
				return true;
			}
			return false;
		}

		public override string ToString() {
			return "ID: " + id + " Username: " + username + " Location: " + x + ":" + y;
		}
	}
}
