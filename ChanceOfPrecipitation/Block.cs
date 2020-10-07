using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Block : GameObject, ICollider
    {
        private readonly Texture2D texture;
        private readonly TextureInfo info;
        public RectangleF bounds;
        public string type;

        public Block(float x, float y, string type)
        {
            this.type = type;

            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public override void Update(EventList<GameObject> objects) { }

        private const float Tol = 0.2f;

        public void Collide(ICollidable c)
        {
            var i = RectangleF.Intersect(bounds, c.Bounds());
            if (Math.Abs(i.width) < Tol || Math.Abs(i.height) < Tol) return;
            //special case collisions

            if (type == "stage1_platform_top_middle") c.Collide(Collision.Bottom, i.height, this);
            else if (type == "stage1_platform_middle_left") c.Collide(Collision.Left, i.width, this);
            else if (type == "stage1_platform_middle_right") c.Collide(Collision.Right, i.width, this);
            else if (type == "stage1_platform_bottom_middle") c.Collide(Collision.Top, i.height, this);
            else if (i.width < i.height) {
                c.Collide(i.x > bounds.x + 1 ? Collision.Right : Collision.Left, i.width, this);
            } else {
                c.Collide(i.y > bounds.y + 1 ? Collision.Top : Collision.Bottom, i.height, this);
            }
        }

        public RectangleF Bounds()
        {
            return bounds;
        }
    }

    public class InvisibleBlock : Block
    {

        public InvisibleBlock(float x, float y) : base(x, y, "invisibleBlock") { }

        public override void Draw(SpriteBatch sb)
        {
            //do nothing
        }

    }

    public class Rope : GameObject, ICollider
    {
        private int length;
        private float x;
        private float y;

        RopeSegment head;

        public Rope(float x, float y, int length)
        {
            this.length = length;
            this.x = x;
            this.y = y;

            head = new RopeSegment(x, y, "ropeTop", true);

            for (var i = 1; i < length; i++) {
                head.next = new RopeSegment(x, y + 32 * i, "ropeMid");
                head.next.prev = head;
                head = head.next;
            }

            head.next = new RopeSegment(x, head.bounds.y + 32, "ropeBot");
            head.next.prev = head;
            head = head.next;
            while (head.prev != null) head = head.prev;
        }

        public override void Update(EventList<GameObject> objects)
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

        public RectangleF Bounds()
        {
            return new RectangleF(x, y, 4, 6 + (32 * length));
        }
    }

    public class RopeSegment : GameObject, ICollider
    {
        public RectangleF bounds;
        TextureInfo info;
        Texture2D texture;

        public RopeSegment next;
        public RopeSegment prev;

        public RopeSegment(float x, float y, string info, bool isHead = false)
        {
            this.info = TextureManager.blocks[info];
            texture = TextureManager.textures[this.info.texName];
            if (!isHead)
                bounds = new RectangleF(x + 14, y, 4, 32);
            else
                bounds = new RectangleF(x + 14, y + 26, 4, 6);
        }

        public override void Update(EventList<GameObject> objects) { }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public void Collide(ICollidable c)
        {
            if (c is Player) {
                var p = c as Player;
                if (p.Bounds().Intersects(bounds)) {
                    var state = Playing.Instance.state;

                    /*if (state.IsKeyDown(p.jump) && p.rope != null)
                    {
                        p.ropeCollide = false;
                        Playing.Instance.quad.RunCollision(x => !(x is Rope));

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
                    }*/
                    if (p.rope == null && (state.IsKeyDown(p.down) || state.IsKeyDown(p.up)))
                        p.rope = this;
                }

                /*if (p.rope == this)
                {
                    if (p.Bounds().Bottom < bounds.Top)
                    {
                        p.rope = prev;
                    }
                    else if (p.Bounds().Top > bounds.Bottom)
                    {
                        p.rope = next;
                    }
                }*/
            }
        }

        public void UpdatePlayer(Player p)
        {
            if (Playing.Instance.state.IsKeyDown(p.jump) && !Playing.Instance.lastState.IsKeyDown(p.jump) &&
                p.rope != null) {
                p.ropeCollide = false;
                Playing.Instance.quad.RunCollision(x => !(x is Rope));

                if (!p.ropeCollide) {
                    p.rope = null;
                    p.Jump();
                }
            } else {
                if (p.Bounds().Bottom < bounds.Top) {
                    p.rope = prev;
                } else if (p.Bounds().Top > bounds.Bottom) {
                    p.rope = next;
                }
            }
        }

        public RectangleF Bounds()
        {
            return bounds;
        }
    }

    public class Facade : GameObject
    {
        private readonly Texture2D texture;
        private readonly TextureInfo info;
        public RectangleF bounds;
        public string type;

        public Facade(float x, float y, string type)
        {
            this.type = type;

            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public override void Update(EventList<GameObject> objects) { }
    }
}