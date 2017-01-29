using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;
using GameServer.Data.MapData;
using GameServer.Data.Interactables.Tower;

namespace GameServer.Data
{
    public class Game
    {
        public Players Players { get; private set; }
        public GameObjects GameObjects { get; private set; }
        public Turrets Turrets { get; private set; }
        public Map Map { get; private set; }


        public Game()
        {
            Players = new Players();
            GameObjects = new GameObjects();
            Turrets = new Turrets();
            Map = new Map();
            GameObjects.AddRange(Map.seedBarbarians());
        }

        public bool placeTurret(int playerID, int x, int y, out string errorMessage)
        {
            errorMessage = "";

            if (Turrets.getTurretByPlayerID(playerID) == null)
            {
                Turret t = new Turret(playerID, x, y);
                if (Map.verifyPlacement(t) && GameObjects.getGameObjectsOverlapping(t.X, t.Y, t.Width, t.Height).Count == 0)
                {
                    if (!Turrets.isInNoBuildZone(t.X, t.Y))
                    {
                        Turrets.Add(t);
                        return true;
                    }
                    else
                    {
                        errorMessage = "To close to another tower";
                    }
                }
                else
                {
                    errorMessage = "Could not place. Something is in the way";
                }
            }
            else
            {
                errorMessage = "You already have a turret placed";
            }
            return false;
        }

        public List<GameObject> getPlayerGameState(int playerID)
        {
            List<GameObject> result = new List<GameObject>();
            Player player = Players.getPlayerById(playerID);
            Turret turret = Turrets.getTurretByPlayerID(playerID);

            // vision for player
            result.AddRange(Players.FindAll(p => p.isInRadious(player.X, player.Y, Player.VISION_RADIOUS)));
            result.AddRange(GameObjects.FindAll(o => o.isInRadious(player.X, player.Y, Player.VISION_RADIOUS)));
            result.AddRange(Turrets.FindAll(t => t.isInRadious(player.X, player.Y, Player.VISION_RADIOUS)));

            // vision for player turret
            result.AddRange(Players.FindAll(p => p.isInRadious(turret.X, turret.Y, Turret.VISION_RADIOUS)));
            result.AddRange(GameObjects.FindAll(o => o.isInRadious(turret.X, turret.Y, Turret.VISION_RADIOUS)));
            result.AddRange(Turrets.FindAll(t => t.isInRadious(turret.X, turret.Y, Turret.VISION_RADIOUS)));

            return result;
        }

        public void update()
        {
            Players.update(this);
            Turrets.update(this);
            GameObjects.update(this);
        }

        public override string ToString()
        {
            string output = string.Empty;

            output += "Players:\n";
            foreach (Player p in Players)
            {
                output += p.ToString() + "\n";
            }

            output += "\nGameObjects:\n";
            foreach (GameObject gO in GameObjects)
            {
                output += gO.ToString() + "\n";
            }

            return output;
        }
    }
}
