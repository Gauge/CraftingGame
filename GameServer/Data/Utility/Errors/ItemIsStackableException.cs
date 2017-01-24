using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Errors {
	public class ItemIsStackableException : Exception {

		public ItemIsStackableException() : base("Could not preform operation. Item is not stackable.") { }
	}
}
