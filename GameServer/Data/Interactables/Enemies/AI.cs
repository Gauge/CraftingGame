using GameServer.Data.Interactables.Missiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables.Enemies
{
    class AI
    {
        public static void BasicAI(Game game, Enemy unit)
        {
            if (unit.Threat.CurrentThreat != null)
            {
                Pawn target = (Pawn)unit.Threat.CurrentThreat.Unit;

                if (unit.isInActionRange(target))
                {
                    if (unit.IsMoveing)
                    {
                        StopMoving(unit);
                    }

                    
                    if (unit.CanAttack)
                    {
                        Missile m = unit.generateAttack(target);
                        game.Missiles.Add(m);
                        game.Actions.Enqueue(new Action(m, ActionType.ENTER_PLAYER_VISION));
                    }
                }
                else
                {
                    SomeDayAPlusPathing(unit);
                }
            }
        }

        private static void StopMoving(Enemy unit)
        {
            if (unit.Moves[Pawn.UP])
            {
                unit.setMove(Pawn.UP, true);
            }

            if (unit.Moves[Pawn.DOWN])
            {
                unit.setMove(Pawn.DOWN, true);
            }

            if (unit.Moves[Pawn.LEFT])
            {
                unit.setMove(Pawn.LEFT, true);
            }

            if (unit.Moves[Pawn.RIGHT])
            {
                unit.setMove(Pawn.RIGHT, true);
            }
        }

        private static void SomeDayAPlusPathing(Enemy unit)
        {

            if (unit.Threat.CurrentThreat != null)
            {
                GameObject destination = unit.Threat.CurrentThreat.Unit;
                if (unit.X - destination.X < 0.001 && unit.X - destination.X > -0.001 && (unit.Moves[Pawn.LEFT] || unit.Moves[Pawn.RIGHT]))
                {
                    unit.setMove(Pawn.LEFT, true);
                    unit.setMove(Pawn.RIGHT, true);
                }

                if (unit.Y - destination.Y < 0.001 && unit.Y - destination.Y > -0.001 && (unit.Moves[Pawn.UP] || unit.Moves[Pawn.DOWN]))
                {
                    unit.setMove(Pawn.UP, true);
                    unit.setMove(Pawn.DOWN, true);
                }


                if (!(unit.X - destination.X < 0.001 && unit.X - destination.X > -0.001))
                {
                    if (unit.X - destination.X < 0 && !unit.Moves[Pawn.RIGHT])
                    {
                        unit.setMove(Pawn.RIGHT, false);
                    }
                    else if (unit.X - destination.X > 0 && !unit.Moves[Pawn.LEFT])
                    {
                        unit.setMove(Pawn.LEFT, false);
                    }
                }

                if (!(unit.Y - destination.Y < 0.001 && unit.Y - destination.Y > -0.001))
                {
                    if (unit.Y - destination.Y < 0 && !unit.Moves[Pawn.DOWN])
                    {
                        unit.setMove(Pawn.DOWN, false);
                    }
                    else if (unit.Y - destination.Y > 0 && !unit.Moves[Pawn.UP])
                    {
                        unit.setMove(Pawn.UP, false);
                    }
                }
            }
        }
    }
}
