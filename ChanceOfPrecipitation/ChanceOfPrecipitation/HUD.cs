using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class HUD : GameObject
    {
        Rectangle bounds;
        Texture2D tex;
        TextureInfo info;

        Player player;

        Ability one;
        Ability two;
        Ability three;
        Ability four;

        public HUD(Player player)
        {
            this.player = player;
            one = player.abilityOne;
            two = player.abilityTwo;
            three = player.abilityThree;
            four = player.abilityFour;

            info = TextureManager.blocks["HUD"];
            tex = TextureManager.textures[info.texName];

            bounds = new Rectangle(1280 / 2, 720 - 150, info.src.Width, info.src.Height);
            bounds.X -= bounds.Width / 2;
        }

        public override void Update(EventList<GameObject> objects)
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(tex, bounds, Color.White);
        }
    }
}
