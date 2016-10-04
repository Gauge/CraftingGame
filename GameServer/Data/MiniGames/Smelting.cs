using GameServer.Data.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Data.MiniGames {
	public class Smelting : IMiniGame {

		public enum STATE { Setup, Active, Complete } 

		public int maxTemp { get; }
		public int presetTemp { get; set; }
		public int currentTemp { get; private set; }
		public List<Ore> Items { get; private set; }
		private bool activate { get; set; }
		public STATE state { get; private set; }

		public int points { get; private set; }
		public int slegRemoved { get; private set; }
		public int slegMissed { get; private set; }
		public int oreRemoved { get; private set; }

		private const int _tick = 100;
		private int _bucket { get; set; }
		private int _position { get; set; }
		private List<bool> _timeline { get; set; }


		public Smelting() {
			Items = new List<Ore>();
			Items.Add(null);
			Items.Add(null);
			Items.Add(null);
			Items.Add(null);
		}

		private double getPureity() {

			int count = 0;
			double purity = 0;
			foreach (Ore ore in Items) {
				if (ore != null) {
					purity += ore.purity;
					++count;
				}
			}
			return purity / count;
        }


		public void start() {
			// 3000 = 30 seconds
			int size = 3000; // timeline size

			_timeline.AddRange(Enumerable.Repeat(false, size));

			Random r = new Random(5476);
			int genPosition = 0;

			while (genPosition < _timeline.Count) {
				double rn = r.NextDouble();
				bool blockType = rn <= getPureity();
				int blockCount = r.Next(3, 10);

				for (int i = 1; i < blockCount; i++) {
					if (genPosition >= _timeline.Count) {
						break;
					}

					_timeline[genPosition] = blockType;
					genPosition++;
				}
			}
			state = STATE.Active;
		}

		public void stop() {
			state = STATE.Setup;
			_position = 0;
			activate = false;
			_timeline = null;
			slegRemoved = 0;
			slegMissed = 0;
			oreRemoved = 0;
		}

		public void update() {
			if (state == STATE.Active) {
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

		public void terminate() {}

		public void itemAdd(int index, Item item) {
			if (index >= 0 && index <= Items.Count) {
				if (Items[index] == null) {
					Items[index] = (Ore)item;
				} else {
					throw new Errors.MiniGameItemAddError(index);
				}
			}
		}

		public void itemRemove(int index) { }

		public void itemRemove(Item item) { }
	}
}
