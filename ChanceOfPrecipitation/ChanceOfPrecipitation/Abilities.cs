using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public abstract class Ability {

        internal int cd;

        public virtual void Fire(List<GameObject> objects) {
            cd = Cooldown();
        }

        public abstract int Cooldown();

        public void Update() {
            if (cd != 0) cd--;
        }
    }

    #region Enemy Abilities
    public abstract class EnemyAbility : Ability {

        public abstract bool ShouldFire(List<Player> players);
    }

    public class EnemyMeleeAbility : EnemyAbility {
        private readonly ICollidable origin;

        public EnemyMeleeAbility(ICollidable origin) {
            this.origin = origin;
        }

        public override bool ShouldFire(List<Player> players) {
            return players.Any(p => p.Bounds().Intersects(origin.Bounds())) && cd <= 0;
        }

        public override void Fire(List<GameObject> objects) => objects.OfType<Player>().ToList().ForEach(p => {
            Console.WriteLine("KILL ME");
            if ((origin.Facing() == Direction.Left && p.Bounds().Center.X < origin.Bounds().Center.X) || (origin.Facing() == Direction.Right && p.Bounds().Center.X > origin.Bounds().Center.X))
                p.Damage(10);
        });

        public override int Cooldown() {
            return 60;
        }
    }
    #endregion

    #region Player Abilities
    public class BulletAbility : Ability {

        private readonly ICollidable origin;
        public int damage = 10;

        public override int Cooldown() {
            return 5;
        }

        public BulletAbility(ICollidable origin) {
            this.origin = origin;
        }

        public override void Fire(List<GameObject> objects) {
            if (cd == 0) {
                objects.Add(new Bullet(origin, origin.Facing() == Direction.Left ? origin.Bounds().x - Bullet.Width : origin.Bounds().Right, origin.Bounds().Center.Y, origin.Facing() == Direction.Left ? -10 : 10, damage));
                base.Fire(objects);
            }
            
        }

    }

    public class BurstFireAbility : Ability {

        private readonly ICollidable origin;
        public int damage = 10;

        public override int Cooldown() {
            return 30;
        }

        public BurstFireAbility(ICollidable origin) {
            this.origin = origin;
        }

        public override void Fire(List<GameObject> objects) {
            if (cd == 0) {
                objects.Add(new BurstFireDummyObject(origin, damage));
                base.Fire(objects);
            }

        }

        private class BurstFireDummyObject : GameObject {
            private const int Interval = 3;
            private int count;
            private readonly ICollidable origin;
            private int damage;

            public BurstFireDummyObject(ICollidable origin, int damage) {
                this.origin = origin;
                this.damage = damage;
            }

            public override void Draw(SpriteBatch sb) {
                //do nothing
            }

            public override void Update(List<GameObject> objects) {
                if (count%Interval==0) {
                    objects.Add(new Bullet(origin, origin.Facing() == Direction.Left ? origin.Bounds().x - Bullet.Width : origin.Bounds().Right, origin.Bounds().Center.Y, origin.Facing() == Direction.Left ? -10 : 10, damage));
                }
                if (count == Interval * 2) Destroy();
                count++;
            }
        }

    }
    #endregion

    public class Bullet : GameObject, ICollider {
        private RectangleF bounds;
        private readonly float damage;
        private readonly float speed;

        private readonly Texture2D texture;

        public const int Width = 3;

        private readonly ICollidable origin;

        public Bullet(ICollidable origin, float x, float y, float speed, float damage) {
            texture = TextureManager.textures["Square"];
            bounds = new RectangleF(x, y, Width, 1);
            this.damage = damage;
            this.speed = speed;
            this.origin = origin;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
        }

        public override void Update(List<GameObject> objects) {
            bounds.x += speed;
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public void Collide(ICollidable other) {
            var i = RectangleF.Intersect(bounds, other.Bounds());
            if (i.width == 0 || i.height == 0) return;
            Destroy();
            if (other is IEntity)
                if (other != origin) (other as IEntity).Damage(damage);
        }

        public Direction Facing() {
            return speed < 0 ? Direction.Left : Direction.Right;
        }

    }


}
