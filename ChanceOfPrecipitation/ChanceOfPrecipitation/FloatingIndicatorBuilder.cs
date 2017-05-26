using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation {
    public class FloatingIndicatorBuilder {
        public float Scale { get; set; } = 2f;
        public float UpSpeed { get; set; } = 0.3f;
        public int Life { get; set; } = 120;
        public Color Color { get; set; } = Color.White;
        public bool IsStatic { get; set; } = false;
        public int Spacing { get; set; } = 0;

        public FloatingIndicator Build(int number, Vector2 position) {
            return new FloatingIndicator(this, number, position);
        }

        public FloatingIndicator Build(string text, Vector2 position) {
            return new FloatingIndicator(this, text, position);
        }
    }
}