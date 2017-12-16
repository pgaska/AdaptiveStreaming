using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Event
    {
        public enum Type { Empty, Full, Regular};

        public Type type; //typ eventu
        public int time; //czas w symulacji
    }
}
