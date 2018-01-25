using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Simulation
    {
        public List<Tuple<double, double>> downloadPoints = new List<Tuple<double, double>>();
        public List<Tuple<double, double>> bitRatePoints = new List<Tuple<double, double>>();

        private double bandwidth; //predkosc sciagania high lub low
        private double currentTime;
        private double segmentTime = 0;
        private double buffer = 0;
        private double time = 0;
        private double bitRate = 2;

        private List<Event> events = new List<Event>();

        private bool downloading = false;
        private bool playing = false;

        private const int durationTime = 350;   //dlugosc symulacji
        private const int bufferSize = 30;
        private const double high = 6.5;
        private const int low = 1;
        private const double segmentSize = 2;
        private static readonly double[] availableBitrates = { 1.4, 2.0, 3.2, 4.3 };


        //symuluje i zwraca punkty do wykresu
        public List<Tuple<double, double>> Simulate()
        {
            List<Tuple<double, double>> parameters = new List<Tuple<double, double>>(); //wspolrzedne do wykresu

            Random rand = new Random(); //rand do rozkładu

            currentTime = 0;
            bandwidth = low;

            parameters.Add(Tuple.Create(buffer, currentTime));  //dodaj punkt do wykresu

            downloadPoints.Add(Tuple.Create(bandwidth, currentTime));

            bitRatePoints.Add(Tuple.Create(bitRate, currentTime));

            InsertEvents();

            while (currentTime < durationTime)
            {
                events = events.OrderBy(e => e.time).ToList();  //sortuje zdarzenia wzgledem czasu ich wystapienia

                Event currentEvent = events[0];

                //currentTime = currentEvent.time;

                if (downloading)
                {
                    double downloadTime = (currentEvent.time - currentTime) * bandwidth / bitRate;
                    buffer += downloadTime;
                    segmentTime += downloadTime;
                }

                if (playing)
                {
                    buffer -= (currentEvent.time - currentTime);
                    if (buffer < 0)
                        buffer = 0;
                }

                switch (currentEvent.type)
                {
                    case Event.Type.BandwidthChange:
                        downloadPoints.Add(Tuple.Create(bandwidth, currentTime));   //dodaj punkt do drugiego wykresu
                        bitRatePoints.Add(Tuple.Create(bitRate, currentTime));
                        bandwidth = currentEvent.value; //zmiana bandwidth na tę z Eventu
                        chooseBitrate();

                        if (downloading)
                        {
                            RemoveEvents();
                            continueDownloading(currentEvent);
                        }

                        break;
                    case Event.Type.DownloadFinished:
                        segmentTime = 0.0; //zaktualizuj licznik pobranego segmentu

                        //jeśli bufor >= 30 to nie pobieraj nic
                        if (buffer >= bufferSize)
                        {
                            downloading = false;
                        }
                        else
                        {
                            continueDownloading(currentEvent);
                        }

                        break;
                    case Event.Type.SegmentPlayFinished:
                        if (buffer >= segmentSize)
                        {
                            events.Add(new Event(Event.Type.SegmentPlayFinished, currentEvent.time + segmentSize,  0.0));
                            playing = true;
                        }
                        else
                        {
                            playing = false;
                        }

                        if (!downloading && (buffer < bufferSize))
                        {
                            continueDownloading(currentEvent);
                        }

                        break;
                }

                if (!playing)
                {
                    if (bandwidth > bitRate && buffer >= segmentSize)
                    {
                        events.Add(new Event(Event.Type.SegmentPlayFinished, currentEvent.time + segmentSize, 0.0));
                        playing = true;
                    }

                    if (bandwidth <= bitRate && buffer >= 0)
                    {
                        events.Add(new Event(Event.Type.SegmentPlayFinished, currentEvent.time + segmentSize, 0.0));
                        playing = true;
                    }

                    if (!downloading)
                    {
                        continueDownloading(currentEvent);
                    }
                }

                currentTime = currentEvent.time;
                events.Remove(currentEvent);
                parameters.Add(Tuple.Create(buffer, currentTime));  //dodaj punkt do wykresu
                downloadPoints.Add(Tuple.Create(bandwidth, currentTime));   //dodaj punkt do drugiego wykresu
                bitRatePoints.Add(Tuple.Create(bitRate, currentTime));

            }

            return parameters;
        }

        private void InsertEvents()
        {
            double myBandwidth;
            Random rand = new Random();
            Random rand2 = new Random();
            time = 0.0;
            events.Add(new Event(Event.Type.BandwidthChange, 0.0, high));

            while (time < durationTime)
            {
                myBandwidth = SetRandomBandwidth(rand);

                time += exponentialDistribution(rand2, 0.08); //losuj czas
                Event e = new Event(Event.Type.BandwidthChange, time, myBandwidth);
                events.Add(e);
            }
        }

        private void chooseBitrate()
        {
            bitRate = availableBitrates[0];
            for(int i =0; i<availableBitrates.Length; i++)
            {
                if (availableBitrates[i] < bandwidth)
                    bitRate = availableBitrates[i];
                else
                    break;
            }
        }

        private void continueDownloading(Event e)
        {
            double videoTime = segmentSize - segmentTime;
            double download = videoTime * bitRate / bandwidth;
            events.Add(new Event(Event.Type.DownloadFinished, e.time + download, 0.0));
            downloading = true;
        }

        private void RemoveEvents()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i<events.Count; i++)
            {
                if (events[i].type == Event.Type.DownloadFinished)
                {
                    ids.Add(i);
                }
            }
            for(int i = 0; i<ids.Count; i++)
            {
                events.Remove(events[ids[i]]);
            }
        }

        private double exponentialDistribution(Random rand, double lambda)    // rozkład wykładniczy pstwa
        {
            //const double lambda = 0.07;

            return Math.Log(1-rand.NextDouble()) / (-lambda);
        }

        private double SetRandomBandwidth(Random rand)
        {
            return rand.NextDouble() * 5.5 + 1;
        }

        private void changeDownload(Random rand)
        {
            if (bandwidth == high)
              bandwidth = low;
            else if (bandwidth == low)
              bandwidth = high;
        }

    }
}
