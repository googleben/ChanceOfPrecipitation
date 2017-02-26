using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class HealthBarBuilder
    {
        public enum Options
        {
            Boss
        }

        public Vector2 Position { get; set; } = new Vector2(100, 100);
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

        public HealthBarBuilder(int maxHealth)
        {
            MaxHealth = maxHealth;
        }

        public HealthBarBuilder()
        {
        }

        public HealthBar Build()
        {
            return new HealthBar(this);
        }

        public HealthBar Build(Options option)
        {
            if (option == Options.Boss)
            {
                Position = new Vector2(10, 10);
                Width = Game1.BufferWidth - 20;
            }

            return new HealthBar(this);
        }
    }
}
