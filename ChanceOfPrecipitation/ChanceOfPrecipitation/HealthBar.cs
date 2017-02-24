using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    class HealthBar : GameObject
    {
        public Vector2 position;
        public int width;
        public int height;
        private int maxHealth;
        private float currentHealth;

        private int borderWidth;
        private Color borderColor;
        private Color healthColor;
        private Color damageColor;

        private Rectangle HealthBounds => new Rectangle((int)(position.X + borderWidth), (int)(position.Y + borderWidth), HealthWidth - borderWidth, height - (borderWidth * 2));
        private Rectangle DamageBounds => new Rectangle((int)(HealthWidth + position.X), (int)(position.Y + borderWidth), width - HealthWidth - borderWidth, height - (borderWidth * 2));
        private Rectangle BorderBounds => new Rectangle((int)position.X, (int)position.Y, width, height);
        private int HealthWidth => (int)(currentHealth * width / maxHealth);

        /// <summary>
        /// Initialize a new instance of <see cref="HealthBar"/>.
        /// </summary>
        /// <param name="position">The initial position of the <see cref="HealthBar"/>.</param>
        /// <param name="width">The full width of the <see cref="HealthBar"/>, including the border.</param>
        /// <param name="height">The full height of the <see cref="HealthBar"/>, including the border.</param>
        /// <param name="maxHealth">The maximum health of the <see cref="HealthBar"/> instance.</param>
        /// <param name="borderWidth">The border width of the <see cref="HealthBar"/> instance.</param>
        /// <param name="borderColor">The border color of the <see cref="HealthBar"/> instance.</param>
        /// <param name="healthColor">The color of the health indicator of the <see cref="HealthBar"/> instance.</param>
        /// <param name="damageColor">The color of the damage indicator of the <see cref="HealthBar"/> instance.</param>
        public HealthBar(Vector2 position, int width, int height, int maxHealth, int borderWidth, Color borderColor, Color healthColor, Color damageColor)
        {
            this.position = position;
            this.width = width;
            this.height = height;
            this.maxHealth = maxHealth;
            this.borderWidth = borderWidth;
            this.borderColor = borderColor;
            this.healthColor = healthColor;
            this.damageColor = damageColor;
            currentHealth = maxHealth;
        }

        public HealthBar(int width, int height) : this(Vector2.Zero, width, height, 100, 3, new Color(128, 128, 128), new Color(0, 255, 0), new Color(255, 0, 0)) { }

        public override void Update(IEnumerable<GameObject> objects)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(TextureManager.Textures["HealthBar"], BorderBounds, borderColor);
            sb.Draw(TextureManager.Textures["HealthBar"], HealthBounds, healthColor);
            sb.Draw(TextureManager.Textures["HealthBar"], DamageBounds, damageColor);
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

        public void AlignHorizontally(Rectangle value)
        {
            var offset = BorderBounds.Center.X - value.Center.X;
            position.X -= offset;
        }
    }
}
