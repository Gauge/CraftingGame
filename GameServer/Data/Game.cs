using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;

namespace GameServer.Data {

	public class Game {
		public LPlayer Players { get; private set; }
		public LGameObject GameObjects {get; private set;}


		public Game() {
			Players = new LPlayer();
			GameObjects = new LGameObject();
			GameObjects.Add(new Kiln(5, 10)); // testing
		}

		public void update() {
			foreach (Player p in Players) {
				p.update();
			}

			foreach (GameObject i in GameObjects) {
				i.update();
			}
		}

		public override string ToString() {
			return "Player Count: " + Players.Count + " Game Objects: " + GameObjects.Count;
		}
	}
}
