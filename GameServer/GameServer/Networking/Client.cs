using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Networking {
    public class Client {

        private UdpClient _socket;
        private IPHostEntry _serverLocation;
        private int _port;
        private List<Message> _messages;
        private Thread _listener;
        private IPEndPoint _server;
        private bool _isListening;

        public bool IsConnected {
            get { return (_listener != null && _listener.IsAlive && _isListening); }
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

        public Client() {
            _port = 0;
            _serverLocation = Dns.GetHostEntry("");
            _socket = new UdpClient();
            _messages = new List<Message>();
        }

        public Client(string hostname, int port) {
            _port = port;
            _serverLocation = Dns.GetHostEntry(hostname);
            _socket = new UdpClient();
            _messages = new List<Message>();
            updateConnection();
        }

        private void updateConnection() {
            if (_serverLocation.AddressList.Length >= 2) {
                _socket = new UdpClient();
                _server = new IPEndPoint(_serverLocation.AddressList[1], _port);
                _socket.Connect(_server);
            } else {
                throw null;
            }
        }

        public void Start() {
            updateConnection();
            _isListening = true;
            _listener = new Thread(new ThreadStart(this.listenForServerData));
            _listener.Start();
        }

        public void Stop() {
            _socket.Close();
            _isListening = false;
            _listener = null;
        }

        private void listenForServerData() {
            Console.WriteLine("Listening for Server...");
            while (_isListening) {
                try {
                    byte[] data = _socket.Receive(ref _server);
                    _messages.Add(new Message(data, _server));
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
            _socket.Send(data, data.Length);
        }
    }
}
