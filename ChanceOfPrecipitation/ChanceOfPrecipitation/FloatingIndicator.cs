using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicator : GameObject
    {
        private Vector2 position;
        private readonly float scale;
        private readonly float upSpeed;
        private readonly bool oscillates;
        private float oscillationDist;
        private readonly Timer oscillationPeriod;
        private readonly Timer life;
        private readonly int number;
        private int direction = 1;
        private bool isStatic;
        private int spacing;

        private readonly Color color;

        private List<Number> numbers;

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
            isStatic = builder.IsStatic;
            spacing = builder.Spacing;

            if (isStatic) upSpeed = 0;

            oscillationPeriod.Start();
            life.Start();

            oscillationPeriod.Elapsed += ChangeDirection;
            life.Elapsed += Delete;

            numbers = new List<Number>();
            var nums = number.ToString().ToCharArray();
            foreach (var c in nums) {
                numbers.Add(new Number(c+"", scale, color, isStatic));
            }

        }

        public override void Update(List<GameObject> objects)
        {
            position.Y -= upSpeed;
            var x = position.X;

            foreach (var n in numbers) {
                n.SetPos(x, position.Y);
                x += n.bounds.width + spacing;
            }

            if (oscillates)
            {
                position.X += direction > 0
                    ? (float) (oscillationDist / oscillationPeriod.Interval * 100)
                    : (float) (-oscillationDist / oscillationPeriod.Interval * 100);

                oscillationDist /= 1.01f;
            }
            else
            {
                //var shrink = (float)(upSpeed / life.Interval * 100);
                //scale -= shrink;
                //position.X += shrink * 10;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            //var nums = number.ToString().ToCharArray();

            
            foreach (var n in numbers) n.Draw(sb);
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

    class Number {
        public float scale;
        public RectangleF bounds;
        public Texture2D texture;
        public BlockInfo info;
        public string type;
        public Color color;
        public bool isStatic;

        public Number(string type, float scale, bool isStatic = false) : this(type, scale, Color.White, isStatic) { }

        public Number(string type, float scale, Color color, bool isStatic = false) {
            this.scale = scale;
            this.type = type;
            this.color = color;
            this.isStatic = isStatic;
            this.info = TextureManager.blocks[type];
            this.texture = TextureManager.textures[info.texName];
            this.bounds = new RectangleF(0, 0, info.src.Width*scale, info.src.Height*scale);
        }

        public void SetPos(float x, float y) {
            bounds.x = x;
            bounds.y = y;
        }

        public void Draw(SpriteBatch sb) {
            if (isStatic)
                sb.Draw(texture, (Rectangle) bounds, info.src, color);
            else
                sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, color);
        }

    }
}