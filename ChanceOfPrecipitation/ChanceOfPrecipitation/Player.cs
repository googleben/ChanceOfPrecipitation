using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class Player : GameObject, ICollidable, IEntity {

        RectangleF bounds;
        Vector2 velocity;
        Collision collision;

        HealthBar healthBar;

        Keys left = Keys.Left;
        Keys right = Keys.Right;
        Keys up = Keys.Up;
        Keys down = Keys.Down;
        Keys jump = Keys.Space;
        Keys abilityOneKey = Keys.J;

        Ability abilityOne;

        Texture2D texture;

        public float maxSpeed = 5f;

        public float jumpSpeed = 10f;

        public float health = MaxHealth;

        public const float MaxHealth = 100;

        Direction facing = Direction.Right;

        public Player(float x, float y, float width, float height) {
            this.bounds = new RectangleF(x, y, width, height);
            this.texture = TextureManager.Textures["HealthBar"];

            healthBar = new HealthBarBuilder() { Position = new Vector2(x, y), Width = (int)width + 10 }.Build();

            abilityOne = new BulletAbility(this);
        }

        public override void Update(List<GameObject> objects) {
            var state = Playing.Instance.state;

            abilityOne.Update();
            
            if (state.IsKeyDown(left)) {
                facing = Direction.Left;
                this.velocity.X = -maxSpeed;
            } else if (state.IsKeyDown(right)) {
                facing = Direction.Right;
                this.velocity.X = maxSpeed;
            } else {
                this.velocity.X = 0;
            }

            if (state.IsKeyDown(jump) && collision.HasFlag(Collision.Bottom)) {
                this.velocity.Y = -jumpSpeed;
            }

            if (state.IsKeyDown(abilityOneKey)) abilityOne.Fire(objects); // TODO: Add ability

            collision = Collision.None;

            this.velocity += Playing.Instance.gravity;
            this.bounds.X += velocity.X;
            this.bounds.Y += velocity.Y;

            healthBar.AlignHorizontally((Rectangle)Bounds());
            healthBar.SetY(Bounds().Y - 20);
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

        public void Damage(float amount) {
            this.health -= amount;
        }

        public void Collide(Collision side, float amount, IStaticObject origin)
        {
            collision |= side;

            if (side == Collision.Right) {
                this.bounds.X -= amount;
                this.velocity.X = 0;
            }
            else if (side == Collision.Left) {
                this.bounds.X += amount;
                this.velocity.X = 0;
            }
            else if (side == Collision.Top) {
                this.bounds.Y += amount;
                this.velocity.Y = 0;
            }
            else if (side == Collision.Bottom) {
                this.bounds.Y -= amount;
                this.velocity.Y = 0;
            }
        }

        public Direction Facing() {
            return facing;
        }
    }
}
