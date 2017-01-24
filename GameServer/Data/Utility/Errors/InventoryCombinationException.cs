using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	class InventoryCombinationException : Exception {

		public InventoryCombinationException() : base("Failed to combine items") {

		}

		public InventoryCombinationException(string message) : base(message) {
		}

		public InventoryCombinationException(string message, Exception inner) : base(message, inner) {

		}
	}
}
