using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public static class TextureManager
    {
        public static Dictionary<string, Texture2D> Textures;
        public static Dictionary<string, BlockInfo> Blocks;

        static TextureManager()
        {
            Textures = new Dictionary<string, Texture2D>();
            Blocks = new Dictionary<string, BlockInfo>();
        }
    }
}
