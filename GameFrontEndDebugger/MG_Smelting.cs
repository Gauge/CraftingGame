using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameServer.Data.Interactables;
using GameServer.Data;

namespace GameFrontEndDebugger {

	public partial class MG_Smelting : Form {
		private enum State { Forground, Background, Dead }
		private State state;
		public Smelters game { get; set; }

		public MG_Smelting() {
			InitializeComponent();
			state = State.Background;

			Application.Idle += new EventHandler(mainLoop);
		}

		#region game loop setup

		private bool AppStillIdle {
			get {
				NativeMessage msg;
				return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NativeMessage {
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}

		[System.Security.SuppressUnmanagedCodeSecurity] // We won’t use this maliciously
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(out NativeMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		private void mainLoop(object sender, EventArgs e) {
			while (AppStillIdle && state != State.Dead) {
				GameServer.Data.Settings.getDelta(true);
				draw();
			}

			if (state == State.Dead) {
				Dispose();
			}
		}
		#endregion

		private void draw() {
			if (game != null && game.state == MG_Type_Smelting.Active) {
				Graphics g = Display.CreateGraphics();


			}
			
		}
	}
}
