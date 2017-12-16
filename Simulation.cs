using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Simulation
    {
        public List<Tuple<int, int>> downloadPoints = new List<Tuple<int, int>>();

        private int download; //predkosc sciagania high lub low
        private int currentTime;
        private Event e = new Event();
        private int inBuffer;

        private const int durationTime = 250;
        private const int bufferSize = 30;
        private const int bitRate = 3;
        private const int high = 5;
        private const int low = 1;
        private const int timeIter = 2;


        //symuluje i zwraca punkty do wykresu
        public List<Tuple<int, int>> Simulate()
        {
            List<Tuple<int, int>> parameters = new List<Tuple<int, int>>(); //wspolrzedne do wykresu
            double expDistribution = exponentialDistribution();
            int changeTime = currentTime + (int)expDistribution; //losowy czas do zmiany pasma

            currentTime = 0;
            inBuffer = 0;
            download = high;

            while (currentTime < durationTime)
            {
                parameters.Add(Tuple.Create(inBuffer, currentTime));  //dodaj punkt do wykresu

                downloadPoints.Add(Tuple.Create(download, currentTime));

                if(currentTime>=changeTime)
                {
                    changeDownload();
                    double distribution = exponentialDistribution();
                    changeTime = currentTime + (int)distribution;
                }

                switch (e.type)
                {
                    case Event.Type.Empty:
                        inBuffer += download;   //jak bufor jest pusty to sciagaj i nie odtwarzaj

                        currentTime += timeIter;
                        if(inBuffer>0)
                        {
                            e.type = Event.Type.Regular;
                        }
                        break;

                    case Event.Type.Full:
                        inBuffer -= bitRate;    //jak bufor pełny to wypluwaj i nie sciagaj

                        currentTime += timeIter;
                        if (inBuffer <= bufferSize)
                        {
                            e.type = Event.Type.Regular;
                        }
                        break;

                    case Event.Type.Regular:
                        inBuffer += download;
                        inBuffer -= bitRate;

                        if (inBuffer <= 0)
                        {
                            inBuffer = 0;
                            e.type = Event.Type.Empty;
                        }
                        if(inBuffer>=bufferSize)
                        {
                            inBuffer = bufferSize;
                            e.type = Event.Type.Full;
                        }
                        currentTime += timeIter;
                        break;
                }
            }
            return parameters;
        }

        double exponentialDistribution()    // rozkład wykładniczy pstwa
        {
            const double lambda = 0.02;
            Random rand = new Random();

            return Math.Log(1-rand.NextDouble()) / (-lambda);
        }

        void changeDownload()
        {
            if (download == high)
                download = low;
            else if (download == low)
                download = high;
        }

    }
}
