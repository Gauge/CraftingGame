using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables {

	public class Kiln : Smelters {

		public Kiln(int x, int y) : base(2000, 2000, "Kiln", x, y) {
		}
	}
}
