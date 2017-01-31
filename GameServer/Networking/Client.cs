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
		private Task _listener;
		private IPEndPoint _server;
		private bool _isListening;

        public Queue<KeyValuePair<IPEndPoint, byte[]>> MessageQueue { get; }

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

		public Client() {
			_port = 0;
			_serverLocation = Dns.GetHostEntry("");
			_socket = new UdpClient();
			MessageQueue = new Queue<KeyValuePair<IPEndPoint, byte[]>>();
		}

		public Client(string hostname, int port) {
			_port = port;
			_serverLocation = Dns.GetHostEntry(hostname);
			_socket = new UdpClient();
			MessageQueue = new Queue<KeyValuePair<IPEndPoint, byte[]>>();
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
            _socket.Dispose();
			_listener.Dispose();
		}

		private void listenForServerData() {
			Console.WriteLine("Listening for Server...");
			while (_isListening) {
				try {
					byte[] data = _socket.Receive(ref _server);
					MessageQueue.Enqueue(new KeyValuePair<IPEndPoint, byte[]>(_server, data));
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
