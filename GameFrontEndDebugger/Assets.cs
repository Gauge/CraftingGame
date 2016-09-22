using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameFrontEndDebugger {
	class Assets {

		private static Dictionary<int, Image> icons;

		public static Image getIconById(int id) {
			if (icons == null) {
				icons = new Dictionary<int, Image>();

				string assetPath = Path.GetFullPath(".") + @"\Assets\";

				string[] assetNames = Directory.GetFiles(assetPath);

				foreach (string name in assetNames) {
					string assetId = name.Split('\\').Last();
					icons.Add(int.Parse(assetId), Image.FromFile(name));
				}
			}

			return icons[id];

		} 

	}
}
