using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    class MoneyDisplay
    {
        private Vector2 position;
        private List<Character> numbers;
        private float scale = 3;

        public MoneyDisplay(Vector2 position)
        {
            this.position = position;
            this.numbers = new List<Character>();
            SetMoney(0);
        }

        public void SetMoney(int money)
        {
            numbers.Clear();
            Character credits = new Character("$", scale, Color.Gold, true);
            numbers.Add(credits);
            foreach (char c in (money + "")) {
                numbers.Add(new Character(c.ToString(), scale, Color.Gold, true));
            }
            float x = position.X;
            foreach (Character c in numbers) {
                c.SetPos(x, position.Y);
                x += c.bounds.width;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Character c in numbers) c.Draw(sb);
        }
    }
}