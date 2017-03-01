using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public abstract class Ability {

        internal int cd = 0;

        public virtual void Fire(List<GameObject> objects) {
            cd = Cooldown();
        }

        public abstract int Cooldown();

        public void Update() {
            if (cd != 0) cd--;
        }

    }

    public class BulletAbility : Ability {

        private ICollidable origin;

        public override int Cooldown() {
            return 5;
        }

        public BulletAbility(ICollidable origin) {
            this.origin = origin;
        }

        public override void Fire(List<GameObject> objects) {
            if (cd == 0) {
                objects.Add(new Bullet(origin, origin.Facing() == Direction.Left ? origin.Bounds().X - Bullet.Width : origin.Bounds().Right, origin.Bounds().Center.Y, origin.Facing() == Direction.Left ? -10 : 10, 10));
                base.Fire(objects);
            }
            
        }

    }

    public class BurstFireAbility : Ability {

        private ICollidable origin;

        public override int Cooldown() {
            return 30;
        }

        public BurstFireAbility(ICollidable origin) {
            this.origin = origin;
        }

        public override void Fire(List<GameObject> objects) {
            if (cd == 0) {
                objects.Add(new BurstFireDummyObject(origin));
                base.Fire(objects);
            }

        }

        class BurstFireDummyObject : GameObject {

            public const int interval = 3;
            int count = 0;
            ICollidable origin;

            public BurstFireDummyObject(ICollidable origin) {
                this.origin = origin;
            }

            public override void Draw(SpriteBatch sb) {
                //do nothing
            }

            public override void Update(List<GameObject> objects) {
                if (count%interval==0) {
                    objects.Add(new Bullet(origin, origin.Facing() == Direction.Left ? origin.Bounds().X - Bullet.Width : origin.Bounds().Right, origin.Bounds().Center.Y, origin.Facing() == Direction.Left ? -10 : 10, 10));
                }
                if (count == interval * 2) Destroy();
                count++;
            }
        }

    }

    public class Bullet : GameObject, ICollider {

        RectangleF bounds;
        float damage;
        float speed;

        Texture2D texture;

        public const int Width = 3;

        ICollidable origin;

        public Bullet(ICollidable origin, float x, float y, float speed, float damage) {
            texture = TextureManager.Textures["Square"];
            bounds = new RectangleF(x, y, Width, 1);
            this.damage = damage;
            this.speed = speed;
            this.origin = origin;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
        }

        public override void Update(List<GameObject> objects) {
            this.bounds.X += speed;
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public void Collide(ICollidable other) {
            var i = RectangleF.Intersect(bounds, other.Bounds());
            if (i.Width == 0 || i.Height == 0) return;
            Destroy();
            if (other is IEntity)
                if (other != origin) (other as IEntity).Damage(damage);
        }

        public Direction Facing() {
            return speed < 0 ? Direction.Left : Direction.Right;
        }

    }


}
