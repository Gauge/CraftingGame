using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Data;
using GameServer.Data.Resources;

namespace GameServer.Tests {
	[TestClass()]
	public class OreTests {
		[TestMethod()]
		public void OreTest() {
			Ore o = new Ore(0, "Normal", 400, 34, "This is oreish", 0.5, 5000);
			Assert.AreEqual(0, o.id);
			Assert.AreEqual("Normal", o.name);
			Assert.AreEqual(1, o.count);
			Assert.AreEqual(400, o.value);
			Assert.AreEqual(34, o.weight);
			Assert.AreEqual("This is oreish", o.description);
			Assert.AreEqual(0.5, o.purity);
			Assert.IsFalse(o.isStackable);

			o = new Ore(1, "Nagative weight", -55, -55, "This is oreish", 5.5, 5000);
			Assert.AreEqual(1, o.id);
			Assert.AreEqual("Nagative weight", o.name);
			Assert.AreEqual(1, o.count);
			Assert.AreEqual(0, o.value);
			Assert.AreEqual(0, o.weight);
			Assert.AreEqual("This is oreish", o.description);
			Assert.AreEqual(0, o.purity);
			Assert.IsFalse(o.isStackable);

			o = new Ore(2, "Nagative weight", -55, -55, "This is oreish", -0.5, 5000);
			Assert.AreEqual(2, o.id);
			Assert.AreEqual("Nagative weight", o.name);
			Assert.AreEqual(1, o.count);
			Assert.AreEqual(0, o.value);
			Assert.AreEqual(0, o.weight);
			Assert.AreEqual("This is oreish", o.description);
			Assert.AreEqual(0, o.purity);
			Assert.IsFalse(o.isStackable);
		}

		[TestMethod()]
		public void addTest() {
			Ore o = new Ore(0, "Normal", 400, 50, "This is oreish", 0.5, 5000);

			Assert.IsFalse(o.add(-45, 0.5));
			Assert.IsFalse(o.add(54, 1.2));
			Assert.IsFalse(o.add(54, -0.4));
			Assert.IsTrue(o.add(25, 0.25));

			Assert.AreEqual(75, o.weight);
			Assert.AreEqual((0.5 + 0.25) / 2, o.purity);
		}

		[TestMethod()]
		public void removeTest() {
			Ore o = new Ore(0, "Normal", 400, 50, "This is oreish", 0.5, 5000);

			Assert.IsTrue(o.remove(25));
			Assert.AreEqual(25, o.weight);
			Assert.AreEqual(0.5, o.purity);
			Assert.IsFalse(o.remove(26));
			Assert.AreEqual(25, o.weight);
			Assert.AreEqual(0.5, o.purity);
			Assert.IsFalse(o.remove(-5));
			Assert.AreEqual(25, o.weight);
			Assert.AreEqual(0.5, o.purity);
		}
	}
}