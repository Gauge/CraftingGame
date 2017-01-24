/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables {
	public interface IInventory {

		// Combine/Add Item return leftovers or the object if failed to add
		Item Add(int index, Item item);
		// Set the item to given index and returns the item in the slot before
		Item Replace(int index, Item item);
		// Perminently removes an item from slot
		void Remove(int index);
		void Remove(Item item);
		// swaps items location with another
		void Move(int index1, int index2);
		// places the item an empty inventory slot
		// if none exists return the item
		bool PlaceInEmptySlot(Item item);
		// get a list of all current items
		Item[] getItems();

	}
}*/
