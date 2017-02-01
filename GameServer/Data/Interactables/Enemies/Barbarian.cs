using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables.Enemies
{
    class Barbarian : Enemy
    {
        public Barbarian(double x, double y) : base(1000, "Barbarian", x, y, 1, 1)
        {
            Stats.Health = 500;
            Stats.MoveSpeed = 500;
            Stats.Damage = 100;
            Stats.AttackSpeed = 5000;
        }
    }
}
