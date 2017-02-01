using GameServer.Data.Interactables;
using GameServer.Data.Interactables.Missiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public class StatSheet
    {

        public int Health { get; set; }
        public int Damage { get; set; }
        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; } // in ms

        public StatSheet()
        {
            Health = 0;
            Damage = 0;
            MoveSpeed = 0;
            AttackSpeed = 0;
        }

        public int CalculateDamage(StatSheet stats)
        {
            return CalculateDamage(stats.Damage);
        }

        public int CalculateDamage(Pawn p)
        {
            return CalculateDamage(p.Stats.Damage);
        }

        public int CalculateDamage(int damage)
        {
            return damage;
        }

        public void ApplyDamate(int damage)
        {
            Health -= CalculateDamage(damage);
        }
    }
}
