using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class Player : GameObject, Collidable {

        RectangleF bounds;
        Vector2 velocity;
        Collision collision;

        const float speed = 1f;

        Keys left = Keys.Left;
        Keys right = Keys.Right;
        Keys up = Keys.Up;
        Keys down = Keys.Down;
        Keys jump = Keys.Space;

        Texture2D texture;

        public float maxSpeed = 5f;

        public Player(float x, float y, float width, float height) {
            this.bounds = new RectangleF(x, y, width, height);
            this.texture = TextureManager.Textures["HealthBar"];
        }

        public override void Update(IEnumerable<GameObject> objects) {
            var state = Playing.Instance.state;
            
            if (state.IsKeyDown(left)) {
                this.velocity.X = -maxSpeed;
            } else if (state.IsKeyDown(right)) {
                this.velocity.X = maxSpeed;
            } else {
                this.velocity.X = 0;
            }
            this.velocity += Playing.Instance.gravity;
            this.bounds.X += velocity.X;
            this.bounds.Y += velocity.Y;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
        }

        public void Collide(Collision side, float amount) {
            Console.WriteLine(side);
            collision |= side;
            if (side==Collision.Right) {
                this.bounds.X -= amount;
                this.velocity.X = 0;
            } else if (side==Collision.Left) {
                this.bounds.X += amount;
                this.velocity.X = 0;
            } else if (side==Collision.Top) {
                this.bounds.Y += amount;
                this.velocity.Y = 0;
            } else if (side==Collision.Bottom) {
                this.bounds.Y -= amount;
                this.velocity.Y = 0;
            }
        }

        public RectangleF Bounds() {
            return bounds;
        }

    }
}
