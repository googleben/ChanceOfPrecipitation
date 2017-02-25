using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicatorBuilder
    {
        public Vector2 Position { get; set; }
        public int Scale { get; set; } = 2;
        public float UpSpeed { get; set; } = 0.3f;
        public float OscillationDist { get; set; } = 1;
        public float OscillationPeriod { get; set; } = 500;
        public float Life { get; set; } = 2000;
        public Color Color { get; set; } = Color.White;
        public int Number { get; set; }

        public FloatingIndicatorBuilder(Vector2 position, int number)
        {
            Position = position;
            Number = number;
        }
    }
}
