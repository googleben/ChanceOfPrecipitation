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
                objects.Add(new Bullet(origin.Facing() == Direction.Left ? origin.Bounds().X - Bullet.Width : origin.Bounds().Right, origin.Bounds().Center.Y, 10, 10));
                base.Fire(objects);
            }
            
        }

    }

    public class Bullet : GameObject, ICollidable {

        RectangleF bounds;
        float damage;
        float speed;

        Texture2D texture;

        public const int Width = 3;

        public Bullet(float x, float y, float speed, float damage) {
            texture = TextureManager.Textures["Square"];
            bounds = new RectangleF(x, y, Width, 1);
            this.damage = damage;
            this.speed = speed;
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

        public void Collide(Collision side, float amount, IStaticObject origin) {
            if (origin is IEntity) (origin as IEntity).Damage(damage);
            Destroy();
        }

        public Direction Facing() {
            return speed < 0 ? Direction.Left : Direction.Right;
        }

    }


}
