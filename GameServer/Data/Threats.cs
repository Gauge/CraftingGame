using GameServer.Data.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public class Threats
    {
        private List<Threat> _threats = new List<Threat>();
        private List<Threat> _toRemove = new List<Threat>();

        public Threat CurrentThreat { get; private set; }

        public void applyThreat(GameObject unit, int quantity)
        {
            Threat t = _threats.Find(u => u.Unit.Equals(unit));

            if (t != null)
            {
                t.Value += quantity;
                t.TimeSinceLastThreat = Helper.getTimestamp();
            }
            else
            {
                t = new Threat();
                t.Unit = unit;
                t.Value = quantity;
                t.TimeSinceLastThreat = Helper.getTimestamp();
                _threats.Add(t);
            }

            if (CurrentThreat == null || t.Value > CurrentThreat.Value)
            {
                CurrentThreat = t;
            }
        }

        public void update()
        {
            long time = Helper.getTimestamp();
            int systemDelta = (int)Helper.getDelta();
            foreach (Threat t in _threats)
            {
                long elapsDelta = time - t.TimeSinceLastThreat;
                if (elapsDelta > Threat.DECAY_ELAPS_TIME)
                {
                    if (t.DecayProcTime > Threat.DECAY_PROC_INTERVAL)
                    {
                        t.DecayProcTime -= Threat.DECAY_PROC_INTERVAL;

                        t.Value -= Threat.DECAY_AMOUNT;
                        if (t.Value <= 0)
                        {
                            _toRemove.Add(t);
                        }

                    }
                    t.DecayProcTime += systemDelta;
                }
            }


            foreach (Threat t in _toRemove)
            {
                _threats.Remove(t);
            }
            _toRemove.Clear();
        }

    }
}
