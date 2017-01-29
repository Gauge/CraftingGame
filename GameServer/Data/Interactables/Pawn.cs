using System;
using System.Collections.Generic;

namespace GameServer.Data.Interactables
{
    public abstract class Pawn : GameObject
    {
        public const int UP = 0;
        public const int DOWN = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;
        public const int SPEED_PER_UNIT = 100;

        public List<bool> Moves { get; protected set; }
        public StatSheet Stats { get; protected set; }


        public Pawn(int id, string name, double x, double y) : base(id, name, x, y)
        {
            Moves = new List<bool> { false, false, false, false };
            Stats = new StatSheet();
            Stats.Heath = 100;
            Stats.Damage = 0;
            Stats.MoveSpeed = 750;
        }

        public bool setMove(int d, bool isComplete)
        {
            //Console.WriteLine(d + " " + ((d == UP && d == LEFT) ? d + 1 : d - 1));
            // this oppositeMoveIndex ensures RIGHT and LEFT or UP and DOWN can not activate at the same time
            int oppositeMoveIndex = ((d == UP || d == LEFT) ? d + 1 : d - 1);
            if (!isComplete && !Moves[oppositeMoveIndex])
            {
                return Moves[d] = true;
            }
            else if (isComplete)
            {
                Moves[d] = false;
                return true;
            }
            return false;
        }

        public override void update(Game game)
        {
            double delta = Helper.getDelta();
            double amountToMove = (((Stats.MoveSpeed / SPEED_PER_UNIT) * delta) / 1000);

            if (Moves[UP])
            {
                Y -= amountToMove;
            }

            if (Moves[DOWN])
            {
                Y += amountToMove;
            }

            if (Moves[LEFT])
            {
                X -= amountToMove;
            }

            if (Moves[RIGHT])
            {
                X += amountToMove;
            }
        }
    }
}
