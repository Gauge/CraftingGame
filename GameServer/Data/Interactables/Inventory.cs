using GameServer.Data.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameServer.Data.Interactables {
	public class Inventory : List<Item> {

		public int Size { get; }

		[JsonConstructor()]
		public Inventory() {
			Size = 30;
        }

		public List<Item> getArray() {
			return this.GetRange(0, this.Count);
		}

		public void initialize(List<Item> items) {
			this.RemoveRange(0, this.Count);
			this.AddRange(items);
		}

		private void resizeInventory() {
			for (int i = this.Count; i < Size; i++) {
				this.Add(null);
			}
		}

		public void add(int index, Item item) {
			if (index >= 0 && index <= Size && this[index] == null) {
				this[index] = item;
			}
		}

		public void remove(Item item) {
			remove(this.IndexOf(item));
		}

		public void remove(int index) {
			this.RemoveAt(index);
		}

		public void move(int index1, int index2) {
			Item I1 = this[index1];
			Item I2 = this[index2];
			this[index1] = I2;
			this[index2] = I1;
		}

		public void combine() {
		}

		public void split() {
		}

		public void update() {
			resizeInventory();
		}

		/*public void moveCombineItem(int index1, int index2) {
			resizeInventory();
			// this will need to change when more than ore is added
			Item item1 = this[index1];
			Item item2 = this[index2];

			if (item1 != null && item2 != null && item1.id == item2.id) {

				if (item1.GetType().ToString() == "Ore") {
					this[index1] = ((Ore)item2).add(((Ore)item1).weight, ((Ore)item2).purity);
				}
			} else {
				// switch locations if they arn't compatable
				this[index1] = item2;
				this[index2] = item1;
			}
		}*/
	}
}
