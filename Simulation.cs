using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Simulation
    {
        public List<Tuple<int, double>> downloadPoints = new List<Tuple<int, double>>();

        private int bandwidth; //predkosc sciagania high lub low
        private double currentTime;
        private double prevTime;
        private double buffer = 0;

        private const int durationTime = 150;   //dlugosc symulacji
        private const int bufferSize = 30;
        private const int bitRate = 2;
        private const int high = 5;
        private const int low = 1;
        private const double segmentSize = 2;
        

        //symuluje i zwraca punkty do wykresu
        public List<Tuple<double, double>> Simulate()
        {
            List<Tuple<double, double>> parameters = new List<Tuple<double, double>>(); //wspolrzedne do wykresu

            List<Event> events = new List<Event>();

            Random rand = new Random(); //rand do rozkładu

            currentTime = 0;
            prevTime = 0;
            bandwidth = high;

            Event firstBandwidthChange = new Event (Event.Type.BandwidthChange, exponentialDistribution(rand));
            events.Add(firstBandwidthChange);

            Event firstSegmentPlay = new Event(Event.Type.SegmentPlay, currentTime + (segmentSize/bandwidth));
            events.Add(firstSegmentPlay);

            parameters.Add(Tuple.Create(buffer, currentTime));  //dodaj punkt do wykresu

            downloadPoints.Add(Tuple.Create(bandwidth, currentTime));

            while (currentTime < durationTime)
            {
                events = events.OrderBy(e => e.time).ToList();  //sortuje zdarzenia wzgledem czasu ich wystapienia

                Event currentEvent = events[0];

                currentTime = currentEvent.time;

                switch(currentEvent.type)
                {
                    case Event.Type.BandwidthChange:
                        changeDownload();   //zmien pasmo
                        Event changeBandwidth = new Event(Event.Type.BandwidthChange, exponentialDistribution(rand) + currentTime);
                        events.Add(changeBandwidth);    //wylosuj nowy czas zmiany i dodaj do listy
                        break;

                    case Event.Type.SegmentPlay:
                        Event nextSegment = new Event(Event.Type.SegmentPlay, currentTime + (bandwidth/segmentSize));
                        events.Add(nextSegment);    //dodaj do listy event wczytujacy nastepny segment
                        if (buffer >= bufferSize)
                        {
                            buffer = bufferSize;    //jak bufor osiaga wartosc optymalna to nie wczytuje danych tylko je wyrzuca
                            buffer -= (currentTime - prevTime) * bitRate / segmentSize;
                        }
                        else if (buffer <= 0)
                        {
                            buffer = 0; //jak jest pusty to tylko  pobiera
                            buffer += (currentTime - prevTime) * bandwidth / segmentSize;
                        }
                        else
                        {
                            //w pozostałych przypadkach wczytuje i odtwarza jednoczesnie
                            buffer += (currentTime - prevTime) * bandwidth / segmentSize;
                            buffer -= (currentTime - prevTime) * bitRate / segmentSize;

                            //zabezpieczenia przed wyjsciem poza zakres
                            if (buffer > bufferSize)
                                buffer = bufferSize;
                            if (buffer < 0)
                                buffer = 0;
                        }
                        prevTime = currentTime;
                        break;
                }
                parameters.Add(Tuple.Create(buffer, currentTime));  //dodaj punkt do wykresu

                downloadPoints.Add(Tuple.Create(bandwidth, currentTime));   //dodaj punkt do drugiego wykresu

                events.Remove(events[0]);
            }

            return parameters;
        }

        double exponentialDistribution(Random rand)    // rozkład wykładniczy pstwa
        {
            const double lambda = 0.04;

            return 10+Math.Log(1-rand.NextDouble()) / (-lambda);
        }

        void changeDownload()
        {
            if (bandwidth == high)
                bandwidth = low;
            else if (bandwidth == low)
                bandwidth = high;
        }

    }
}
