using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Networking {
	public class Server {

		private int _port;
		private List<Message> _messages;
		private UdpClient _socket;
		private Thread _clientListener;
		private bool _isListening = false;

		public int Port {
			get { return _port; }
		}

		public List<Message> PendingMessages {
			get {
				if (_messages.Count > 0) {
					List<Message> m = _messages;
					_messages = new List<Message>();
					return m;
				}
				return new List<Message>();
			}
		}

		public Server(int port) {
			_port = port;
			_socket = new UdpClient(new IPEndPoint(IPAddress.Any, port));
			_messages = new List<Message>();
		}

		public void Start() {
			Console.WriteLine("Starting Server...");
			_isListening = true;
			_clientListener = new Thread(new ThreadStart(this.listenForClientData));
			_clientListener.Start();
		}

		public void Stop() {
			_isListening = false;
		}

		private void listenForClientData() {
			IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
			Console.WriteLine("Listening for Clients...");

			while (_isListening) {
				try {
					byte[] data = _socket.Receive(ref client);
					_messages.Add(new Message(data, client));
				} catch (SocketException e) {
					Console.WriteLine(e.ToString());
				}
			}
			Console.WriteLine("No Longer Listening for Clients");
		}

		public void send(IPEndPoint client, string data) {
			this.send(client, Encoding.ASCII.GetBytes(data));
		}

		public void send(IPEndPoint client, byte[] data) {
			_socket.Send(data, data.Length, client);
		}

		public void broadcast(IPEndPoint[] clients, string data) {
			this.broadcast(clients, Encoding.ASCII.GetBytes(data));
		}

		public void broadcast(IPEndPoint[] clients, byte[] data) {
			for (int i = 0; i < clients.Length; i++) {
				_socket.Send(data, data.Length, clients[i]);
			}
		}
	}
}