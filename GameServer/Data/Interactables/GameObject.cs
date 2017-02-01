using System;
using System.Collections.Generic;

namespace GameServer.Data.Interactables
{
    public abstract class GameObject
    {
        public int TypeID { get; }
        public int CreationID { get; }
        public string Name { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double ActionRadious { get; }
        public List<Player> Observers { get; set; }

        private bool observerSetForUpdate { get; set; }

        public GameObject(int id, string name, double x, double y, double actionRadious = 3)
        {
            CreationID = Helper.generateCreationID();
            TypeID = id;
            Name = name;
            X = x;
            Y = y;
            Width = 1;
            Height = 1;
            observerSetForUpdate = false;
            Observers = new List<Player>();

            this.ActionRadious = actionRadious;
        }

        public GameObject(int id, string name, double x, double y, int width, int height, double actionRadious = 3)
        {
            CreationID = Helper.generateCreationID();
            this.TypeID = id;
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            observerSetForUpdate = false;
            Observers = new List<Player>();

            this.ActionRadious = actionRadious;
        }

        //public abstract void interact();
        public virtual void update(Game game) {

            if (!observerSetForUpdate)
            {
                Action tempAction = new Action(this, ActionType.ENTER_PLAYER_VISION);
                List<Player> observerList = tempAction.getEffectedPlayerIDs(game);

                foreach (Player observer in observerList)
                {
                    if (Observers.Find(o => o.PlayerID == observer.PlayerID) == null)
                    {
                        game.Actions.Enqueue(tempAction);
                        observerSetForUpdate = true;
                    }
                }
            }
        }

        public void updateObservers(List<Player> players)
        {
            Observers = players;
            observerSetForUpdate = false;
        }

        public bool isInActionRange(GameObject o)
        {
            return isInActionRange(o.X, o.Y);
        }

        public bool isInActionRange(double a, double b)
        {
            return isInRadious(a, b, ActionRadious);
        }

        public bool isInRadious(double a, double b, double r)
        {
            // Pythagoras
            return Math.Pow((X - (Width / 2) - a), 2) + Math.Pow((Y + (Height / 2) - b), 2) < (Math.Pow(r, 2) + ((Width + Height) / 4));
        }
    }
}
