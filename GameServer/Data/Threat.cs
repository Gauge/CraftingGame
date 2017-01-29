using GameServer.Data.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public class Threat
    {
        public const int DECAY_AMOUNT = 100; // units of threat
        public const int DECAY_ELAPS_TIME = 5000; // ms
        public const int DECAY_PROC_INTERVAL = 1000; // ms

        public GameObject Unit { get; set; }
        public int Value { get; set; }
        public long TimeSinceLastThreat { get; set; }
        public int DecayProcTime { get; set; }
    }
}
