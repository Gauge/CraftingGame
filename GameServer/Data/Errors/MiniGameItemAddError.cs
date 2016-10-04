using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	class MiniGameItemAddError : Exception {

		public MiniGameItemAddError(int index) : base("An item already exists position: " + index) {}

	}
}
