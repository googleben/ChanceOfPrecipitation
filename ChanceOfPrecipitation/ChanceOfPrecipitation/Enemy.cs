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

        public float maxSpeed = 2f;

        public float jumpSpeed = 10f;

        public float health;

        Direction facing = Direction.Right;

        public Enemy(float x, float y, float width, float height)
        {
            this.bounds = new RectangleF(x, y, width, height);
            this.texture = TextureManager.Textures["HealthBar"];

            healthBar = new HealthBarBuilder(new Vector2(x, y - 20), (int)width + 10, 15).Build();
        }

        public Enemy(float x, float y, float width, float height, float maxSpeed) : this(x, y, width, height)
        {
            this.maxSpeed = maxSpeed;
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
        }

        public void Damage(float amount)
        {
            this.health -= amount;
            healthBar.Damage(amount);
        }

        public void Collide(Collision side, float amount, IStaticObject origin)
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
    }
}