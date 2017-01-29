using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data.Interactables
{
    public class Players : List<Player>
    {

        private List<int> _playersToRemove;
        private int _currentID = 1;
        private int _maxID = 99999999;
        private bool _maxedOut = false;

        private int generatePlayerID()
        {
            if (_currentID > _maxID)
            {
                _currentID = 1;
                _maxedOut = true;
            }
            // increments by 1 the first time through
            if (!_maxedOut && _currentID <= _maxID)
            {
                return _currentID++;
            }

            // after the first time though looks for free id's
            while (_currentID <= _maxID)
            {
                if (this.Find(p => p.PlayerID == _currentID) == null)
                {
                    return _currentID;
                }
                _currentID++;
            }
            // starts it over if it reaches the max
            return generatePlayerID();
        }

        public Players()
        {
            _playersToRemove = new List<int>();
        }

        private void removePlayers(Game game)
        {
            foreach (int id in _playersToRemove)
            {
                game.Turrets.removeTurretByPlayerID(id);
                this.RemoveAll(p => p.PlayerID == id);
            }
            _playersToRemove.Clear();
        }

        public int addPlayer(string username)
        {
            return addPlayer(new Player(generatePlayerID(), username));
        }

        public int addPlayer(Player p)
        {
            if (this.Find(plr => plr.Name == p.Name || plr.PlayerID == p.PlayerID) == null)
            {
                this.Add(p);
                return getIdByUsername(p.Name);
            }
            return -1;
        }

        public void removePlayer(int id)
        {
            _playersToRemove.Add(id);
        }

        public bool loadPlayers(List<Player> players)
        {
            if (players != null)
            {
                this.AddRange(players);
                return true;
            }
            return false;
        }

        public Player getPlayerById(int id)
        {
            return this.Find(u => u.PlayerID == id);
        }

        public Player getPlayerByUsername(string username)
        {
            return this.Find(p => p.Name == username);
        }

        public int getIdByUsername(string username)
        {
            Player plr = this.Find(p => p.Name == username);
            if (plr != null)
                return plr.PlayerID;
            return -1;
        }

        public void setPlayer(Player player)
        {
            this[this.FindIndex(u => u.PlayerID == player.PlayerID)] = player;
        }

        public void update(Game game)
        {
            removePlayers(game);
            foreach (Player p in this)
            {
                p.update(game);
            }
        }
    }
}
