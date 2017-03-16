using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Enemy : GameObject, ICollidable, IEntity
    {

        public RectangleF bounds;
        private Vector2 velocity;
        private Collision collision;

        private readonly HealthBar healthBar;

        private Ability abilityOne;

        private readonly Texture2D texture;

        private readonly float maxSpeed;

        private float jumpSpeed;

        private float health;

        private Direction facing;

        private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            set
            {
                maxHealth = value;
                healthBar.SetMaxHealth((int)maxHealth);
            }
        }

        private readonly FloatingIndicatorBuilder healBuilder;
        private readonly FloatingIndicatorBuilder damageBuilder;

        public Enemy(EnemyBuilder builder) {
            bounds = new RectangleF(builder.X, builder.Y, builder.Width, builder.Height);
            texture = TextureManager.textures["Square"];

            maxSpeed = builder.MaxSpeed;

            facing = builder.Facing;

            maxHealth = builder.MaxHealth;
            health = maxHealth;
            healthBar = new HealthBarBuilder(new Vector2(builder.X, builder.Y - 20), (int)builder.Width + 20, 5).Build();

            damageBuilder = new FloatingIndicatorBuilder() { Color = Color.Red };
            healBuilder = new FloatingIndicatorBuilder() { Color = Color.Lavender };
        }

        public override void Update(List<GameObject> objects) {
            Player target = null;
            float min = float.PositiveInfinity;
            foreach (Player p in Playing.Instance.players) {
                float dist = Math.Abs(p.Bounds().x - bounds.x);
                if (dist < min) {
                    min = dist;
                    target = p;
                }
            }
            if (target == null) return;
            if (target.Bounds().x < Bounds().x)
            {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
            }
            else if (target.Bounds().x > Bounds().x)
            {
                facing = Direction.Right;
                velocity.X = maxSpeed;
            }
            else
            {
                velocity.X = 0;
            }

            var distanceThreshold = Math.Abs(target.Bounds().x - Bounds().x) < target.Bounds().width * 2;

            if (target.Bounds().y < Bounds().y && collision.HasFlag(Collision.Bottom) && distanceThreshold)
            {
                velocity.Y = -jumpSpeed;
            }

            //TODO: Add ability

            collision = Collision.None;

            velocity += Playing.Instance.gravity;
            bounds.x += velocity.X;
            bounds.y += velocity.Y;

            healthBar.AlignHorizontally((Rectangle)Bounds());
            healthBar.SetY(Bounds().y - 20);
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
            health += amount;
            healthBar.Heal(amount);

            Playing.Instance.objects.Add(healBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));
        }

        public void Damage(float amount)
        {
            health -= amount;
            healthBar.Damage(amount);

            Playing.Instance.objects.Add(damageBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));

            if (health <= 0)
                Destroy();
        }

        public void Collide(Collision side, float amount, ICollider origin)
        {
            Console.WriteLine(side);
            collision |= side;

            if (side == Collision.Right)
            {
                bounds.x -= amount;
                velocity.X = 0;
            }
            else if (side == Collision.Left)
            {
                bounds.x += amount;
                velocity.X = 0;
            }
            else if (side == Collision.Top)
            {
                bounds.y += amount;
                velocity.Y = 0;
            }
            else if (side == Collision.Bottom)
            {
                bounds.y -= amount;
                velocity.Y = 0;
            }
        }

        public Direction Facing()
        {
            return facing;
        }
    }
}