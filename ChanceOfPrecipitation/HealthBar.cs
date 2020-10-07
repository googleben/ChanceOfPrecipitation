using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public static class RectExtension
    {
        public static Rectangle add(this Rectangle r, Vector2 amount)
        {
            return new Rectangle((int)(r.X + amount.X), (int)(r.Y + amount.Y), r.Width, r.Height);
        }
    }

    public class HealthBar : GameObject
    {
        //TODO: Clean up and reduce coupling

        private Vector2 position;
        private readonly int width;
        private readonly int height;
        private int maxHealth;
        private float currentHealth;

        private readonly int borderWidth;
        private readonly Color borderColor;
        private readonly Color healthColor;
        private readonly Color damageColor;

        private Rectangle HealthBounds
            =>
                new Rectangle((int)(position.X + borderWidth), (int)(position.Y + borderWidth),
                    HealthWidth - borderWidth * 2, height - borderWidth * 2);

        private Rectangle DamageBounds
            =>
                new Rectangle((int)(HealthWidth + position.X), (int)(position.Y + borderWidth),
                    width - HealthWidth - borderWidth, height - borderWidth * 2);

        public Rectangle BorderBounds => new Rectangle((int)position.X, (int)position.Y, width, height);
        private int HealthWidth => (int)(currentHealth * width / maxHealth);

        private bool isBoss;
        public bool IsBoss => isBoss;

        private bool isPlayer;
        public bool IsPlayer => isPlayer;

        public HealthBar(HealthBarBuilder builder)
        {
            position = builder.Position;
            width = builder.Width;
            height = builder.Height;
            maxHealth = builder.MaxHealth;
            borderWidth = builder.BorderWidth;
            borderColor = builder.BorderColor;
            healthColor = builder.HealthColor;
            damageColor = builder.DamageColor;

            isBoss = builder.IsBoss;
            isPlayer = builder.IsPlayer;

            currentHealth = maxHealth;
        }

        public override void Update(EventList<GameObject> objects) { }

        public override void Draw(SpriteBatch sb)
        {
            if (currentHealth < 0) currentHealth = 0;
            var off = Playing.Instance.offset;
            if (isPlayer || isBoss)
                off = Vector2.Zero;

            sb.Draw(TextureManager.textures["Square"], BorderBounds.add(off), borderColor);
            sb.Draw(TextureManager.textures["Square"], HealthBounds.add(off), healthColor);
            sb.Draw(TextureManager.textures["Square"], DamageBounds.add(off), damageColor);
        }

        public void Heal(float amount)
        {
            currentHealth += amount;

            currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        }

        public void Damage(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
                Destroy();
        }

        public void SetHealth(float amount)
        {
            if (currentHealth < amount) Heal(amount - currentHealth);
            else Damage(currentHealth - amount);
        }

        public void SetMaxHealth(int amount)
        {
            maxHealth = amount;
        }

        public void SetY(float y)
        {
            position.Y = y;
        }

        public void AlignHorizontally(Rectangle value)
        {
            var offset = BorderBounds.Center.X - value.Center.X;
            position.X -= offset;
        }
    }

    public class HealthBarBuilder
    {
        public Vector2 Position { get; set; } = new Vector2(100, 100);
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 15;
        public int MaxHealth { get; set; } = 100;

        public int BorderWidth { get; set; } = 1;
        public Color BorderColor { get; set; } = new Color(200, 200, 200);
        public Color HealthColor { get; set; } = new Color(0, 255, 0);
        public Color DamageColor { get; set; } = new Color(255, 0, 0);

        private bool isBoss;

        public bool IsBoss
        {
            get { return isBoss; }
            set {
                isBoss = true;
                Position = new Vector2(10, 10);
                Width = Game1.BufferWidth - 20;
            }
        }

        private bool isPlayer;

        public bool IsPlayer
        {
            get { return isPlayer; }
            set {
                isPlayer = true;
                Width = 492;
                Height = 28;
                Position = new Vector2(0, 720 - 50);
            }
        }

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

        public HealthBarBuilder() { }

        public HealthBar Build()
        {
            return new HealthBar(this);
        }
    }
}