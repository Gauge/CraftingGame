using System;
using System.Collections.Generic;

namespace GameServer {

    public class Game {

        private struct Move {
            public int id;
            public Direction direction;

            public Move(int id, Direction direction) {
                this.id = id;
                this.direction = direction;
            }
        }

        private List<Player> _players;
        private List<Move> _activeMoves;
        private List<int> _playersToRemove;
        private long _lastTime;

        public List<Player> Players {
            get { return _players; }
        }

        public Game() {
            _players = new List<Player>();
            _activeMoves = new List<Move>();
            _playersToRemove = new List<int>();
            _lastTime = 0;
        }

        public int addPlayer(string username) {
            if (_players.Find(p => p.username == username) == null) {
                _players.Add(new Player(username));
                return getIdByUsername(username);
            }
            return -1;
        }

        public int addPlayer(Player p) {
            if (_players.Find(plr => plr.username == p.username) == null) {
                _players.Add(p);
                return getIdByUsername(p.username);
            }
            return -1;
        }

        public void removePlayer(int id) {
            _playersToRemove.Add(id);
        }

        public void loadPlayers(List<Player> players) {
            _players = players;
        }

        public Player getPlayerById(int id) {
            return _players.Find(u => u.id == id);
        }

        public Player getPlayerByUsername(string username) {
            return _players.Find(p => p.username == username);
        }

        public int getIdByUsername(string username) {
            Player plr = _players.Find(p => p.username == username);
            if (plr != null) return plr.id;
            return -1;
        }

        public void setPlayerMove(int id, Direction direction, bool isComplete) { 

            Move[] currentMoves = _activeMoves.FindAll(m => m.id == id).ToArray();

            if (currentMoves.Length > 0) {

                foreach (Move m in currentMoves) {
                    if (isComplete && m.direction == direction) {
                        _activeMoves.Remove(m);
                    }

                    int[] upDown = new int[] { 1, 1, 0, 0 };
                    int[] leftRight = new int[] { 0, 0, 1, 1 };
                    if (!isComplete &&
                        !(upDown[(int)m.direction] == 1 && upDown[(int)direction] == 1) &&
                        !(leftRight[(int)m.direction] == 1 && leftRight[(int)direction] == 1)) {

                        _activeMoves.Add(new Move(id, direction));
                    }
                }
            } else if (!isComplete) {
                _activeMoves.Add(new Move(id, direction));
            }
        }

        public void setPlayerLocation(int id, double x, double y) {
            Player p = getPlayerById(id);
            int index = _players.FindIndex(player => player.Equals(p));
            p.x = x;
            p.y = y;

            _players[index] = p;

        }

        public void update() {
            updateRemove();

            // update delta
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long delta = currentTime - _lastTime;
            _lastTime = currentTime;

            foreach (Move move in _activeMoves) {

                Player p = getPlayerById(move.id);
                double amountToMove = (((double)(Player.movementSpeed/Player.speedPerUnit) * (double)delta) / 1000);

                switch (move.direction) {

                    case Direction.Up:
                        p.y -= amountToMove;
                        break;

                    case Direction.Down:
                        p.y += amountToMove;
                        break;

                    case Direction.Left:
                        p.x -= amountToMove;
                        break;

                    case Direction.Right:
                        p.x += amountToMove;
                        break;
                }
            }
        }

        private void updateRemove() {
            foreach (int id in _playersToRemove) {
                if (id != -1) {
                    int userId = _players.FindIndex(u => u.id == id);
                    Console.WriteLine("Removing player: " + Players[userId].username);
                    if (userId >= 0 && userId <= _players.Count) {
                        _players.RemoveAt(userId);
                        List<Move> moves = _activeMoves.FindAll(m => m.id == userId);

                        foreach (Move m in moves) {
                            Console.WriteLine("Removing move: " + m.ToString());
                            _activeMoves.Remove(m);
                        }
                    }
                }
            }
            _playersToRemove.Clear();
        }

        public override string ToString() {
            string data = "Active Players:\n";
            foreach (Player p in _players) {
                data += p.ToString() + "\n";
            }

            foreach (Move m in _activeMoves) {
                data += m.ToString() + "\n";
            }

            return data;
        }
    }
}
