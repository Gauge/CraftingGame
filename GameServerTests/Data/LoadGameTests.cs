using GameServer.Data;
using GameServer.Data.Interactables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GameServer.Tests {
	[TestClass()]
	public class LoadGameTests {
		[TestMethod()]
		public void LoadGameTest() {
			LoadGame lg = new LoadGame(3, new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Jim")
			});

			Assert.AreEqual(3, lg.id);
			Assert.AreEqual(3, lg.players.Count);
			Assert.AreEqual("Bob", lg.players[0].name);
			Assert.AreEqual("Ted", lg.players[1].name);
			Assert.AreEqual("Jim", lg.players[2].name);
		}

		[TestMethod()]
		public void LoadGameSerialization() {
			LoadGame lg = new LoadGame(3, new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Jim")
			});

			LoadGame lg2 = (LoadGame)BaseCommand.fromByteArray(lg.toByteArray());

			Assert.AreEqual(lg2.id, lg.id);
			Assert.AreEqual(lg2.players.Count, lg.players.Count);
			Assert.AreEqual(lg2.players[0].name, lg.players[0].name);
			Assert.AreEqual(lg2.players[1].name, lg.players[1].name);
			Assert.AreEqual(lg2.players[2].name, lg.players[2].name);
		}
	}
}