using System;
using System.Collections.Generic;

namespace GameServer.Data.Interactables
{
    public class Pawn : GameObject
    {
        public const int UP = 0;
        public const int DOWN = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;

        public const int movementSpeed = 750; // this is temp till i get other matter settled with
        public const int speedPerUnit = 100;
        public List<bool> Moves { get; private set; }


        public Pawn(int id, string name, double x, double y) : base(id, name, x, y)
        {
            Moves = new List<bool> { false, false, false, false };
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

        public virtual void update()
        {
            double delta = Helper.getDelta();
            double amountToMove = (((movementSpeed / speedPerUnit) * delta) / 1000);

            if (Moves[UP])
            {
                y -= amountToMove;
            }

            if (Moves[DOWN])
            {
                y += amountToMove;
            }

            if (Moves[LEFT])
            {
                x -= amountToMove;
            }

            if (Moves[RIGHT])
            {
                x += amountToMove;
            }
        }
    }
}
