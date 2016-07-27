using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Threading;

namespace GameServer.Tests {
	[TestClass()]
	public class LoginTests {
		[TestMethod()]
		public void LoginTest() {
			Login l = new Login("Bob");
			Thread.Sleep(1000);
			Login l2 = new Login("Ted");

			Assert.AreEqual(l.type, ComType.Login);
			Assert.AreEqual(l.username, "Bob");
			Assert.AreEqual(l.id, -1);
			Assert.IsNull(l.player);
			Assert.AreNotEqual(l.timestamp, 0);
			Assert.IsTrue(l.timestamp < l2.timestamp);
		}

		[TestMethod()]
		public void LoginTest2() {
			Player p = new Player(1, "Bob");
			Login l = new Login(p.id, p);
			Thread.Sleep(1000);
			Login l2 = new Login("Ted");

			Assert.AreEqual(l.type, ComType.Login);
			Assert.AreEqual(l.id, p.id);
			Assert.IsNull(l.username);
			Assert.AreEqual(l.player, p);
			Assert.AreNotEqual(l.timestamp, 0);
			Assert.IsTrue(l.timestamp < l2.timestamp, l.timestamp + " " + l2.timestamp);
		}

		[TestMethod()]
		public void LoginSerialization() {
			Login l = new Login("Bob");
			Login output = JsonConvert.DeserializeObject<Login>(JsonConvert.SerializeObject(l));

			Assert.IsNotNull(output);
			Assert.AreEqual(output.type, l.type);
			Assert.AreEqual(output.id, l.id);
			Assert.AreEqual(output.username, l.username);
			Assert.AreEqual(output.player, l.player);
			Assert.AreEqual(output.timestamp, l.timestamp);

		}

		[TestMethod()]
		public void LoginSerialization2() {
			Player p = new Player(1, "Bob");
			Login l = new Login(p.id, p);
			Login output = JsonConvert.DeserializeObject<Login>(JsonConvert.SerializeObject(l));

			Assert.IsNotNull(output);
			Assert.AreEqual(output.type, l.type);
			Assert.AreEqual(output.id, l.id);
			Assert.AreEqual(output.username, l.username);
			Assert.AreEqual(output.player.id, l.player.id);
			Assert.AreEqual(output.player.username, l.player.username);
			Assert.AreEqual(output.timestamp, l.timestamp);

		}
	}
}