using GameServer.Data.Interactables.Tower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables.Enemies
{
    public abstract class Enemy : Pawn
    {

        public Threats Threat { get; private set; }

        public Enemy(int id, string name, double x, double y) : base(id, name, x, y)
        {
            Threat = new Threats();
        }

        public override void update(Game game)
        {
            base.update(game);

            foreach (Turret t in game.Turrets)
            {
                this.Threat.applyThreat(t, t.Threat);
            }


            Threat.update();

            // basic AI
            if (Threat.CurrentThreat != null)
            {
                GameObject destination = Threat.CurrentThreat.Unit;
                if (Math.Abs(X - destination.X) < 0.5 && (Moves[LEFT] || Moves[RIGHT]))
                {
                    setMove(LEFT, true);
                    setMove(RIGHT, true);
                }

                if (Math.Abs(Y - destination.Y) < 0.5 && (Moves[UP] || Moves[DOWN]))
                {
                    setMove(UP, true);
                    setMove(DOWN, true);
                }


                if (X - destination.X < 0)
                {
                    setMove(RIGHT, false);
                }
                else if (X - destination.X > 0)
                {
                    setMove(LEFT, false);
                }

                if (Y - destination.Y < 0)
                {
                    setMove(DOWN, false);
                }
                else if (Y - destination.Y > 0)
                {
                    setMove(LEFT, false);
                }
            }

        }
    }
}
