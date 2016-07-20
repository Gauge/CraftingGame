namespace GameServer {
	public class Player {
		public static int movementSpeed = 750; // this is temp till i get other matter settled with
		public static int speedPerUnit = 100;

		public int id;
		public string username;
		public double x;
		public double y;

		public Player(string u) {
			id = idGenerator();
			username = u;
			x = 0;
			y = 0;
		}

		public override string ToString() {
			return "ID: " + id + " Username: " + username + " Location: " + x + ":" + y;
		}

		private static int idGeneration = 0;
		private static int idGenerator() {
			return ++idGeneration;
		}
	}
}
