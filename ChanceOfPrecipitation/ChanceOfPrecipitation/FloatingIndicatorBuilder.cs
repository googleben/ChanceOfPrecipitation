﻿using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicatorBuilder
    {
        public float Scale { get; set; } = 2f;
        public float UpSpeed { get; set; } = 0.3f;
        public bool Oscillates { get; set; } = false;
        public float OscillationDist { get; set; } = 1;
        public float OscillationPeriod { get; set; } = 500;
        public float Life { get; set; } = 2000;
        public Color Color { get; set; } = Color.White;
        public bool IsStatic { get; set; } = false;
        public int Spacing { get; set; } = 0;

        public FloatingIndicator Build(int number, Vector2 position)
        {
            return new FloatingIndicator(this, number, position);
        }
    }
}
