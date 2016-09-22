using GameServer.Data.Resources;
using System;

namespace GameServer.Data.Interactables {
	public class Smelters : Interactable {


		public int maxTemp { get; }
		public int presetTemp { get; set; }
		public int currentTemp { get; private set; }
		public Ore ore { get; set; }
		private bool activate { get; set; }
		public MG_Type_Smelting state { get; private set; }

		public int points { get; private set; }
		public int slegRemoved { get; private set; }
		public int slegMissed { get; private set; }
		public int oreRemoved { get; private set; }

		private const int _tick = 100;
		private int _bucket { get; set; }
		private int _position { get; set; }
		private bool[] _timeline { get; set; }


		public Smelters(int maxTemp, int id, string name, int x, int y, int actionRadious = 5) : base(id, name, x, y, actionRadious) {
			this.maxTemp = maxTemp;
			reset();
		}

		public void startGame() {
			// 3000 = 30 seconds
			int size = 3000; // timeline size
			_timeline = new bool[size];

			Random r = new Random(5476);
			int genPosition = 0;

			while (genPosition < _timeline.Length) {
				double rn = r.NextDouble();
				bool blockType = rn <= ore.purity; // if the number is
				int blockCount = r.Next(3, 10);

				for (int i = 1; i < blockCount; i++) {
					if (genPosition >= _timeline.Length) {
						break;
					}

					_timeline[genPosition] = blockType;
					genPosition++;
				}
			}
			state = MG_Type_Smelting.Active;
		}

		private void reset() {
			state = MG_Type_Smelting.Setup;
			_position = 0;
			activate = false;
			_timeline = null;
			slegRemoved = 0;
			slegMissed = 0;
			oreRemoved = 0;

        }

		public override void update() {
			if (state == MG_Type_Smelting.Active) {
				int delta = (int)Settings.getDelta();

				while (delta > 0) {
					bool shouldBeActive = _timeline[_position];
					int leftovers = _bucket + delta - _tick;
					int pointsToCalculateNow = delta - leftovers;

					if (leftovers > 0) {
						if (activate && shouldBeActive) {
							slegRemoved += pointsToCalculateNow;
							points += pointsToCalculateNow;
						} else if (activate && !shouldBeActive) {
							oreRemoved += pointsToCalculateNow;
							points -= pointsToCalculateNow;
						} else if (!activate && shouldBeActive) {
							slegMissed += pointsToCalculateNow;
							points -= pointsToCalculateNow;
						} else {
							points += pointsToCalculateNow;
						}
						_position++;

					} else {
						if (activate && shouldBeActive) {
							slegRemoved += delta;
							points += delta;
						} else if (activate && !shouldBeActive) {
							oreRemoved += delta;
							points -= delta;
						} else if (!activate && shouldBeActive) {
							slegMissed += delta;
							points -= delta;
						} else {
							points += delta;
						}
					}
					delta = leftovers;
				}
			}
		}
	}
}
