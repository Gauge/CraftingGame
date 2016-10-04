using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Data.MiniGames;

namespace GameServer.Data.Interactables {

	public class Kiln : GameObject {

		public Smelting game { get; private set; }

		public Kiln(int x, int y) : base(2000, "Kiln", x, y) {
			game = new Smelting();
		}
	}
}
