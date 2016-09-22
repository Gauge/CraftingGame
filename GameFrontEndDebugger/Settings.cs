using GameServer.Data;
using System.Windows.Forms;
using System;

namespace GameFrontEndDebugger {
	class Settings {

		public static int UNIT = 16;
		public static bool Debug = true;

		public static Keys Up = Keys.W;
		public static Keys Down = Keys.S;
		public static Keys Left = Keys.A;
		public static Keys Right = Keys.D;
		public static Keys Inventory = Keys.I;
		public static Keys Interact = Keys.E;

		public static Direction keyDirection(Keys key) {
			if (key == Up)
				return Direction.Up;
			if (key == Down)
				return Direction.Down;
			if (key == Left)
				return Direction.Left;
			if (key == Right)
				return Direction.Right;

			return Direction.None;
		}
	}
}
