using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GameServer.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;

namespace GameFrontEndDebugger
{
    public partial class Display : Form
    {
        // connection data
        private string _address = "127.0.0.1";
        private int _port = 1234;
        private Client _client;

        // game data
        private List<JObject> Players;
        private List<JObject> Bunkers;
        private List<JObject> Enemies;

        // display
        public Bitmap display { get; set; }

        public Display()
        {
            InitializeComponent();

            DoubleBuffered = true;
            Width = 800;
            Height = 640;

            _client = new Client(_address, _port);
            _client.Start();
            Players = new List<JObject>();
            Bunkers = new List<JObject>();
            Enemies = new List<JObject>();
            display = new Bitmap(Width, Height);

            Application.Idle += HandleApplicationIdle;
        }

        #region Yucky Form Looping

        bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                handleServerMessages();
                draw();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

        #endregion

        private void transmit(string data)
        {
            _client.send(string.Join("", data.Split('\n', '\t', '\r', ' ')));
        }

        private void handleServerMessages()
        {
            KeyValuePair<IPEndPoint, byte[]> m;
            while (_client.MessageQueue.Count > 0)
            {
                m = _client.MessageQueue.Dequeue();

                JObject com = JObject.Parse(Encoding.ASCII.GetString(m.Value));
                string type = (string)com["type"];

                if (type == "map_data")
                {
                    foreach (JObject p in ((JArray)com["players"]))
                    {
                        int index = Players.FindIndex(per => (int)per["creation_id"] == (int)p["creation_id"]);
                        if (index == -1)
                        {
                            Players.Add(p);
                        }
                        else
                        {
                            Players[index] = p;
                        }
                    }

                    foreach (JObject t in ((JArray)com["bunkers"]))
                    {
                        int index = Bunkers.FindIndex(per => (int)per["creation_id"] == (int)t["creation_id"]);
                        if (index == -1)
                        {
                            Bunkers.Add(t);
                        }
                        else
                        {
                            Bunkers[index] = t;
                        }
                    }

                    foreach (JObject e in ((JArray)com["enemies"]))
                    {
                        int index = Enemies.FindIndex(per => (int)per["creation_id"] == (int)e["creation_id"]);
                        if (index == -1)
                        {
                            Enemies.Add(e);
                        }
                        else
                        {
                            Enemies[index] = e;
                        }
                    }
                }

            }
        }

        private void GameApplication_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) {
                transmit("{\"type\": \"move\",\"direction\": 0,\"complete\": true}");
            }

            if (e.KeyCode == Keys.S)
            {
                transmit("{\"type\": \"move\",\"direction\": 1,\"complete\": true}");
            }

            if (e.KeyCode == Keys.A)
            {
                transmit("{\"type\": \"move\",\"direction\": 2,\"complete\": true}");
            }
            if (e.KeyCode == Keys.D)
            {
                transmit("{\"type\": \"move\",\"direction\": 3,\"complete\": true}");
            }
        }

        private void GameApplication_KeyDown(object sender, KeyEventArgs e)
        {
            int screen_move = 10;
            if (e.KeyCode == Keys.Up)
            {
                screen_y -= screen_move;
            }

            if (e.KeyCode == Keys.Down)
            {
                screen_y += screen_move;
            }

            if (e.KeyCode == Keys.Left)
            {
                screen_x -= screen_move;
            }

            if (e.KeyCode == Keys.Right)
            {
                screen_x += screen_move;
            }

            if (e.KeyCode == Keys.PageUp)
            {
                if (zoom > 1)
                {
                    zoom -= (64f/100f);
                }
            }

            if (e.KeyCode == Keys.PageDown)
            {
                if (zoom <= 64)
                {
                    zoom += (64f/100f);
                }
            }

            if (e.KeyCode == Keys.NumPad1)
            {
                transmit("{\"type\": \"login\",\"username\": \"Gauge "+(int)(new Random().NextDouble()*100)+"\"}");
            }

            if (e.KeyCode == Keys.NumPad2)
            {
                transmit("{\"type\": \"logout\"}");
            }

            if (e.KeyCode == Keys.NumPad3)
            {
                transmit("{\"type\": \"place_bunker\",\"x\": 60,\"y\": 60,\"timestamp\": 0}");
            }

            if (e.KeyCode == Keys.W)
            {
                transmit("{\"type\": \"move\",\"direction\": 0,\"complete\": false}");
            }

            if (e.KeyCode == Keys.S)
            {
                transmit("{\"type\": \"move\",\"direction\": 1,\"complete\": false}");
            }

            if (e.KeyCode == Keys.A)
            {
                transmit("{\"type\": \"move\",\"direction\": 2,\"complete\": false}");
            }
            if (e.KeyCode == Keys.D)
            {
                transmit("{\"type\": \"move\",\"direction\": 3,\"complete\": false}");
            }

        }

        private Font _font = new Font("courier", 10);
        private SolidBrush _highlightBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0));
        private SolidBrush _nonHighlightBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
        private Pen _highlightPen = new Pen(Color.FromArgb(50, 0, 255, 0), 3);
        private Pen _nonHighlightPen = new Pen(Color.FromArgb(25, 0, 0, 0), 3);

        // number of pixels a 1x1 unit takes up

        private int screen_x = 50;
        private int screen_y = 50;
        private double zoom = 32;

        private RectangleF translateToCameraView(double x, double y, double width, double height)
        {
            RectangleF r = new RectangleF();
            r.X = (float)((Width / 2) - ((float)(screen_x - x) * zoom)) - (float)(zoom * width / 2);
            r.Y = (float)(((Height / 2) - ((float)(screen_y - y) * zoom)) - (float)(zoom * height / 2));
            r.Width = (float)(zoom * width);
            r.Height = (float)(zoom * height);
            return r;
        }

        private void draw()
        {

            Graphics g = Graphics.FromImage(display);

            g.FillRectangle(Brushes.Gray, new RectangleF(0, 0, Width, Height)); // create background
            foreach (JObject p in Players)
            {
                double x = (double)p["x"];
                double y = (double)p["y"];
                g.FillRectangle(Brushes.Green, translateToCameraView(x, y, 1, 1));
            }

            foreach (JObject b in Bunkers)
            {
                double x = (double)b["x"];
                double y = (double)b["y"];
                g.FillRectangle(Brushes.Blue, translateToCameraView(x, y, 2, 2));
            }

            foreach (JObject e in Enemies)
            {
                double x = (double)e["x"];
                double y = (double)e["y"];
                g.FillRectangle(Brushes.Red, translateToCameraView(x, y, 1, 1));
            }

            g.DrawString(string.Format("Screen Location: {0} | {1}", screen_x, screen_y), _font, Brushes.DarkRed, 5, 5);
            g.DrawString(string.Format("Zoom {0}%", zoom/64*100), _font, Brushes.DarkRed, 5, 20);

            g.Flush();
            g.Dispose();

            Graphics formGraphics = CreateGraphics();
            formGraphics.DrawImage(display, 0, 0);
            formGraphics.Flush();
            formGraphics.Dispose();
        }

        private void Display_Resize(object sender, EventArgs e)
        {
            display = new Bitmap(Width, Height);
        }
    }
}
