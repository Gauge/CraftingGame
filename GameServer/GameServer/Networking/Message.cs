using System.Net;

namespace GameServer.Networking {
	public struct Message {
		public byte[] data;
		public IPEndPoint sender;
		public BaseCommand parsed;

		public Message(byte[] d, IPEndPoint s) {
			data = d;
			sender = s;
			parsed = null;
		}
	}
}
