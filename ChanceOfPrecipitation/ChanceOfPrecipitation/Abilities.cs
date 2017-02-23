﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChanceOfPrecipitation {
    public abstract class Ability {

        public abstract void fire(Entity origin);

    }

    public class Bullet : GameObject, Collidable {

        RectangleF bounds;
        float damage;
        float speed;

        Texture2D texture;

        public Bullet(float x, float y, float speed, float damage) {
            texture = TextureManager.Textures["Square"];
            bounds = new RectangleF(x, y, 3, 1);
            this.damage = damage;
            this.speed = speed;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
        }

        public override void Update(IEnumerable<GameObject> objects) {
            this.bounds.X += speed;
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public void Collide(Collision side, float amount, StaticObject origin) {
            if (origin is Entity) (origin as Entity).Damage(damage);
            Destroy();
        }
    }


}