using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServer.Tests {
	[TestClass()]
	public class PingTests {
		[TestMethod()]
		public void PingTest() {
			Ping p = new Ping(3);

			Assert.AreEqual(3, p.id);
			Assert.AreEqual(ComType.Ping, p.type);
			Assert.IsTrue(p.timestamp > 0);
		}

		[TestMethod()]
		public void PingSerializtion() {
			Ping p = new Ping(6);

			Ping p2 = (Ping)BaseCommand.fromByteArray(p.toByteArray());

			Assert.AreEqual(p2.id, p.id);
			Assert.AreEqual(p2.type, p.type);
			Assert.AreEqual(p2.timestamp, p.timestamp);

		}
	}
}