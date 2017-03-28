using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{

    public abstract class Item {
        public static int space = 5;

        public abstract void Update(List<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
        public abstract void AddedToPlayer(Player p, ref float loc);

    }

    class ItemEntity<T> : GameObject, ICollider where T : Item, new() {
        private string type;
        private RectangleF bounds;
        private readonly Texture2D texture;
        private readonly BlockInfo info;
        private readonly float origX;
        private readonly float origY;

        private readonly Random rand;

        private Vector2 velocity;

        public ItemEntity(float x, float y, string type) {
            this.type = type;
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width*info.scale, info.src.Height*info.scale);
            origX = x;
            origY = y;
            rand = new Random();
            velocity = new Vector2();
        }

        public override void Update(List<GameObject> objects) {
            velocity.X += ((float) rand.NextDouble() - .5f) * .01f;
            velocity.Y += ((float)rand.NextDouble() - .5f) * .01f;
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
            sb.Draw(texture, (Rectangle)bounds, info.src, Color.White);
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(bounds)) {
                Destroy();
                ((Player) c).AddItem(new T());
            }
        }

    }

    class DamageUpgrade : Item {
        private const string type = "Canister";

        private Texture2D texture;
        private RectangleF bounds;
        private readonly BlockInfo info;

        public DamageUpgrade() {
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(0, 720-100, info.src.Width*info.scale, info.src.Height*info.scale);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, info.src, Color.White);
        }

        public override void AddedToPlayer(Player p, ref float loc) {
            bounds.x = loc;
            loc += space;
            Ability[] abilities = {p.abilityOne};
            foreach (var a in abilities) {
                if (a is BurstFireAbility) ((BurstFireAbility)a).damage += 10;
            }
        }
    }

}
