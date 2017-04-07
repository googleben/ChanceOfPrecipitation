using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
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

        private Rectangle HealthBounds => new Rectangle((int)(position.X + borderWidth), (int)(position.Y + borderWidth), HealthWidth - borderWidth*2, height - borderWidth * 2);
        private Rectangle DamageBounds => new Rectangle((int)(HealthWidth + position.X), (int)(position.Y + borderWidth), width - HealthWidth - borderWidth, height - borderWidth * 2);
        private Rectangle BorderBounds => new Rectangle((int)position.X, (int)position.Y, width, height);
        private int HealthWidth => (int)(currentHealth * width / maxHealth);

        private bool isBoss;
        public bool IsBoss => isBoss;

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

            currentHealth = maxHealth;
        }

        public override void Update(List<GameObject> objects)
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            //System.Console.WriteLine(borderWidth);
            sb.Draw(TextureManager.textures["Square"], BorderBounds, borderColor);
            sb.Draw(TextureManager.textures["Square"], HealthBounds, healthColor);
            sb.Draw(TextureManager.textures["Square"], DamageBounds, damageColor);
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

        public void SetHealth(float amount) {
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
}
