using GameServer.Data.Interactables;
using GameServer.Data.Interactables.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.MapData
{
    public class Map
    {

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int EnemySeed { get; private set; }

        public Map()
        {
            Width = 1000;
            Height = 1000;
            EnemySeed = 10000;
        }

        public List<Enemy> seedBarbarians()
        {
            Random Rng = new Random(EnemySeed);
            List<Enemy> units = new List<Enemy>();

            double x;
            double y;
            for (int i = 0; i < EnemySeed; i++)
            {
                x = Width * Rng.NextDouble();
                y = Height * Rng.NextDouble();
                units.Add(new Barbarian(x, y));
            }

            return units;
        }

        public Enemy seedSingleBarbarian() {
            Random Rng = new Random(EnemySeed);
            return new Barbarian(Width * Rng.NextDouble(), Height * Rng.NextDouble());
        }

        public bool verifyPlacement(GameObject obj)
        {
            return true;
        }
    }
}
