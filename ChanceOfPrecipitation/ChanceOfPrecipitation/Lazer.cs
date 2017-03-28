﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Lazer : GameObject, ICollider {

        private const float Damage = 10f;

        private readonly RectangleF bounds;
        private readonly Direction facing;

        private bool collided;
        private int life;

        private readonly Rectangle lazerSegmentSource = new Rectangle(0, 0, 1, 5);
        private readonly Rectangle lazerEndSource = new Rectangle(1, 0, 1, 5);

        public Lazer(ICollidable origin, int range, int life) {
            this.life = life;

            facing = origin.Facing();

            var blocks = Playing.Instance.objects.OfType<Block>().ToList();

            var left = facing == Direction.Left;

            Console.WriteLine(origin.Bounds().Center.Y);

            bounds = new RectangleF(origin.Bounds().Center.X + (origin.Bounds().width / (left ? -2 : 2)), origin.Bounds().Center.Y, 1, 5);

            if (left) bounds.x--;

            while (!collided && bounds.width < range) {
                bounds.width++;
                if (left) bounds.x--;

                blocks.ForEach(b => {
                    if (b.bounds.Intersects(Bounds()))
                        collided = true;
                });
            }

        }

        public override void Update(List<GameObject> objects) {
            life--;

            if (life <= 0)
                Destroy();
        }

        public override void Draw(SpriteBatch sb) {
            if (Facing() == Direction.Right)
                for (var i = 0; i < bounds.width; i++)
                    sb.Draw(TextureManager.textures["Lazer"], new Rectangle((int)Bounds().x + i, (int)Bounds().y, 1, 5), i == (int)bounds.width - 1 ? lazerEndSource : lazerSegmentSource, Color.White);
            else
                for (var i = 0; i > bounds.width * -1; i--)
                    sb.Draw(TextureManager.textures["Lazer"], new Rectangle((int)Bounds().x - i, (int)Bounds().y, 1, 5), i == (int)bounds.width + 1 ? lazerEndSource : lazerSegmentSource, Color.White);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public Direction Facing() {
            return facing;
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(Bounds()))
                ((Player) c).Damage(Damage);
        }
    }
}
