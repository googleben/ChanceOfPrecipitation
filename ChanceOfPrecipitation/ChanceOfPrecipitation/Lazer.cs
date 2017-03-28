using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Lazer : GameObject, ICollider {

        private const float Damage = 10f;

        private readonly int width;
        private readonly RectangleF bounds;
        private readonly Direction facing;

        private bool collided;
        private int life;

        private readonly Rectangle lazerSegmentSource = new Rectangle(0, 0, 1, 5);
        private readonly Rectangle lazerEndSource = new Rectangle(1, 0, 1, 5);

        public Lazer(ICollidable origin, int range, int life) {
            this.life = life;

            facing = origin.Facing();

            var blocks = Playing.Instance.objects.OfType<Block>().ToList();

            bounds = new RectangleF(origin.Bounds().Center.X + origin.Bounds().width / 2, origin.Bounds().Center.Y, width, 5);

            while (!collided && width < range) {
                width++;
                if (facing == Direction.Left) bounds.x--;

                bounds.width = width;

                blocks.ForEach(b => {
                    if (b.bounds.Intersects(Bounds()))
                        collided = true;
                });
            }
            bounds = new RectangleF(origin.Bounds().Center.X + origin.Bounds().width / 2, origin.Bounds().Center.Y, width, 5);
        }

        public override void Update(List<GameObject> objects) {
            life--;

            if (life <= 0)
                Destroy();
        }

        public override void Draw(SpriteBatch sb) {
            if (Facing() == Direction.Right)
                for (var i = 0; i < width; i++)
                    sb.Draw(TextureManager.textures["Lazer"], new Rectangle((int)Bounds().x + i, (int)Bounds().x, 1, 5), i == width - 1 ? lazerEndSource : lazerSegmentSource, Color.White);
            else
                for (var i = 0; i > width * -1; i--)
                    sb.Draw(TextureManager.textures["Lazer"], new Rectangle((int)Bounds().x - i, (int)Bounds().x, 1, 5), i == width + 1 ? lazerEndSource : lazerSegmentSource, Color.White);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public Direction Facing() {
            return facing;
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(Bounds()))
                ((Player) c).Damage(Damage);
        }
    }
}
