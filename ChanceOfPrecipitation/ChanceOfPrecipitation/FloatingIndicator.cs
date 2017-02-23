using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    class FloatingIndicator : GameObject
    {
        private const int spacing = 1;

        private Vector2 position;
        private int scale;
        private float center;
        private float upSpeed;
        private float oscillationDist;
        private Timer oscillationPeriod;
        private Timer life;
        private int number;

        private Color color;

        private Func<int, Rectangle> Bounds;

        public FloatingIndicator(Vector2 position, int scale, float upSpeed, float oscillationDist, float oscillationPeriod, float life, Color color, int number)
        {
            this.position = position;
            this.scale = scale;
            this.upSpeed = upSpeed;
            this.oscillationDist = oscillationDist;
            this.oscillationPeriod = new Timer(oscillationPeriod);
            this.life = new Timer(life);
            this.color = color;
            this.number = number;

            center = position.X;

            Bounds = digit => new Rectangle((int)(position.X) + digit * (scale * 2), (int)position.Y, scale * 2, scale * 5);
        }

        public override void Update(IEnumerable<GameObject> objects)
        {
            oscillationPeriod.Update();
            life.Update();

            position.Y -= upSpeed;
        }

        public override void Draw(SpriteBatch sb)
        {
            var nums = number.ToString().ToCharArray();

            for (int i = 0; i < nums.Length; i++)
            {
                sb.Draw(TextureManager.Textures["Numbers"], Bounds(i), TextureManager.Sources[nums[i].ToString()], color);
                Console.WriteLine(Bounds(i).X);
            }
        }
    }
}
