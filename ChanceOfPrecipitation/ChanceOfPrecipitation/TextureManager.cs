using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public static class TextureManager
    {
        public static Dictionary<string, Texture2D> Textures;

        static TextureManager()
        {
            Textures = new Dictionary<string, Texture2D>();
        }
    }
}
