using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public abstract class Item {
        public static int space = 5;
        public Texture2D texture;
        public RectangleF bounds;
        public readonly TextureInfo info;

        public abstract void Update(List<GameObject> objects);

        public void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle) bounds, info.src, Color.White * .75f);
        }

        public abstract void AddedToPlayer(Player p, ref float loc);

        public static List<IItemEntity> items;

        static Item() {
            items = new List<IItemEntity> {
                new ItemEntity<DamageUpgrade>(0, 0, DamageUpgrade.type),
                new ItemEntity<HealingUpgrade>(0, 0, HealingUpgrade.type),
                new ItemEntity<MoneyUpgrade>(0, 0, MoneyUpgrade.type)
            };
        }

        protected Item(string type) {
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(0, 720 - 100, info.src.Width * info.scale, info.src.Height * info.scale);
        }
    }

    public interface IItemEntity {
        GameObject Clone();
        void SetPos(float x, float y);
    }

    public class ItemEntity<T> : GameObject, ICollider, IItemEntity where T : Item, new() {
        private string type;
        private RectangleF bounds;
        private readonly Texture2D texture;
        private readonly TextureInfo info;
        private float origX;
        private float origY;

        private readonly Random rand;

        private Vector2 velocity;

        public ItemEntity(float x, float y, string type) {
            this.type = type;
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
            origX = x;
            origY = y;
            rand = new Random();
            velocity = new Vector2();
        }

        public override void Update(EventList<GameObject> objects) {
            velocity.X += ((float) rand.NextDouble() - .5f) * .01f;
            velocity.Y += ((float) rand.NextDouble() - .5f) * .01f;
            velocity.X = MathHelper.Clamp(velocity.X, -.5f, .5f);
            velocity.Y = MathHelper.Clamp(velocity.Y, -.5f, .5f);
            bounds.x += velocity.X;
            bounds.y += velocity.Y;
            var xd = origX - bounds.x;
            if (xd < -4) bounds.x = origX + 4;
            if (xd > 4) bounds.x = origX - 4;
            var yd = origY - bounds.y;
            if (yd < -4) bounds.y = origY + 4;
            if (yd > 4) bounds.y = origY - 4;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(bounds)) {
                Destroy();
                ((Player) c).AddItem(new T());
            }
        }

        public GameObject Clone() {
            return (ItemEntity<T>) MemberwiseClone();
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public void SetPos(float x, float y) {
            bounds.x = origX = x;
            bounds.y = origY = y;
        }
    }

    class DamageUpgrade : Item {
        public const string type = "RedCanister";

        public DamageUpgrade() : base(type) {}

        public override void Update(List<GameObject> objects) {}

        public override void AddedToPlayer(Player p, ref float loc) {
            bounds.x = loc;
            loc += bounds.width + space;
            Ability[] abilities = {p.abilityOne};
            foreach (var a in abilities) {
                if (a is BurstFireAbility) ((BurstFireAbility) a).damage += 10;
            }
        }
    }

    class HealingUpgrade : Item {
        public const string type = "GreenCanister";

        public HealingUpgrade() : base(type) {}

        public override void Update(List<GameObject> objects) {}

        public override void AddedToPlayer(Player p, ref float loc) {
            bounds.x = loc;
            loc += bounds.width + space;
            p.passiveHealingAmount += 5;
        }
    }

    class MoneyUpgrade : Item {
        public const string type = "YellowCanister";

        public MoneyUpgrade() : base(type) {}

        public override void Update(List<GameObject> objects) {}

        public override void AddedToPlayer(Player p, ref float loc) {
            bounds.x = loc;
            loc += bounds.width + space;
            Coin.Value += 5;
        }
    }
}