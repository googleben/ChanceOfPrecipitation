using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class Text
    {
        public bool IsVisible { get; set; }
        public int width;

        private int spacing;
        private Vector2 position;
        private float scale;
        private Color color;
        private bool isStatic;

        private readonly List<Character> characters;

        public Text(string text, Vector2 position) : this(text, position, 0.5f, 1, Color.White, false) { }
        public Text(string text, Vector2 position, float scale, Color color, bool isStatic = false) : this(text, position, scale, 1, color, isStatic) { }
        public Text(string text, Vector2 position, Color color) : this(text, position, 0.5f, 1, color, false) { }

        public Text(string text, Vector2 position, float scale, int spacing, Color color, bool isStatic)
        {
            this.position = position;
            this.scale = scale;
            this.spacing = spacing;
            this.color = color;
            this.isStatic = isStatic;

            characters = new List<Character>();
            SetText(text);
        }

        public void SetPos(float x, float y)
        {
            position.X = x;
            position.Y = y;

            foreach (var c in characters) {
                c.SetPos(x, y);
                var amount = c.bounds.width + spacing;
                x += amount;
            }
        }

        public void SetText(string text)
        {
            text = text.ToLower();

            characters.Clear();
            width = 0;

            foreach (var c in text)
                characters.Add(new Character(c.ToString(), scale, color, isStatic));

            var x = position.X;

            foreach (var c in characters) {
                c.SetPos(x, position.Y);
                var amount = c.bounds.width + spacing;
                x += amount;
                width += (int)amount;
            }
        }

        public void AlignHorizontally(Rectangle value)
        {
            var offset = position.X + width / 2 - value.Center.X;
            SetPos(position.X - offset, position.Y);
        }

        public void AlignVertically(Rectangle value)
        {
            var offset = position.Y + 17 / 2 - value.Center.Y;
            SetPos(position.X, position.Y - offset);
        }

        public void AlignToCenter(Rectangle value)
        {
            AlignHorizontally(value);
            AlignVertically(value);
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsVisible)
                foreach (var c in characters) c.Draw(sb);
        }
    }
}