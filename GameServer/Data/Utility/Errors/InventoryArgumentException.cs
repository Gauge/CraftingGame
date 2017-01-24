using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	class InventoryArgumentException : Exception {
		public InventoryArgumentException() : base() {

		}

		public InventoryArgumentException(string message) : base(message) {
		}

		public InventoryArgumentException(string message, Exception inner) : base(message, inner) {

		}
	}
}
