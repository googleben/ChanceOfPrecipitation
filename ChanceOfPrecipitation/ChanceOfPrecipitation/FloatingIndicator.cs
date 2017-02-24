using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    class FloatingIndicator : GameObject
    {
        private const int spacing = 1;
        private readonly Point proportions = new Point(3, 5);

        private Vector2 position;
        private readonly float scale;
        private float center;
        private readonly float upSpeed;
        private readonly float oscillationDist;
        private readonly Timer oscillationPeriod;
        private readonly Timer life;
        private readonly int number;
        private int direction = 1;

        private readonly Color color;

        private Func<int, Rectangle> Bounds;

        /// <summary>
        /// Initialize a new instance of <see cref="FloatingIndicator"/>
        /// </summary>
        /// <param name="position">Initial position of the <see cref="FloatingIndicator"/>.</param>
        /// <param name="scale">Amount the original proportions of the <see cref="FloatingIndicator"/> are scaled up by.</param>
        /// <param name="upSpeed">The speed the <see cref="FloatingIndicator"/> moves up.</param>
        /// <param name="oscillationDist">The distance the <see cref="FloatingIndicator"/> is displaced from the center during oscillation.</param>
        /// <param name="oscillationPeriod">The time it takes for the <see cref="FloatingIndicator"/> to move from the far right to the far left of its oscillation, in milliseconds.</param>
        /// <param name="life">The time the <see cref="FloatingIndicator"/> lasts, in milliseconds.</param>
        /// <param name="color">The color of the digits of the <see cref="FloatingIndicator"/>.</param>
        /// <param name="number">The number the <see cref="FloatingIndicator"/> displays.</param>
        public FloatingIndicator(Vector2 position, float scale, float upSpeed, float oscillationDist, float oscillationPeriod, float life, Color color, int number)
        {
            this.position = new Vector2(position.X - oscillationDist, position.Y);
            this.scale = scale;
            this.upSpeed = upSpeed;
            this.oscillationDist = oscillationDist;
            this.oscillationPeriod = new Timer(oscillationPeriod);
            this.life = new Timer(life);
            this.color = color;
            this.number = number;

            center = position.X;

            this.oscillationPeriod.Start();
            this.life.Start();

            this.oscillationPeriod.Elapsed += new ElapsedEventHandler(ChangeDirection);
            this.life.Elapsed += new ElapsedEventHandler(Delete);
        }

        public override void Update(IEnumerable<GameObject> objects)
        {
            Bounds = digit => new Rectangle((int)(position.X + digit * (scale * proportions.X) + (digit - 1) * spacing), (int)position.Y, (int)scale * proportions.X, (int)scale * proportions.Y);

            position.Y -= upSpeed;
            position.X += direction > 0
                ? (float)(oscillationDist / oscillationPeriod.Interval * 100)
                : (float)(-oscillationDist / oscillationPeriod.Interval * 100);
        }

        public override void Draw(SpriteBatch sb)
        {
            var nums = number.ToString().ToCharArray();

            for (var i = 0; i < nums.Length; i++)
            {
                try
                {
                    sb.Draw(TextureManager.Textures["Numbers"], Bounds(i + 1),TextureManager.Sources[nums[i].ToString()], color);
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
