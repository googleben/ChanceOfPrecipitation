﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class FloatingIndicator : GameObject
    {
        private Vector2 position;
        private readonly float scale;
        private readonly float upSpeed;
        private int life;
        private readonly int origLife;
        private readonly string Character;
        private bool isStatic;
        private int spacing;

        private readonly Color color;

        private List<Character> Characters;

        public FloatingIndicator(FloatingIndicatorBuilder builder, int number, Vector2 position)
            : this(builder, number.ToString(), position) { }

        public FloatingIndicator(FloatingIndicatorBuilder builder, string text, Vector2 position)
        {
            this.position = new Vector2(position.X, position.Y);
            scale = builder.Scale / 2.5f;
            upSpeed = builder.UpSpeed;
            origLife = builder.Life;
            life = origLife;
            color = builder.Color;
            Character = text;
            isStatic = builder.IsStatic;
            spacing = builder.Spacing;

            if (isStatic) upSpeed = 0;

            Characters = new List<Character>();
            var nums = Character.ToCharArray();
            foreach (var c in nums) {
                Characters.Add(new Character(c.ToString(), scale, color, isStatic));
            }
            var x = position.X;

            foreach (var n in Characters) {
                n.SetPos(x, position.Y);
                x += n.bounds.width + spacing;
            }
        }

        public override void Update(EventList<GameObject> objects)
        {
            position.Y -= upSpeed;
            var x = position.X;

            life--;
            if (life <= 0) Destroy();

            foreach (var n in Characters) {
                n.SetPos(x, position.Y);
                x += n.bounds.width + spacing;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            //var nums = number.ToString().ToCharArray();


            foreach (var n in Characters) n.Draw(sb, (float)life / origLife);
        }
    }

    public class FloatingIndicatorBuilder
    {
        public float Scale { get; set; } = 2f;
        public float UpSpeed { get; set; } = 0.3f;
        public int Life { get; set; } = 120;
        public Color Color { get; set; } = Color.White;
        public bool IsStatic { get; set; } = false;
        public int Spacing { get; set; } = 0;

        public FloatingIndicator Build(int number, Vector2 position)
        {
            return new FloatingIndicator(this, number, position);
        }

        public FloatingIndicator Build(string text, Vector2 position)
        {
            return new FloatingIndicator(this, text, position);
        }
    }
}