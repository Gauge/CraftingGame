using System;

namespace GameServer.Data.Interactables
{


    public abstract class GameObject
    {
        public int ObjectID { get; }
        public int CreationID { get; }
        public string Name { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double ActionRadious { get; }

        public GameObject(int id, string name, double x, double y, double actionRadious = 3)
        {
            CreationID = Helper.generateCreationID();
            ObjectID = id;
            Name = name;
            X = x;
            Y = y;
            Width = 1;
            Height = 1;

            this.ActionRadious = actionRadious;
        }

        public GameObject(int id, string name, double x, double y, int width, int height, double actionRadious = 3)
        {
            this.ObjectID = id;
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            this.ActionRadious = actionRadious;
        }

        //public abstract void interact();
        public abstract void update(Game game);

        public bool isInActionRange(Player p)
        {
            return isInActionRange(p.X, p.Y);
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
