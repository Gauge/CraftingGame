using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables.Tower
{
    public class Turrets : List<Turret>
    {

        public Turret getTurretByPlayerID(int playerID)
        {
            return this.Find(i => i.OwnerID == playerID);
        }

        public void removeTurretByPlayerID(int playerID) {
            this.RemoveAll(i => i.OwnerID == playerID);
        }

        public bool isInNoBuildZone(double x, double y)
        {
            return this.Find(i => i.isInNoBuildZone(x, y)) != null;
        }

        public void update(Game game)
        {
            foreach (Turret t in this)
            {
                t.update(game);
            }
        }

    }
}
