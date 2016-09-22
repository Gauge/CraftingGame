using System;
using System.Collections.Generic;

namespace GameServer.Data.Interactables {
	public class Controllable : Interactable {
		public const int movementSpeed = 750; // this is temp till i get other matter settled with
		public const int speedPerUnit = 100;
		public List<bool> Moves { get; private set; }


		public Controllable(int id, string name, int x, int y) : base(id, name, x, y) {
			Moves = new List<bool> { false, false, false, false };
		}

		public bool setMove(Direction d, bool isComplete) {
			Console.WriteLine((int)d + " " + (((int)d == 0 && (int)d == 2) ? (int)d + 1 : (int)d - 1));
			int index = (((int)d == 0 || (int)d == 2) ? (int)d + 1 : (int)d - 1); // this index ensures RIGHT and LEFT or UP and DOWN can not activate at the same time
			if (!isComplete && !Moves[index]) {
				return Moves[(int)d] = true;
			} else if (isComplete) {
				Moves[(int)d] = false;
				return true;
			}
			return false;
		}

		public void update() {
			double delta = Settings.getDelta();
			double amountToMove = (((movementSpeed / speedPerUnit) * delta) / 1000);

			if (Moves[0]) {
				y -= amountToMove;
			}

			if (Moves[1]) {
				y += amountToMove;
			}

			if (Moves[2]) {
				x -= amountToMove;
			}

			if (Moves[3]) {
				x += amountToMove;
			}
		}
	}
}
