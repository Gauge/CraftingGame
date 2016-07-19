using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace GameServer.Networking {
    public class Server {

        private int _port;
        private List<IPEndPoint> _clients;
        private List<IPEndPoint> _removedClients;
        private List<Message> _messages;
        private UdpClient _socket;
        private Thread _clientListener;
        private bool _isListening = false;

        public int Port {
            get { return _port; }
        }

        public List<IPEndPoint> Clients {
            get { return _clients; }
        }

        public void removeClientByEndPoint(IPEndPoint endPoint) {
            _clients.Remove(endPoint);
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

        public IPEndPoint[] DisconnectedClients {
            get {
                IPEndPoint[] endPoints = _removedClients.ToArray();
                _removedClients.Clear();
                return endPoints;
            }
        }

        public Server(int port) {
            _port = port;
            _socket = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            _clients = new List<IPEndPoint>();
            _messages = new List<Message>();
            _removedClients = new List<IPEndPoint>();
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
                    if (_clients.IndexOf(client) == -1) {
                        Console.WriteLine("Adding client {0}", client.ToString());
                        _clients.Add(client);
                    }
                    _messages.Add(new Message(data, client));
                } catch (SocketException e) {
                    _clients.Remove(client);
                    _removedClients.Add(client);
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

        public void broadcastAll(string data) {
            this.broadcastAll(Encoding.ASCII.GetBytes(data));
        }

        public void broadcastAll(byte[] data) {
            for (int i = 0; i < _clients.Count; i++) {
                _socket.Send(data, data.Length, _clients[i]);
            }
        }
    }
}
