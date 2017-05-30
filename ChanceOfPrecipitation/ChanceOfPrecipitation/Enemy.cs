using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public abstract class Enemy : GameObject, ICollidable, IEntity, IValuable {
        protected RectangleF bounds;
        protected Vector2 velocity;
        public Collision collision;

        protected HealthBar healthBar;
        protected Color color;

        public EnemyAbility[] abilities;

        protected Texture2D texture;

        public int value = 2;

        public float maxSpeed = 2f;

        public float jumpSpeed = 10f;

        public float health;

        public bool canMove = true;

        protected Direction facing = Direction.Right;

        protected float maxHealth;

        public float MaxHealth {
            get { return maxHealth; }
            set {
                maxHealth = value;
                health = maxHealth;
                healthBar.SetMaxHealth((int) maxHealth);
            }
        }

        protected readonly FloatingIndicatorBuilder damageBuilder;

        protected Enemy(float x, float y, float width, float height) {
            bounds = new RectangleF(x, y, width, height);
            color = Color.Red;

            healthBar = new HealthBarBuilder(new Vector2(x, y - 20), (int) width + 20, 5).Build();

            MaxHealth = 100;

            damageBuilder = new FloatingIndicatorBuilder {Color = Color.Red};
        }

        protected Enemy(float x, float y, float width, float height, float maxSpeed) : this(x, y, width, height) {
            this.maxSpeed = maxSpeed;
        }

        public override void Update(EventList<GameObject> objects) {
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
            if (target.Bounds().x < Bounds().x) {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
            }
            else if (target.Bounds().x > Bounds().x) {
                facing = Direction.Right;
                velocity.X = maxSpeed;
            }
            else {
                velocity.X = 0;
            }

            var distanceThreshold = Math.Abs(target.Bounds().x - Bounds().x) < target.Bounds().width * 2;

            if (target.Bounds().y < Bounds().y && collision.HasFlag(Collision.Bottom) && distanceThreshold) {
                velocity.Y = -jumpSpeed;
            }

            UpdateAbilities(objects);

            collision = Collision.None;

            UpdatePosition();

            UpdateHealthBar();
        }

        protected void UpdatePosition() {
            velocity += Playing.Instance.gravity;
            velocity.Y = MathHelper.Clamp(velocity.Y, -15, 15);
            if (!canMove) velocity = Vector2.Zero;
            bounds.x += velocity.X;
            bounds.y += velocity.Y;
        }

        protected void UpdateAbilities(EventList<GameObject> objects) {
            foreach (var e in abilities) {
                e.Update();

                if (e.ShouldFire(Playing.Instance.players))
                    e.Fire(objects);
            }
        }

        protected void UpdateHealthBar() {
            if (!healthBar.IsBoss) {
                healthBar.AlignHorizontally((Rectangle) (bounds));
                healthBar.SetY((bounds.y - 20));
            }
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), color);
            healthBar.Draw(sb);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public float Health() {
            return health;
        }

        public void Heal(float amount) {
            health += amount;
            healthBar.Heal(amount);
        }

        public void Damage(float amount) {
            health -= amount;
            healthBar.Damage(amount);

            Playing.Instance.objects.Add(damageBuilder.Build((int) amount, new Vector2(bounds.Center.X, bounds.y)));

            if (health <= 0) {
                if (Playing.random.NextDouble() < ChanceToDropItem()) {
                    var drop = Item.items[Playing.random.Next(Item.items.Count)].Clone();
                    (drop as IItemEntity)?.SetPos(bounds.Center.X, bounds.Center.Y);
                    Playing.Instance.objects.Add(drop);
                }
                Destroy();
                DropCoins();
            }
        }

        public void Collide(Collision side, float amount, ICollider origin) {
            //Console.WriteLine(side);
            collision |= side;

            if (side == Collision.Right) {
                bounds.x += amount;
                velocity.X = 0;
            }
            else if (side == Collision.Left) {
                bounds.x -= amount;
                velocity.X = 0;
            }
            else if (side == Collision.Top) {
                bounds.y += amount;
                velocity.Y = 0;
            }
            else if (side == Collision.Bottom) {
                bounds.y -= amount;
                velocity.Y = 0;
            }
        }

        public Direction Facing() {
            return facing;
        }

        // Chance to drop an item as a part of 1
        // .1 = 10%
        // 1 = 100%
        float ChanceToDropItem() {
            return .1f;
        }

        public int Value() {
            return value;
        }

        public void DropCoins() {
            for (var i = 0; i < Value(); i++)
                Playing.Instance.objects.Add(new Coin(bounds.Center.X, bounds.Center.Y));
        }

        public void SetPos(float x, float y) {
            bounds.x = x;
            bounds.y = y;
        }
    }
}