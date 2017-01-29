using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables
{
    public class GameObjects : List<GameObject>
    {

        public List<GameObject> getGameObjectsInRange(Player p)
        {
            return getGameObjectsInRange(p.X, p.Y);
        }

        public List<GameObject> getGameObjectsInRange(double x, double y)
        {
            return this.FindAll(i => i.isInActionRange(x, y));
        }

        public List<GameObject> getGameObjectsOverlapping(double x, double y, int width, int height)
        {
            return this.FindAll(i => (i.X >= x && i.X <= (x + width)) && (i.Y >= y && i.Y <= (y + height)));
        }

        public void update(Game game)
        {
            foreach (GameObject i in this)
            {
                i.update(game);
            }
        }
    }
}
