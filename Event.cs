using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Event
    {
        public enum Type { BandwidthChange, DownloadFinished, SegmentPlayFinished};

        public Type type; //typ eventu
        public double time; //czas w symulacji
        public double value;

        public Event(Type type, double time, double value)
        {
            this.type = type;
            this.time = time;
            this.value = value;
        }
    }
}
