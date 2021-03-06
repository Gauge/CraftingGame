﻿namespace GameServer.Data.Interactables.Bunkers
{
    public class Bunker : Pawn
    {

        public const int VISION_RADIOUS = 32;
        public const int MIN_BUILD_RADIOUS = 4;

        public int OwnerID { get; }
        public int Threat { get; set; }
        public double Influence { get; set; } // the radious a units threat reaches


        public Bunker(int playerID, int x, int y) : base(100, "Turret", x, y, 2, 2)
        {
            OwnerID = playerID;
            Influence = 100;
            Threat = 50;

            Stats = new StatSheet();
            Stats.Health = 5000;
            Stats.Damage = 0;
            Stats.MoveSpeed = 0;
        }

        public bool isInNoBuildZone(double a, double b)
        {
            return isInRadious(a, b, MIN_BUILD_RADIOUS);
        }

        public bool isInInfluenceRadious(double a, double b)
        {
            return isInRadious(a, b, Influence);
        }

        public override void update(Game game)
        {
            base.update(game);
        }
    }
}
