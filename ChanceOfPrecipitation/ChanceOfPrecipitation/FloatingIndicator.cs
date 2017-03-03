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
        private readonly Point proportions = new Point(5, 7);

        private Vector2 position;
        private float scale;
        private readonly float upSpeed;
        private readonly bool oscillates;
        private float oscillationDist;
        private readonly Timer oscillationPeriod;
        private readonly Timer life;
        private readonly int number;
        private int direction = 1;

        private readonly Color color;

        private Func<int, Rectangle> Bounds;

        public FloatingIndicator(FloatingIndicatorBuilder builder, int number, Vector2 position)
        {
            this.position = new Vector2(position.X - builder.OscillationDist, position.Y);
            scale = builder.Scale;
            upSpeed = builder.UpSpeed;
            oscillates = builder.Oscillates;
            oscillationDist = builder.OscillationDist;
            oscillationPeriod = new Timer(builder.OscillationPeriod);
            life = new Timer(builder.Life);
            color = builder.Color;
            this.number = number;

            oscillationPeriod.Start();
            life.Start();

            oscillationPeriod.Elapsed += ChangeDirection;
            life.Elapsed += Delete;
        }

        public override void Update(List<GameObject> objects)
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
                //scale -= shrink;
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
                    var rectangle = Bounds(i + 1);
                    if (nums[i] != '1')
                        sb.Draw(TextureManager.Textures["Numbers"], rectangle, TextureManager.Sources[nums[i].ToString()], color);
                    else
                    {
                        rectangle.Width = (int)(proportions.X * scale * 3 / 4);
                        sb.Draw(TextureManager.Textures["Numbers"], new Rectangle(rectangle.X, rectangle.Y, rectangle.Width / 2, rectangle.Height), TextureManager.Sources[nums[i].ToString()], color);
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    sb.Draw(TextureManager.Textures["Numbers"], Bounds(i + 1), TextureManager.Sources["0"], color);
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