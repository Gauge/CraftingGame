using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables {
	public class LPlayer : List<Player> {

		private List<int> _playersToRemove;
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
				if (this.Find(p => p.id == _currentID) == null) {
					return _currentID;
				}
				_currentID++;
			}
			// starts it over if it reaches the max
			return generatePlayerID();
		}

		public LPlayer() {
			_playersToRemove = new List<int>();
		}

		private void removePlayers() {
			foreach (int id in _playersToRemove) {
				this.RemoveAll(p => p.id == id);
			}
			_playersToRemove.Clear();
		}

		public int addPlayer(string username) {
			return addPlayer(new Player(generatePlayerID(), username));
		}

		public int addPlayer(Player p) {
			if (this.Find(plr => plr.name == p.name || plr.id == p.id) == null) {
				this.Add(p);
				return getIdByUsername(p.name);
			}
			return -1;
		}

		public void removePlayer(int id) {
			_playersToRemove.Add(id);
		}

		public bool loadPlayers(List<Player> players) {
			if (players != null) {
				this.AddRange(players);
				return true;
			}
			return false;
		}

		public Player getPlayerById(int id) {
			return this.Find(u => u.id == id);
		}

		public Player getPlayerByUsername(string username) {
			return this.Find(p => p.name == username);
		}

		public int getIdByUsername(string username) {
			Player plr = this.Find(p => p.name == username);
			if (plr != null)
				return plr.id;
			return -1;
		}

		public void setPlayer(Player player) {
			this[this.FindIndex(u => u.id == player.id)] = player;
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

	}
}
