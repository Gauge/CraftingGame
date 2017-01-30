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

        public Enemy(int id, string name, double x, double y, int width, int height) : base(id, name, x, y, width, height)
        {
            Threat = new Threats();
        }

        public override void update(Game game)
        {
            base.update(game);

            foreach (Turret t in game.Turrets)
            {
                if (t.isInInfluenceRadious(X, Y))
                {
                    this.Threat.applyThreat(t, t.Threat);
                }
            }


            Threat.update();

            // basic AI
            if (Threat.CurrentThreat != null)
            {
                GameObject destination = Threat.CurrentThreat.Unit;
                if (X - destination.X < 0.001 && X - destination.X > -0.001 && (Moves[LEFT] || Moves[RIGHT]))
                {
                    setMove(LEFT, true);
                    setMove(RIGHT, true);
                }

                if (Y - destination.Y < 0.001 && Y - destination.Y > -0.001 && (Moves[UP] || Moves[DOWN]))
                {
                    setMove(UP, true);
                    setMove(DOWN, true);
                }


                if (!(X - destination.X < 0.001 && X - destination.X > -0.001))
                {
                    if (X - destination.X < 0 && !Moves[RIGHT])
                    {
                        setMove(RIGHT, false);
                    }
                    else if (X - destination.X > 0 && !Moves[LEFT])
                    {
                        setMove(LEFT, false);
                    }
                }

                if (!(Y - destination.Y < 0.001 && Y - destination.Y > -0.001))
                {
                    if (Y - destination.Y < 0 && !Moves[DOWN])
                    {
                        setMove(DOWN, false);
                    }
                    else if (Y - destination.Y > 0 && !Moves[UP])
                    {
                        setMove(UP, false);
                    }
                }
            }

        }
    }
}
