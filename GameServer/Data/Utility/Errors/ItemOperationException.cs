using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	class ItemOperationException : Exception {

		public ItemOperationException(string message) : base(message) {}
	}
}
