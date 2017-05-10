using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public class Block : GameObject, ICollider {
        private readonly Texture2D texture;
        private readonly TextureInfo info;
        public RectangleF bounds;
        private string type;

        public Block(float x, float y, string type) {
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        private const float Tol = 0.2f;

        public void Collide(ICollidable c) {
            var i = RectangleF.Intersect(bounds, c.Bounds());
            if (Math.Abs(i.width) < Tol || Math.Abs(i.height) < Tol) return;
            if (i.width < i.height) {
                c.Collide(i.x > bounds.x+1 ? Collision.Right : Collision.Left, i.width, this);
            } else {
                c.Collide(i.y > bounds.y+1 ? Collision.Top : Collision.Bottom, i.height, this);
            }
        }
    }

    public class Rope : GameObject, ICollider
    {
        private int length;

        RopeSegment head;

        public Rope(float x, float y, int length) {
            this.length = length;

            head = new RopeSegment(x, y, "ropeTop");

            for (var i = 1; i < length; i++)
            {
                head.next = new RopeSegment(x, y + 32 * i, "ropeMid");
                head.next.prev = head;
                head = head.next;
            }

            head.next = new RopeSegment(x, head.bounds.y + 32, "ropeBot");
            head.next.prev = head;
            head = head.next;
            while (head.prev != null) head = head.prev;
        }

        public override void Update(List<GameObject> objects)
        {
            for (var curr = head; curr != null; curr = curr.next)
                curr.Update(objects);
        }

        public override void Draw(SpriteBatch sb)
        {
            for (var curr = head; curr != null; curr = curr.next)
                curr.Draw(sb);
        }

        public void Collide(ICollidable c)
        {
            for (var curr = head; curr != null; curr = curr.next)
                curr.Collide(c);
        }
    }

    public class RopeSegment : GameObject, ICollider {
        public RectangleF bounds;
        TextureInfo info;
        Texture2D texture;

        public RopeSegment next;
        public RopeSegment prev;

        public RopeSegment(float x, float y, string info) {
            this.info = TextureManager.blocks[info];
            texture = TextureManager.textures[this.info.texName];
            bounds = new RectangleF(x + 14, y, 4, 32);
        }

        public override void Update(List<GameObject> objects)
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public void Collide(ICollidable c)
        {
            if (c is Player)
            {
                var p = c as Player;
                if (c.Bounds().Intersects(bounds))
                {
                    var state = Playing.Instance.state;

                    if (state.IsKeyDown(p.jump))
                    {
                        p.ropeCollide = false;
                        foreach (var s in Playing.Instance.objects.OfType<ICollider>().Where(a => !(a is Rope)).ToList())
                            foreach (var x in Playing.Instance.objects.OfType<ICollidable>().ToList())
                                s.Collide(x);

                        if (!p.ropeCollide)
                        {
                            p.rope = null;
                            p.Jump();
                        }
                    }
                    else if (p.rope != prev || p.rope != next)
                    {
                        if (state.IsKeyDown(p.down) || state.IsKeyDown(p.up))
                            p.rope = this;
                    }
                }

                if (p.rope == this)
                {
                    if (c.Bounds().Bottom < bounds.Top)
                    {
                        p.rope = prev;
                    }
                    else if (c.Bounds().Top > bounds.Bottom)
                    {
                        p.rope = next;
                    }
                }
            }
        }

        public override string ToString()
        {
            return "" + bounds.Center;
        }
    }
}
