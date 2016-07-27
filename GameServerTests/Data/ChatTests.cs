using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServer.Tests {
	[TestClass()]
	public class ChatTests {
		[TestMethod()]
		public void ChatTest() {
			Chat c = new Chat("System Message");

			Assert.AreEqual(-1, c.id);
			Assert.AreEqual(ChatType.System, c.chatType);
			Assert.AreEqual("System Message", c.message);
			Assert.AreEqual(null, c.sender);
			Assert.AreEqual(null, c.recipient);

			c = new Chat(3, "Bob", "Global Message");

			Assert.AreEqual(3, c.id);
			Assert.AreEqual(ChatType.Global, c.chatType);
			Assert.AreEqual("Global Message", c.message);
			Assert.AreEqual("Bob", c.sender);
			Assert.AreEqual(null, c.recipient);

			c = new Chat(5, "Ted", "/w Bob this is a message to bob");

			Assert.AreEqual(5, c.id);
			Assert.AreEqual(ChatType.Whisper, c.chatType);
			Assert.AreEqual("this is a message to bob", c.message);
			Assert.AreEqual("Ted", c.sender);
			Assert.AreEqual("Bob", c.recipient);

			c = new Chat(6, "Ted", "/whisper Bob this is the long version");

			Assert.AreEqual(6, c.id);
			Assert.AreEqual(ChatType.Whisper, c.chatType);
			Assert.AreEqual("this is the long version", c.message);
			Assert.AreEqual("Ted", c.sender);
			Assert.AreEqual("Bob", c.recipient);
		}

		[TestMethod()]
		public void ChatSerialization() {
			Chat c = new Chat(6, "Ted", "/whisper Bob this is the long version");

			Chat c2 = (Chat)BaseCommand.fromByteArray(c.toByteArray());

			Assert.IsNotNull(c2);
			Assert.AreEqual(c.id, c2.id);
			Assert.AreEqual(c.chatType, c2.chatType);
			Assert.AreEqual(c.message, c2.message);
			Assert.AreEqual(c.sender, c2.sender);
			Assert.AreEqual(c.recipient, c2.recipient);
			Assert.AreEqual(c.timestamp, c2.timestamp);
		}
	}
}