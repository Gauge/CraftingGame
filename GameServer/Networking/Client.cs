using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer.Networking {
	public class Client {

		private UdpClient _socket;
		private IPHostEntry _serverLocation;
		private int _port;
		private Dictionary<IPEndPoint, byte[]> _messages;
		private Task _listener;
		private IPEndPoint _server;
		private bool _isListening;

		public bool IsConnected {
			get { return (_listener != null && _listener.Status == TaskStatus.Running && _isListening); }
		}

		public string Address {
			get { return _serverLocation.HostName; }
			set {
				_serverLocation = Dns.GetHostEntry(value);
				this.Stop();
			}
		}

		public int Port {
			get { return _port; }
			set {
				_port = value;
				this.Stop();
			}
		}

		public Dictionary<IPEndPoint, byte[]> PendingMessages {
			get {
				if (_messages.Count > 0) {
					Dictionary<IPEndPoint, byte[]> m = _messages;
					_messages = new Dictionary<IPEndPoint, byte[]>();
					return m;
				}
				return new Dictionary<IPEndPoint, byte[]>();
			}
		}

		public Client() {
			_port = 0;
			_serverLocation = Dns.GetHostEntry("");
			_socket = new UdpClient();
			_messages = new Dictionary<IPEndPoint, byte[]>();
		}

		public Client(string hostname, int port) {
			_port = port;
			_serverLocation = Dns.GetHostEntry(hostname);
			_socket = new UdpClient();
			_messages = new Dictionary<IPEndPoint, byte[]>();
			updateConnection();
		}

		private void updateConnection() {
			if (_serverLocation.AddressList.Length >= 2) {
				_socket = new UdpClient();
				_server = new IPEndPoint(_serverLocation.AddressList[4], _port);
				_socket.Connect(_server);
			} else {
				throw null;
			}
		}

		public void Start() {
			updateConnection();
			_isListening = true;
			_listener = new Task(listenForServerData);
			_listener.Start();
		}

		public void Stop() {
			_socket.Close();
			_isListening = false;
			Task.WaitAll(_listener);
			_listener.Dispose();
		}

		private void listenForServerData() {
			Console.WriteLine("Listening for Server...");
			while (_isListening) {
				try {
					byte[] data = _socket.Receive(ref _server);
					_messages.Add(_server, data);
				} catch {
					Stop();
				}
			}
			Console.WriteLine("No longer Listening for server");

		}

		public void send(string data) {
			send(Encoding.ASCII.GetBytes(data));
		}

		public void send(byte[] data) {
			if (IsConnected)
				_socket.Send(data, data.Length);
		}
	}
}
