namespace GameServer.Data.Interactables
{
    public class Player : Pawn
    {

        public const int VISION_RADIOUS = 16;

        public int PlayerID { get; }

        public Player(int id, string username) : base(1, username, 50, 50, 1, 1)
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
