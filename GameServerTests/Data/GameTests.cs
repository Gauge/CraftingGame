using GameServer.Data;
using GameServer.Data.Interactables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Tests {
	[TestClass()]
	public class GameTests {
		[TestMethod()]
		public void GameTest() {
			Game g = new Game();

			Assert.AreEqual(0, g.Players.Count);
		}

		[TestMethod()]
		public void addPlayerTest() {
			Game g = new Game();

			int id = g.addPlayer("Bob");
			Assert.AreEqual(1, id);
			Assert.AreEqual(1, g.Players.Count);
			Assert.AreEqual("Bob", g.Players[0].name);

			id = g.addPlayer("Bob");
			Assert.AreEqual(-1, id);
			Assert.AreEqual(1, g.Players.Count);

			id = g.addPlayer("Ted");
			Assert.AreEqual(3, id);
			Assert.AreEqual(2, g.Players.Count);
			Assert.AreEqual("Ted", g.Players[1].name);
		}

		[TestMethod()]
		public void addPlayerTest1() {
			Game g = new Game();

			Player p = new Player(1, "Bob");
			int id = g.addPlayer(p);
			Assert.AreEqual(1, id);
			Assert.AreEqual(1, g.Players.Count);
			Assert.AreEqual("Bob", g.Players[0].name);

			p = new Player(2, "Bob");
			id = g.addPlayer(p);
			Assert.AreEqual(-1, id);
			Assert.AreEqual(1, g.Players.Count);

			p = new Player(1, "Jim");
			id = g.addPlayer(p);
			Assert.AreEqual(-1, id);
			Assert.AreEqual(1, g.Players.Count);

			p = new Player(3, "Ted");
			id = g.addPlayer(p);
			Assert.AreEqual(3, id);
			Assert.AreEqual(2, g.Players.Count);
			Assert.AreEqual("Ted", g.Players[1].name);
		}

		[TestMethod()]
		public void removePlayerTest() {
			Game g = new Game();
			g.addPlayer("Bob");
			g.addPlayer("Ted");
			g.addPlayer("Jim");
			g.addPlayer("Roger");

			Assert.AreEqual(4, g.Players.Count);
			g.removePlayer(3);
			Assert.AreEqual(4, g.Players.Count);
			g.update();
			Assert.AreEqual(3, g.Players.Count);

			Assert.IsNull(g.getPlayerById(3));
		}

		[TestMethod()]
		public void loadPlayersTest() {
			Game g = new Game();

			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};

			Assert.IsFalse(g.loadPlayers(null));
			Assert.IsTrue(g.loadPlayers(players));
			Assert.AreEqual(3, g.Players.Count);
		}

		[TestMethod()]
		public void getPlayerByIdTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.IsNull(g.getPlayerById(-1));
			Assert.IsNull(g.getPlayerById(8000));
			Assert.AreEqual("Bob", g.getPlayerById(1).name);
		}

		[TestMethod()]
		public void getPlayerByUsernameTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.IsNull(g.getPlayerByUsername("not a username"));
			Assert.AreEqual("Ted", g.getPlayerByUsername("Ted").name);
		}

		[TestMethod()]
		public void getIdByUsernameTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.AreEqual(-1, g.getIdByUsername("not a username"));
			Assert.AreEqual(3, g.getIdByUsername("Rob"));
		}

		[TestMethod()]
		public void setPlayerMoveTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.IsTrue(g.setPlayerMove(1, Direction.Up, false));
			Assert.IsTrue(g.Players[0].Moves[0]);
			Assert.IsFalse(g.setPlayerMove(1, Direction.Down, false));
			Assert.IsFalse(g.Players[0].Moves[1]);
			Assert.IsFalse(g.setPlayerMove(400, Direction.Left, false));
			Assert.IsTrue(g.setPlayerMove(1, Direction.Up, true));
			Assert.IsTrue(g.setPlayerMove(1, Direction.Up, true));
			Assert.IsFalse(g.Players[0].Moves[0]);

			g.setPlayerMove(1, Direction.Up, false);
			g.setPlayerMove(1, Direction.Left, false);
			g.update();
			Thread.Sleep(1000);
			g.update();
			Assert.IsTrue(g.Players[0].x < (Player.movementSpeed / Player.speedPerUnit));
			Assert.IsTrue(g.Players[0].y < (Player.movementSpeed / Player.speedPerUnit));
        }

		[TestMethod()]
		public void setPlayerLocationTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.IsFalse(g.setPlayerLocation(500, 565, 564));
			Assert.IsTrue(g.setPlayerLocation(2, 4.44, 5.55));
			Assert.AreEqual(g.Players[1].x, 4.44);
			Assert.AreEqual(g.Players[1].y, 5.55);

		}

		[TestMethod()]
		public void updateTest() {
			Game g = new Game();
			List<Player> players = new List<Player>() {
				new Player(1, "Bob"),
				new Player(2, "Ted"),
				new Player(3, "Rob")
			};
			g.loadPlayers(players);

			Assert.AreEqual(3, g.Players.Count);
			g.removePlayer(3);
			Assert.AreEqual(3, g.Players.Count);
			g.update();
			Assert.AreEqual(2, g.Players.Count);
			Assert.IsNull(g.getPlayerById(3));

			g.setPlayerMove(1, Direction.Up, false);
			g.setPlayerMove(1, Direction.Left, false);
			g.update();
			Thread.Sleep(1000);
			g.update();
			Assert.IsTrue(g.Players[0].x < (Player.movementSpeed / Player.speedPerUnit));
			Assert.IsTrue(g.Players[0].y < (Player.movementSpeed / Player.speedPerUnit));
		}
	}
}