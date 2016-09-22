namespace GameServer.Data {
	public class Item {
		public const double MAX_WEIGHT = 100; // weight is in kg

		public int id { get; }
		public string name { get; }
		public int count { get; }
		public double weight { get; }
		public int value { get; }
		public string description { get; }
		public bool isStackable { get; }

		public Item(int id, string name, double weight, int value, string description, bool stackable, int count = 1) {
			this.id = id;
			this.name = name;
			this.count = count;
			this.value = (value > 0) ? value : 0;
			this.weight = weight;
			this.description = description;
			isStackable = stackable;
		}
	}
}
