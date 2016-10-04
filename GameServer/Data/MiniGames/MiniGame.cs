using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data {
	public interface IMiniGame {

		void start();
		void stop();
		void terminate();
		void update();

		void itemAdd(int index, Item item);
		void itemRemove(int index);
		void itemRemove(Item item);
		//Item itemSplit(int index);
		//Item itemCombine(int index, Item item);

	}
}
