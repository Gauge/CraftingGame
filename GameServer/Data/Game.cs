using System;
using System.Collections.Generic;
using GameServer.Data.Interactables;

namespace GameServer.Data {

	public class Game {
		public List<Player> Players { get; private set; }
		public List<Interactable> Tools {get; private set;}
		private List<int> _playersToRemove;

		public Game() {
			Players = new List<Player>();
			Tools = new List<Interactable>();

			Tools.Add(new Kiln(5, 10)); // testing
			_playersToRemove = new List<int>();
		}


		private int _currentID = 1;
		private int _maxID = 99999999;
		private bool _maxedOut = false;
		private int generatePlayerID() {
			if (_currentID > _maxID) {
				_currentID = 1;
				_maxedOut = true;
			}
			// increments by 1 the first time through
			if (!_maxedOut && _currentID <= _maxID) {
				return _currentID++;
			}

			// after the first time though looks for free id's
			while (_currentID <= _maxID) {
				if (Players.Find(p => p.id == _currentID) == null) {
					return _currentID;
				}
				_currentID++;
			}
			// starts it over if it reaches the max
			return generatePlayerID();
		}

		public int addPlayer(string username) {
			return addPlayer(new Player(generatePlayerID(), username));
		}

		public int addPlayer(Player p) {
			if (Players.Find(plr => plr.name == p.name || plr.id == p.id) == null) {
				Players.Add(p);
				return getIdByUsername(p.name);
			}
			return -1;
		}

		public void removePlayer(int id) {
			_playersToRemove.Add(id);
		}

		public bool loadPlayers(List<Player> players) {
			if (players != null) {
				Players = players;
				return true;
			}
			return false;
		}

		public Player getPlayerById(int id) {
			return Players.Find(u => u.id == id);
		}

		public Player getPlayerByUsername(string username) {
			return Players.Find(p => p.name == username);
		}

		public int getIdByUsername(string username) {
			Player plr = Players.Find(p => p.name == username);
			if (plr != null)
				return plr.id;
			return -1;
		}

		public bool setPlayerMove(int id, Direction direction, bool isComplete) {
			Player player = getPlayerById(id);
			if (player != null) {
				return player.setMove(direction, isComplete);
			}
			return false;
		}

		public bool setPlayerLocation(int id, double x, double y) {
			Player p = getPlayerById(id);
			if (p != null) { 
				p.x = x;
				p.y = y;
				return true;
			}
			return false;
		}

		public void update() {
			foreach (Player p in Players) {
				p.update();
			}

			foreach (Interactable i in Tools) {
				i.update();
			}

			removePlayers();
		}

		private void removePlayers() {
			foreach (int id in _playersToRemove) {
				Players.RemoveAll(p => p.id == id);
			}
			_playersToRemove.Clear();
		}

		public override string ToString() {
			string data = "Active Players:\n";
			foreach (Player p in Players) {
				data += p.ToString() + "\n";
			}
			return data;
		}
	}
}
