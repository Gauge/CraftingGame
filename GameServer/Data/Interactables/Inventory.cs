/*using GameServer.Data.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameServer.Data.Interactables {
	public class Inventory : IInventory {

		public const int SIZE = 30;
		private List<Item> _inventory;

		public int Size {
			get { return _inventory.Count; }
		}

		public Item this[int index] {
			get { return _inventory[index]; }
		}

		public Inventory() {
			_inventory = new List<Item>();
			for (int i = 0; i < SIZE; ++i) {
				_inventory.Add(null);
			}
        }

		public Item Add(int index, Item item) {
			if (index >= 0 && index <= Size) {
				if (_inventory[index] == null) {
					if (_inventory[index].isStackable && _inventory[index].id == item.id) {
						_inventory[index] = item;
					} else {
						return item;
					}
				} else {
					return _inventory[index].add(item);
				}
			} else {
				throw new IndexOutOfRangeException("Inventory item add at position: " + index);
			}
			return null;
		}

		public void Remove(Item item) {
			Remove(_inventory.IndexOf(item));
		}

		public void Remove(int index) {
			_inventory.RemoveAt(index);
		}

		public void Move(int index1, int index2) {
			Item I1 = _inventory[index1];
			Item I2 = _inventory[index2];
			_inventory[index1] = I2;
			_inventory[index2] = I1;
		}

		public Item Replace(int index, Item item) {
			Item itemToReturn = _inventory[index];
			_inventory[index] = item;
			return itemToReturn;
		}

		public bool PlaceInEmptySlot(Item item) {
			for (int i= 0; i< SIZE; ++i) {
				if (_inventory[i] == null) {
					_inventory[i] = item;
					return true;
				}
			}
			return false;
		}

		public Item[] getItems() {
			return _inventory.ToArray();
		}
	}
}*/
