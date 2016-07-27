using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tests {
	[TestClass()]
	public class PlayerTests {

		[TestMethod()]
		public void PlayerTest() {
			Player p = new Player(1, "Bob");
			Player p2 = new Player(2, "Ted");

			Assert.AreNotEqual(p.id, -1, "ID is never -1");
			Assert.AreEqual((p.id + 1), p2.id, "ID Increments by 1");
			Assert.AreEqual(p.username, "Bob", "Username is the same as the username argument");
			Assert.AreEqual(p.x, 0, "X value is 0");
			Assert.AreEqual(p.y, 0, "Y value is 0");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, false }, "Moves are a list of booleans of length 4. One for each direction");
		}

		[TestMethod()]
		public void setMoveTest() {
			Player p = new Player(1, "Bob");

			// Start and complete a move in every direction
			Assert.IsTrue(p.setMove(Direction.Up, false), "Successfully setting a move returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { true, false, false, false }, "Property Moves shows move added");

			Assert.IsTrue(p.setMove(Direction.Up, true), "On complete function returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, false }, "Property Moves is shows move removed");

			Assert.IsTrue(p.setMove(Direction.Down, false), "Successfully setting a move returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, true, false, false }, "Property Moves shows move added");

			Assert.IsTrue(p.setMove(Direction.Down, true), "On complete function returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, false }, "Property Moves is shows move removed");

			Assert.IsTrue(p.setMove(Direction.Left, false), "Successfully setting a move returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, true, false }, "Property Moves shows move added");

			Assert.IsTrue(p.setMove(Direction.Left, true), "On complete function returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, false }, "Property Moves is shows move removed");

			Assert.IsTrue(p.setMove(Direction.Right, false), "Successfully setting a move returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, true }, "Property Moves shows move added");

			Assert.IsTrue(p.setMove(Direction.Right, true), "On complete function returns true");
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, false }, "Property Moves is shows move removed");


			// make sure that sending the command twice doesn't cause issues
			p.setMove(Direction.Up, false);
            Assert.IsTrue(p.setMove(Direction.Up, false), "Repeating a move command does nothing");
			CollectionAssert.AreEqual(p.Moves, new bool[] { true, false, false, false }, "Repeating a move command does nothing");
			p.setMove(Direction.Up, true);


			//Opposite directions will be ignored
			p.setMove(Direction.Up, false);
			Assert.IsFalse(p.setMove(Direction.Down, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { true, false, false, false });
			p.setMove(Direction.Up, true);

			p.setMove(Direction.Down, false);
			Assert.IsFalse(p.setMove(Direction.Up, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, true, false, false });
			p.setMove(Direction.Down, true);

			p.setMove(Direction.Left, false);
			Assert.IsFalse(p.setMove(Direction.Right, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, true, false });
			p.setMove(Direction.Left, true);

			p.setMove(Direction.Right, false);
			Assert.IsFalse(p.setMove(Direction.Left, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, false, false, true });
			p.setMove(Direction.Right, true);


			//Diagonal moves are alright
			p.setMove(Direction.Up, false);
			Assert.IsTrue(p.setMove(Direction.Left, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { true, false, true, false });
			p.setMove(Direction.Up, true);
			p.setMove(Direction.Left, true);

			//Diagonal moves are alright
			p.setMove(Direction.Up, false);
			Assert.IsTrue(p.setMove(Direction.Right, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { true, false, false, true });
			p.setMove(Direction.Up, true);
			p.setMove(Direction.Right, true);

			//Diagonal moves are alright
			p.setMove(Direction.Down, false);
			Assert.IsTrue(p.setMove(Direction.Left, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, true, true, false });
			p.setMove(Direction.Down, true);
			p.setMove(Direction.Left, true);

			//Diagonal moves are alright
			p.setMove(Direction.Down, false);
			Assert.IsTrue(p.setMove(Direction.Right, false));
			CollectionAssert.AreEqual(p.Moves, new bool[] { false, true, false, true });
			p.setMove(Direction.Down, true);
			p.setMove(Direction.Right, true);
		}
	}
}