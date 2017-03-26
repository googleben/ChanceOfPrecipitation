using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Lazer : GameObject, ICollider {

        private ICollidable origin;
        private int width = 0;

        private bool collided;
        private int range;
        private int life;

        public Lazer(ICollidable origin, int range, int life) {
            this.origin = origin;
            this.range = range;
            this.life = life;

            var blocks = Playing.Instance.objects.OfType<Block>().ToList();

            while (!collided && width < range) {
                width++;

                blocks.ForEach(b => {
                    if (b.bounds.Intersects(Bounds()))
                        collided = true;
                });
            }
        }

        public override void Update(List<GameObject> objects) {
            life--;

            if (life <= 0)
                Destroy();
        }

        public override void Draw(SpriteBatch sb) {
            for (int i = 0; i < width; i++) {
                
            }
        }

        public RectangleF Bounds() {
            throw new NotImplementedException();
        }

        public Direction Facing() {
            throw new NotImplementedException();
        }

        public void Collide(ICollidable c) {
            throw new NotImplementedException();
        }
    }
}
