using GameServer.Data.Interactables.Tower;
using System.Collections.Generic;
//using GameServer.Data.Resources;

namespace GameServer.Data.Interactables
{
    public class Player : Pawn
    {

        public const int VISION_RADIOUS = 16;

        public int PlayerID { get; }

        public Player(int id, string username) : base(1, username, 0, 0)
        {
            PlayerID = id;
        }


        public override void update(Game game)
        {
            base.update(game);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
