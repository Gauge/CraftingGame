namespace GameServer.Data.Interactables.Missiles
{
    public class Missile : GameObject
    {
        public StatSheet Stats { get; }
        public double Target_X { get; private set; }
        public double Target_Y { get; private set; }


        public Missile(Pawn attacker, Pawn target) : base(500, "Missile", attacker.X, attacker.Y, 1)
        {
            Stats = new StatSheet();
            Stats.Damage = attacker.Stats.Damage;
            Stats.MoveSpeed = 2000;
            Target_X = target.X;
            Target_Y = target.Y;
        }

        public override void update(Game game)
        {
            base.update(game);
            double amountToMove = (((Stats.MoveSpeed / Pawn.SPEED_PER_UNIT) * Helper.getDelta()) / 1000);

            if (X < Target_X)
            {
                if (X - Target_X > amountToMove)
                {
                    X += amountToMove;
                }
                else
                {
                    X = Target_X;
                }
                
            }

            if (X > Target_X) {
                if (Target_X-X > amountToMove)
                {
                    X -= amountToMove;
                }
                else
                {
                    X = Target_X;
                }
            }

            if (Y < Target_Y)
            {
                if (Y - Target_Y > amountToMove)
                {
                    Y += amountToMove;
                }
                else
                {
                    Y = Target_Y;
                }

            }

            if (Y > Target_Y)
            {
                if (Target_Y - Y > amountToMove)
                {
                    Y -= amountToMove;
                }
                else
                {
                    Y = Target_Y;
                }
            }

            if (isInActionRange(Target_X, Target_Y))
            {
                game.Actions.Enqueue(new Action(this, ActionType.COMBAT));
                game.Missiles.removeGameObject(this);
            }
        }
    }
}
