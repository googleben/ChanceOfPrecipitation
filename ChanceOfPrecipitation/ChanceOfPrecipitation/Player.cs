using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class Player : GameObject, ICollidable, IEntity {
        private RectangleF bounds;
        private Vector2 velocity;
        private Collision collision;

        private readonly HealthBar healthBar;

        private readonly Keys left = Keys.Left;
        private readonly Keys right = Keys.Right;
        private Keys up = Keys.Up;
        private Keys down = Keys.Down;
        private readonly Keys jump = Keys.Space;
        private readonly Keys abilityOneKey = Keys.J;

        private readonly Ability abilityOne;

        private readonly Texture2D texture;

        public float maxSpeed = 5f;

        public float jumpSpeed = 10f;

        public float health = MaxHealth;

        public const float MaxHealth = 100;

        private Direction facing = Direction.Right;

        private readonly FloatingIndicatorBuilder healBuilder;
        private readonly FloatingIndicatorBuilder damageBuilder;

        public Player(float x, float y, float width, float height) {
            bounds = new RectangleF(x, y, width, height);
            texture = TextureManager.textures["Square"];

            healthBar = new HealthBarBuilder() { Position = new Vector2(x, y), Width = (int)width + 10 }.Build();

            abilityOne = new BurstFireAbility(this);

            healBuilder = new FloatingIndicatorBuilder() { Color = Color.Lavender };
            damageBuilder = new FloatingIndicatorBuilder() { Color = Color.Red };
        }

        public override void Update(List<GameObject> objects) {
            var state = Playing.Instance.state;

            abilityOne.Update();
            
            if (state.IsKeyDown(left)) {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
            } else if (state.IsKeyDown(right)) {
                facing = Direction.Right;
                velocity.X = maxSpeed;
            } else {
                velocity.X = 0;
            }

            if (state.IsKeyDown(jump) && collision.HasFlag(Collision.Bottom)) {
                velocity.Y = -jumpSpeed;
            }

            if (state.IsKeyDown(abilityOneKey)) abilityOne.Fire(objects); // TODO: Add ability

            collision = Collision.None;

            velocity += Playing.Instance.gravity;
            bounds.x += velocity.X;
            bounds.y += velocity.Y;

            healthBar.AlignHorizontally((Rectangle)Bounds());
            healthBar.SetY(Bounds().y - 20);
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
            healthBar.Draw(sb);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public float Health() {
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
            collision |= side;

            if (side == Collision.Right) {
                bounds.x -= amount;
                velocity.X = 0;
            }
            else if (side == Collision.Left) {
                bounds.x += amount;
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
    }
}
