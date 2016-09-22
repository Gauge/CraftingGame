using GameServer.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServer.Tests {
	[TestClass()]
	public class MoveTests {
		[TestMethod()]
		public void MoveTest() {
			Move m = new Move(3, Direction.Up, false);

			Assert.AreEqual(3, m.id);
			Assert.AreEqual(Direction.Up, m.direction);
			Assert.AreEqual(false, m.isComplete);
			Assert.AreEqual(0, m.x);
			Assert.AreEqual(0, m.y);

			m = new Move(400, Direction.Down, true, 5, 4.54);

			Assert.AreEqual(400, m.id);
			Assert.AreEqual(Direction.Down, m.direction);
			Assert.AreEqual(true, m.isComplete);
			Assert.AreEqual(5, m.x);
			Assert.AreEqual(4.54, m.y);
		}

		[TestMethod()]
		public void MoveSerializtion() {
			Move m = new Move(400, Direction.Left, true, 5, 4.54);

			Move m2 = (Move)BaseCommand.fromByteArray(m.toByteArray());

			Assert.IsNotNull(m2);
			Assert.AreEqual(m.id, m2.id);
			Assert.AreEqual(m.direction, m2.direction);
			Assert.AreEqual(m.isComplete, m2.isComplete);
			Assert.AreEqual(m.x, m2.x);
			Assert.AreEqual(m.y, m2.y);
			Assert.AreEqual(m.timestamp, m2.timestamp);
		}
	}
}