using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables.Bunkers
{
    public class Bunkers : List<Bunker>
    {

        public Bunker getBunkerByPlayerID(int playerID)
        {
            return this.Find(i => i.OwnerID == playerID);
        }

        public void removeBunkerByPlayerID(int playerID) {
            this.RemoveAll(i => i.OwnerID == playerID);
        }

        public bool isInNoBuildZone(double x, double y)
        {
            return this.Find(i => i.isInNoBuildZone(x, y)) != null;
        }

        public void update(Game game)
        {
            foreach (Bunker t in this)
            {
                t.update(game);
            }
        }

    }
}
