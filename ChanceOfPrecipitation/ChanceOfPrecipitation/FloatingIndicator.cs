using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicator : GameObject
    {
        private const int Spacing = 1;
        private readonly Point proportions = new Point(3, 5);

        private Vector2 position;
        private float scale;
        private float center;
        private readonly float upSpeed;
        private readonly bool oscillates;
        private float oscillationDist;
        private readonly Timer oscillationPeriod;
        private readonly Timer life;
        private readonly int number;
        private int direction = 1;

        private readonly Color color;

        private Func<int, Rectangle> Bounds;

        public FloatingIndicator(FloatingIndicatorBuilder builder)
        {
            position = new Vector2(builder.Position.X - builder.OscillationDist, builder.Position.Y);
            scale = builder.Scale;
            upSpeed = builder.UpSpeed;
            oscillates = builder.Oscillates;
            oscillationDist = builder.OscillationDist;
            oscillationPeriod = new Timer(builder.OscillationPeriod);
            life = new Timer(builder.Life);
            color = builder.Color;
            number = builder.Number;

            center = position.X;

            oscillationPeriod.Start();
            life.Start();

            oscillationPeriod.Elapsed += ChangeDirection;
            life.Elapsed += Delete;
        }

        public override void Update(IEnumerable<GameObject> objects)
        {
            Bounds = digit => new Rectangle((int)(position.X + digit * (scale * proportions.X) + (digit - 1) * Spacing), (int)position.Y, (int)(scale * proportions.X), (int)(scale * proportions.Y));

            position.Y -= upSpeed;

            if (oscillates)
            {
                position.X += direction > 0
                    ? (float) (oscillationDist / oscillationPeriod.Interval * 100)
                    : (float) (-oscillationDist / oscillationPeriod.Interval * 100);

                oscillationDist /= 1.01f;
            }
            else
            {
                var shrink = (float)(upSpeed / life.Interval * 100);
                scale -= shrink;
                position.X += shrink * 10;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            var nums = number.ToString().ToCharArray();

            for (var i = 0; i < nums.Length; i++)
            {
                try
                {
                    sb.Draw(TextureManager.Textures["Numbers"], Bounds(i + 1), TextureManager.Sources[nums[i].ToString()], color);
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Delete(object sender, ElapsedEventArgs e)
        {
            Destroy();
            oscillationPeriod.Stop();
            life.Stop();
        }

        private void ChangeDirection(object sender, ElapsedEventArgs e)
        {
            direction *= -1;
        }
    }
}