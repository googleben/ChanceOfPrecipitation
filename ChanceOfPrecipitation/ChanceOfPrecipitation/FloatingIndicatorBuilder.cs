using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicatorBuilder
    {
        public Vector2 Position { get; set; }
        public float Scale { get; set; } = 2f;
        public float UpSpeed { get; set; } = 0.3f;
        public bool Oscillates { get; set; } = false;
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

        public FloatingIndicator Build()
        {
            return new FloatingIndicator(this);
        }
    }
}
