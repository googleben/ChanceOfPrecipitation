using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public abstract class Enemy : GameObject, ICollidable, IEntity
    {
        protected RectangleF bounds;
        protected Vector2 velocity;
        protected Collision collision;

        protected readonly HealthBar healthBar;

        public EnemyAbility[] abilities;

        protected Texture2D texture;

        public float maxSpeed = 2f;

        public float jumpSpeed = 10f;

        public float health;

        protected Direction facing = Direction.Right;

        protected float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            set
            {
                maxHealth = value;
                health = maxHealth;
                healthBar.SetMaxHealth((int)maxHealth);
            }
        }

        protected readonly FloatingIndicatorBuilder damageBuilder;

        public Enemy(float x, float y, float width, float height)
        {
            bounds = new RectangleF(x, y, width, height);
            maxHealth = 100;

            healthBar = new HealthBarBuilder(new Vector2(x, y - 20), (int)width + 20, 5).Build();

            damageBuilder = new FloatingIndicatorBuilder { Color = Color.Red };

            abilities = new EnemyAbility[] { new EnemyMeleeAbility(this) };
        }

        public Enemy(float x, float y, float width, float height, float maxSpeed) : this(x, y, width, height)
        {
            this.maxSpeed = maxSpeed;
        }

        public override void Update(List<GameObject> objects) {
            Player target = null;
            var min = float.PositiveInfinity;
            foreach (var p in Playing.Instance.players) {
                var dist = Math.Abs(p.Bounds().x - bounds.x);
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

            if (target.Bounds().y < Bounds().y && collision.HasFlag(Collision.Bottom) && distanceThreshold) {
                velocity.Y = -jumpSpeed;
            }

            foreach (var e in abilities) {
                e.Update();
                
                if (e.ShouldFire(Playing.Instance.players))
                    e.Fire(objects);
            }

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
            //Console.WriteLine(side);
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
