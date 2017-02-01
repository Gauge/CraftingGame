using GameServer.Data.Interactables.Missiles;
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

        private bool MoveChange { get; set; }
        private int _attackCoolDown { get; set; }

        public List<bool> Moves { get; protected set; }
        public StatSheet Stats { get; protected set; }

        public bool IsMoveing
        {
            get
            {
                return Moves[0] || Moves[1] || Moves[2] || Moves[3];
            }
        }

        public bool CanAttack
        {
            get { return _attackCoolDown <= 0; }
        }

        public int AttackCoolDown
        {
            get { return _attackCoolDown <= 0 ? 0 : _attackCoolDown; }
        }

        public Pawn(int id, string name, double x, double y, int width, int height) : base(id, name, x, y, width, height)
        {
            Observers = new List<Player>();
            MoveChange = false;
            Moves = new List<bool> { false, false, false, false };
            Stats = new StatSheet();
            Stats.Health = 100;
            Stats.Damage = 0;
            Stats.MoveSpeed = 750;
            _attackCoolDown = 0;
        }

        public bool setMove(int d, bool isComplete)
        {
            //Console.WriteLine(d + " " + ((d == UP && d == LEFT) ? d + 1 : d - 1));
            // this oppositeMoveIndex ensures RIGHT and LEFT or UP and DOWN can not activate at the same time
            int oppositeMoveIndex = ((d == UP || d == LEFT) ? d + 1 : d - 1);
            if (!isComplete && !Moves[oppositeMoveIndex])
            {
                if (Moves[d] != true)
                {
                    MoveChange = true;
                }

                return Moves[d] = true;
            }
            else if (isComplete)
            {
                if (Moves[d] != false)
                {
                    MoveChange = true;
                }

                Moves[d] = false;
                return true;
            }
            return false;
        }

        public Missile generateAttack(Pawn t)
        {
            if (CanAttack)
            {
                _attackCoolDown = Stats.AttackSpeed;
                return new Missile(this, t);

            }
            return null;
        }

        public override void update(Game game)
        {
            base.update(game);
            double delta = Helper.getDelta();
            double amountToMove = (((Stats.MoveSpeed / SPEED_PER_UNIT) * delta) / 1000);

            if (Moves[UP])
            {
                Y = Math.Round(Y - amountToMove, 3);
            }

            if (Moves[DOWN])
            {
                Y = Math.Round(Y + amountToMove, 3);
            }

            if (Moves[LEFT])
            {
                X = Math.Round(X - amountToMove, 3);
            }

            if (Moves[RIGHT])
            {
                X = Math.Round(X + amountToMove, 3);
            }

            if (MoveChange)
            {
                MoveChange = false;

                game.Actions.Enqueue(new Action(this, ActionType.MOVE_STATE_CHANGED));
            }
        }
    }
}
