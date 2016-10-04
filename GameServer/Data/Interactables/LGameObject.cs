using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables {
	public class LGameObject : List<GameObject> {

		public List<GameObject> getGameObjectsInRange(Player p) {
			return getGameObjectsInRange(p.x, p.y);
		}

		public List<GameObject> getGameObjectsInRange(double x, double y) {
			return this.FindAll(i => i.isInRange(x, y));
		}

	}
}
