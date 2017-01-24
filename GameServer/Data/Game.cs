using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;

namespace GameServer.Data
{

    public class Game
    {
        public LPlayer Players { get; private set; }
        public LGameObject GameObjects { get; private set; }


        public Game()
        {
            Players = new LPlayer();
            GameObjects = new LGameObject();
            //GameObjects.Add(new Kiln(5, 10)); // testing
        }

        public void update()
        {
            Players.update();
            GameObjects.update();


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
