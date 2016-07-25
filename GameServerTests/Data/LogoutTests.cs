using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServer.Tests {
	[TestClass()]
	public class LogoutTests {
		[TestMethod()]
		public void LogoutTest() {
			Logout lout = new Logout();
			Logout lout2 = new Logout(43, 57897835443);

			Assert.AreEqual(-1, lout.id);
			Assert.AreEqual(43, lout2.id);
			Assert.AreEqual(57897835443, lout2.timestamp);
		}

		[TestMethod()]
		public void LogoutSerialization() {
			Logout lout = new Logout(3);

			Logout lout2 = (Logout)BaseCommand.fromByteArray(lout.toByteArray());

			Assert.IsNotNull(lout2);
			Assert.AreEqual(lout.id, lout2.id);
			Assert.AreEqual(lout.timestamp, lout2.timestamp);
		}
	}
}