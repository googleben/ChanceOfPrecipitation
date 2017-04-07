using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Text {
        private readonly List<Character> characters;

        public Text(string text, Vector2 position, float scale = 1) {
            characters = new List<Character>();

            foreach (var c in text)
                characters.Add(new Character(c.ToString(), scale, Color.White, true));

            var x = position.X;

            foreach (var c in characters) {
                c.SetPos(x, position.Y);
                x += c.bounds.width;
            }
        }

        public void Draw(SpriteBatch sb) {
            foreach (var c in characters) c.Draw(sb);
        }
    }
}
