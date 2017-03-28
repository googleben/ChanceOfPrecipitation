using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Lazer : GameObject, ICollider {

        private const float damage = 20f;

        private ICollidable origin;
        private int width = 0;
        private RectangleF bounds;

        private bool collided;
        private int life;

        private readonly Rectangle lazerSegmentSource = new Rectangle(0, 0, 1, 5);
        private readonly Rectangle lazerEndSource = new Rectangle(1, 0, 1, 5);

        public Lazer(ICollidable origin, int range, int life) {
            this.origin = origin;
            this.life = life;

            var blocks = Playing.Instance.objects.OfType<Block>().ToList();

            while (!collided && width < range) {
                width++;

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
            for (var i = 0; i < width; i++)
                sb.Draw(TextureManager.textures["Lazer"], new Rectangle((int)Bounds().x + i, (int)Bounds().y, 1, 5), i == width - 1 ? lazerEndSource : lazerSegmentSource, Color.White);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public Direction Facing() {
            return origin.Facing();
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(Bounds()))
                ((Player) c).Damage(damage);
        }
    }
}
