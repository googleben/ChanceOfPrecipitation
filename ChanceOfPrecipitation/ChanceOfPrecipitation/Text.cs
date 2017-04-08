using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Text {
        public bool IsVisible { get; set; } = false;
        public readonly int width;

        private int spacing;

        private readonly List<Character> characters;

        public Text(string text, Vector2 position, float scale = 0.5f, int spacing = 1) {
            this.spacing = spacing;

            text = text.ToLower();

            characters = new List<Character>();

            foreach (var c in text)
                characters.Add(new Character(c.ToString(), scale, Color.White));

            var x = position.X;

            foreach (var c in characters) {
                c.SetPos(x, position.Y);
                var amount = c.bounds.width + spacing;
                x += amount;
                width += (int)amount;
            }
        }

        public void SetPos(float x, float y) {
            foreach (var c in characters)
            {
                c.SetPos(x, y);
                var amount = c.bounds.width + spacing;
                x += amount;
            }
        }

        public void Draw(SpriteBatch sb) {
            if (IsVisible)
                foreach (var c in characters) c.Draw(sb);
        }
    }
}
