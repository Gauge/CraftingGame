using GameServer.Data.Interactables;
using GameServer.Data.Interactables.Bunkers;
using System.Collections.Generic;

namespace GameServer.Data
{

    public enum ActionType { ENTER_PLAYER_VISION, MOVE_STATE_CHANGED, COMBAT }

    public class Action
    {
        public Action(GameObject issuer, ActionType type)
        {
            Issuer = issuer;
            Type = type;
        }

        public GameObject Issuer { get; }
        public ActionType Type { get; }

        public List<Player> getEffectedPlayerIDs(Game game)
        {
            double x = Issuer.X;
            double y = Issuer.Y;
            List<Player> result = new List<Player>();

            List<Player> players = game.Players.FindAll(p => p.isInRadious(x, y, Player.VISION_RADIOUS));
            List<Bunker> turrets = game.Bunkers.FindAll(p => p.isInRadious(x, y, Bunker.VISION_RADIOUS));

            foreach (Player p in players)
            {
                result.Add(p);
            }

            foreach (Bunker t in turrets)
            {
                if (result.FindIndex(r => r.PlayerID == t.OwnerID) == -1)
                {
                    result.Add(game.Players.getPlayerById(t.OwnerID));
                }
            }
            return result;
        }
    }
}
