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
        private bool MoveChange { get; set; }
        private bool observerSetForUpdate { get; set; }
        public List<Player> Observers { get; set; }

        public bool IsMoveing
        {
            get
            {
                return Moves[0] || Moves[1] || Moves[2] || Moves[3];
            }
        }

        public Pawn(int id, string name, double x, double y, int width, int height) : base(id, name, x, y, width, height)
        {
            Observers = new List<Player>();
            observerSetForUpdate = false;
            MoveChange = false;
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

        public void updateObservers(List<Player> players) {
            Observers = players;
            observerSetForUpdate = false;
        }

        public override void update(Game game)
        {
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

            if (MoveChange) {
                MoveChange = false;

                game.Actions.Add(new Action(this, Action.ActionType.MOVE_STATE_CHANGED));
            }

            if (!observerSetForUpdate) {
                Action tempAction = new Action(this, Action.ActionType.ENTER_PLAYER_VISION);
                List<Player> observerList = tempAction.getEffectedPlayerIDs(game);

                foreach (Player observer in observerList) {
                    if (Observers.Find(o => o.PlayerID == observer.PlayerID) == null) {
                        game.Actions.Add(tempAction);
                        observerSetForUpdate = true;
                    }
                }
            }
        }
    }
}
