using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChanceOfPrecipitation
{
    class Parallax
    {

        Texture2D texture;
        float speed; //relative to player - 1 == 100% of player's movement

        public Parallax(Texture2D texture, float speed)
        {
            this.texture = texture;
            this.speed = speed;
        }

        public void Draw(SpriteBatch sb, RectangleF pb)
        {
            var offset = new Point((int)Math.Abs((pb.x * speed)), (int)Math.Abs((pb.y * speed)));
            offset.X %= texture.Width;
            offset.Y %= texture.Height;
            if (offset.X + 1280 < texture.Width && offset.Y + 720 < texture.Height) {
                sb.Draw(texture, new Rectangle(0, 0, 1280, 720), new Rectangle(offset.X, offset.Y, 1280, 720), Color.White);
            } else {
                int width = Math.Min(1280, texture.Width - offset.X);
                int height = Math.Min(720, texture.Height - offset.Y);
                sb.Draw(texture, new Rectangle(0, 0, width, height), new Rectangle(offset.X, offset.Y, width, height), Color.White);
                if (width < 1280) {
                    sb.Draw(texture, new Rectangle(width, 0, 1280 - width, height), new Rectangle(0, offset.Y, 1280 - width, height), Color.White);
                }
                if (height < 720) {
                    sb.Draw(texture, new Rectangle(height, 0, width, 720 - height), new Rectangle(offset.X, 0, width, 720 - height), Color.White);
                }
            }
        }

    }
}
