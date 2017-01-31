using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;
using GameServer.Data.MapData;
using GameServer.Data.Interactables.Bunkers;
using GameServer.Data.Interactables.Enemies;

namespace GameServer.Data
{
    public class Game
    {
        public Players Players { get; private set; }
        public GameObjects Enemies { get; private set; }
        public Bunkers Turrets { get; private set; }
        public Map Map { get; private set; }
        public List<Action> Actions { get; private set; }


        public Game()
        {
            Actions = new List<Action>();
            Players = new Players();
            Enemies = new GameObjects();
            Turrets = new Bunkers();
            Map = new Map();
            Enemies.AddRange(Map.seedBarbarians());
        }

        public bool placeBunker(int playerID, int x, int y, out string errorMessage)
        {
            errorMessage = "";

            if (Turrets.getBunkerByPlayerID(playerID) == null)
            {
                Bunker t = new Bunker(playerID, x, y);
                if (Map.verifyPlacement(t) && Enemies.getGameObjectsOverlapping(t.X, t.Y, t.Width, t.Height).Count == 0)
                {
                    if (!Turrets.isInNoBuildZone(t.X, t.Y))
                    {
                        Turrets.Add(t);
                        Actions.Add(new Action(t, Action.ActionType.MOVE_STATE_CHANGED));
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

        public void update()
        {
            Players.update(this);
            Turrets.update(this);
            Enemies.update(this);
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
            foreach (GameObject gO in Enemies)
            {
                output += gO.ToString() + "\n";
            }

            return output;
        }
    }
}
