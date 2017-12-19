using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Event
    {
        public enum Type { BandwidthChange, SegmentPlay};

        public Type type; //typ eventu
        public double time; //czas w symulacji

        public Event(Type type, double time)
        {
            this.type = type;
            this.time = time;
        }
    }
}
