using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveStreaming
{
    class Simulation
    {
        private int currentTime;
        private Event e = new Event();
        private int inBuffer;
        private int download; //predkosc sciagania high lub low

        private const int durationTime = 150;
        private const int bufferSize = 30;
        private const int bitRate = 5;
        private const int high = 8;
        private const int low = 1;
        private const int timeIter = 2;


        //symuluje i zwraca punkty do wykresu
        public List<Tuple<int, int>> Simulate()
        {
            currentTime = 0;
            inBuffer = 0;
            download = high;

            List<Tuple<int, int>> parameters = new List<Tuple<int, int>>(); //wspolrzedne do wykresu

            while (currentTime < durationTime)
            {

                parameters.Add(Tuple.Create<int, int>(inBuffer, currentTime));

                if(currentTime>=30)
                {
                    download = low;
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
    }
}
