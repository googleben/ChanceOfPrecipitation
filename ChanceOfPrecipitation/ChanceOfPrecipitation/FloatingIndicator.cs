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
        private readonly string Character;
        private int direction = 1;
        private bool isStatic;
        private int spacing;

        private readonly Color color;

        private List<Character> Characters;

        public FloatingIndicator(FloatingIndicatorBuilder builder, int Character, Vector2 position) : this(builder, Character.ToString(), position) { }

        public FloatingIndicator(FloatingIndicatorBuilder builder, string text, Vector2 position) {
            this.position = new Vector2(position.X - builder.OscillationDist, position.Y);
            scale = builder.Scale / 2.5f;
            upSpeed = builder.UpSpeed;
            oscillates = builder.Oscillates;
            oscillationDist = builder.OscillationDist;
            oscillationPeriod = new Timer(builder.OscillationPeriod);
            life = new Timer(builder.Life);
            color = builder.Color;
            Character = text;
            isStatic = builder.IsStatic;
            spacing = builder.Spacing;

            if (isStatic) upSpeed = 0;

            oscillationPeriod.Start();
            life.Start();

            oscillationPeriod.Elapsed += ChangeDirection;
            life.Elapsed += Delete;

            Characters = new List<Character>();
            var nums = Character.ToString().ToCharArray();
            foreach (var c in nums) {
                Characters.Add(new Character(c.ToString(), scale, color, isStatic));
            }
            var x = position.X;

            foreach (var n in Characters)
            {
                n.SetPos(x, position.Y);
                x += n.bounds.width + spacing;
            }
        }

        public override void Update(List<GameObject> objects)
        {
            position.Y -= upSpeed;
            var x = position.X;

            foreach (var n in Characters) {
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
            //var nums = Character.ToString().ToCharArray();

            
            foreach (var n in Characters) n.Draw(sb);
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