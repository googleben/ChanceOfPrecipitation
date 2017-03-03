using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class Enemy : GameObject, ICollidable, IEntity
    {

        RectangleF bounds;
        Vector2 velocity;
        Collision collision;

        HealthBar healthBar;

        Ability abilityOne;

        Texture2D texture;

        private float maxSpeed;

        private float jumpSpeed;

        private float health;

        private Direction facing;

        private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            set
            {
                this.maxHealth = value;
                this.healthBar.SetMaxHealth((int)maxHealth);
            }
        }

        private FloatingIndicatorBuilder damageBuilder;
        private FloatingIndicatorBuilder healBuilder;

        public Enemy(EnemyBuilder builder)
        {
            bounds = new RectangleF(builder.X, builder.Y, builder.Width, builder.Height);
            texture = TextureManager.Textures["Square"];

            facing = builder.Facing;

            maxHealth = builder.MaxHealth;
            health = maxHealth;
            healthBar = new HealthBarBuilder(new Vector2(builder.X, builder.Y - 20), (int)builder.Width + 20, 5).Build();

            damageBuilder = new FloatingIndicatorBuilder() { Color = Color.Red };
            healBuilder = new FloatingIndicatorBuilder() { Color = Color.Lavender };
        }

        public override void Update(List<GameObject> objects)
        {
            var state = Playing.Instance.state;

            if (Playing.Instance.player.Bounds().X < Bounds().X)
            {
                facing = Direction.Left;
                this.velocity.X = -maxSpeed;
            }
            else if (Playing.Instance.player.Bounds().X > Bounds().X)
            {
                facing = Direction.Right;
                this.velocity.X = maxSpeed;
            }
            else
            {
                this.velocity.X = 0;
            }

            if (Playing.Instance.player.Bounds().Y < Bounds().Y && collision.HasFlag(Collision.Bottom) && Math.Abs(Playing.Instance.player.Bounds().X - Bounds().X) < Playing.Instance.player.Bounds().Width * 2)
            {
                this.velocity.Y = -jumpSpeed;
            }

            //if (state.IsKeyDown(abilityOneKey)) ; // TODO: Add ability

            collision = Collision.None;

            this.velocity += Playing.Instance.gravity;
            this.bounds.X += velocity.X;
            this.bounds.Y += velocity.Y;

            healthBar.AlignHorizontally((Rectangle)Bounds());
            healthBar.SetY(Bounds().Y - 20);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle)bounds, Color.Red);
            healthBar.Draw(sb);
        }

        public RectangleF Bounds()
        {
            return bounds;
        }

        public float Health()
        {
            return health;
        }

        public void Heal(float amount)
        {
            this.health += amount;
            healthBar.Heal(amount);

            Playing.Instance.objects.Add(healBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.Y)));
        }

        public void Damage(float amount)
        {
            this.health -= amount;
            healthBar.Damage(amount);

            Playing.Instance.objects.Add(damageBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.Y)));

            if (this.health <= 0)
                Destroy();
        }

        public void Collide(Collision side, float amount, ICollider origin)
        {
            Console.WriteLine(side);
            collision |= side;

            if (side == Collision.Right)
            {
                this.bounds.X -= amount;
                this.velocity.X = 0;
            }
            else if (side == Collision.Left)
            {
                this.bounds.X += amount;
                this.velocity.X = 0;
            }
            else if (side == Collision.Top)
            {
                this.bounds.Y += amount;
                this.velocity.Y = 0;
            }
            else if (side == Collision.Bottom)
            {
                this.bounds.Y -= amount;
                this.velocity.Y = 0;
            }
        }

        public Direction Facing()
        {
            return facing;
        }

        public Enemy Clone(float x, float y) {
            var ans = (Enemy)this.MemberwiseClone();
            ans.bounds.X = x;
            ans.bounds.Y = y;
            return ans;
        }

    }
}