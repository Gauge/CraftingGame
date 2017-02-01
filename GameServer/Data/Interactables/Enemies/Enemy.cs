using GameServer.Data.Interactables.Bunkers;

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

            foreach (Bunker t in game.Bunkers)
            {
                if (t.isInInfluenceRadious(X, Y))
                {
                    this.Threat.applyThreat(t, t.Threat);
                }
            }
            Threat.update();
            AI.BasicAI(game, this);
        }
    }
}
