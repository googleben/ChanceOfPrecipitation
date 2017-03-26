﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Texture2D texture;
        private BlockInfo info;
        private float origX;
        private float origY;

        private Random rand;

        public ItemEntity(float x, float y, string type) {
            this.type = type;
            this.info = TextureManager.blocks[type];
            this.texture = TextureManager.textures[info.texName];
            this.bounds = new RectangleF(x, y, info.src.Width*info.scale, info.src.Height*info.scale);
            this.origX = x;
            this.origY = y;
            rand = new Random();
        }

        public override void Update(List<GameObject> objects) {
            bounds.x -= ((float) rand.NextDouble() - .5f) * 2;
            bounds.y -= ((float)rand.NextDouble() - .5f) * 2;
            float xd = origX - bounds.x;
            if (xd < -4) bounds.x = origX - 4;
            if (xd > 4) bounds.x = origX + 4;
            float yd = origY - bounds.y;
            if (yd < -4) bounds.y = origY - 4;
            if (yd > 4) bounds.y = origY + 4;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, info.src, Color.White);
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(bounds)) {
                this.Destroy();
                ((Player) c).AddItem(new T());
            }
        }

    }

    class DamageUpgrade : Item {
        private const string type = "DamageUpgrade";

        private Texture2D texture;
        private RectangleF bounds;
        private BlockInfo info;

        public DamageUpgrade() {
            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(0, 1280-100, info.src.Width*info.scale, info.src.Height*info.scale);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public override void Draw(SpriteBatch sb) {
            //TODO: Draw item
        }

        public override void AddedToPlayer(Player p, ref float loc) {
            bounds.x = loc;
            loc += space;
            Ability[] abilities = {p.abilityOne};
            foreach (Ability a in abilities) {
                if (a is BurstFireAbility) ((BurstFireAbility)a).damage += 10;
            }
        }
    }

}
