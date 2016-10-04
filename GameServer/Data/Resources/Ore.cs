namespace GameServer.Data.Resources {
	public class Ore : Item {

		public int meltingPoint { get; private set; }

		private double _purity;
		public double purity {
			get { return _purity; }
			private set {
				if (value >= 0 && value <= 1) {
					_purity = value;
				}
			}
		}

		private double _weight;
		public new double weight {
			get { return _weight; }
			private set {
				if (value >= 0) {
					_weight = value;
				}
			}
		}

		// melting point celcius
		public Ore(int id, string name, int value, double weight, string description, double purity, int meltingPoint) : base(id, name, weight, value, description, false) {
			this.purity = purity;
			this.weight = weight;
			this.meltingPoint = meltingPoint;
		}

		public Ore add(double weight, double purity) {
			if (weight > 0 && purity >= 0 && purity <= 1) {
				this.purity = (this.purity + purity) / 2;
				this.weight = this.weight + weight;
			}
			// if the value is over max weight retrun what is left over
			if (weight > MAX_WEIGHT) {
				Ore remaining = new Ore(id, name, value, weight - MAX_WEIGHT, description, purity, meltingPoint);
				this.weight = MAX_WEIGHT;
				return remaining;
			}
			return null;
		}

		public bool remove(double weight) {
			if (weight >= 0 && this.weight >= weight) {
				this.weight -= weight;
				return true;
			}
			return false;
		}
	}
}
