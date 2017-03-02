using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    class Block : GameObject, ICollider {

        Texture2D texture;
        Rectangle src;
        RectangleF bounds;
        string type;

        public Block(float x, float y, string type) {
            this.type = type;
            this.texture = TextureManager.Textures[type];
            this.src = TextureManager.Sources[type];
            this.bounds = new RectangleF(x, y, src.Width, src.Height);
        }

        public Block(float x, float y, string type, string src)
        {
            this.type = type;
            this.texture = TextureManager.Textures[type];
            this.src = TextureManager.Sources[src];
            this.bounds = new RectangleF(x, y, this.src.Width, this.src.Height);
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, src, Color.White);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public void Collide(ICollidable c) {
            var i = RectangleF.Intersect(bounds, c.Bounds());
            if (i.Width == 0 || i.Height == 0) return;
            if (i.Width < i.Height) {
                c.Collide((i.X < bounds.X) ? Collision.Right : Collision.Left, i.Width, this);
            } else {
                c.Collide((i.Y < bounds.Y) ? Collision.Top : Collision.Bottom, i.Height, this);
            }
        }

    }
}
