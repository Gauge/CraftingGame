using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public class StatSheet
    {

        public int Heath { get; set; }
        public int Damage { get; set; }
        public int MoveSpeed { get; set; }

        public int CalculateDamageDelt(StatSheet stats)
        {
            return stats.Damage;
        }

    }
}
