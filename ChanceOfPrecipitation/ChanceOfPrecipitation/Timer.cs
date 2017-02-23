using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChanceOfPrecipitation
{
    class Timer
    {
        private const float tick = 1 / 60;

        private float time;
        private float currTime;

        public bool Ticked { get; private set; }
        public float CurrValue => currTime;
        public float Time => time;

        public Timer(float time)
        {
            this.time = time;
            currTime = time;
        }

        public void Update()
        {
            Ticked = false;
            currTime -= tick;

            if (currTime <= 0)
            {
                currTime = time;
                Ticked = true;
            }
        }
    }
}
