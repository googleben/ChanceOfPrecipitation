using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    internal class Block : GameObject, ICollider {
        private readonly Texture2D texture;
        private readonly BlockInfo info;
        private RectangleF bounds;

        public Block(float x, float y, string type) {
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, info.src, Color.White);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public void Collide(ICollidable c) {
            var i = RectangleF.Intersect(bounds, c.Bounds());
            if (i.width == 0 || i.height == 0) return;
            if (i.width < i.height) {
                c.Collide((i.x < bounds.x) ? Collision.Right : Collision.Left, i.width, this);
            } else {
                c.Collide((i.y < bounds.y) ? Collision.Top : Collision.Bottom, i.height, this);
            }
        }

    }
}
