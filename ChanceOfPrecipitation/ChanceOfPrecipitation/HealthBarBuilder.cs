using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class HealthBarBuilder
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 15;
        public int MaxHealth { get; set; } = 100;

        public int BorderWidth { get; set; } = 2;
        public Color BorderColor { get; set; } = new Color(200, 200, 200);
        public Color HealthColor { get; set; } = new Color(0, 255, 0);
        public Color DamageColor { get; set; } = new Color(255, 0, 0);

        public HealthBarBuilder(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public HealthBarBuilder(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public HealthBarBuilder() { }
    }
}
