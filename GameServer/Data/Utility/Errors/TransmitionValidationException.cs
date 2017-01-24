using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	class TransmitionValidationException : Exception {

		public TransmitionValidationException(string message) : base(message) {}
	}
}
