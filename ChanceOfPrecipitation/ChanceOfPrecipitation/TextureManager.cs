using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public static class TextureManager
    {
        public static Dictionary<string, Texture2D> textures;
        public static Dictionary<string, TextureInfo> blocks;

        static TextureManager()
        {
            textures = new Dictionary<string, Texture2D>();
            blocks = new Dictionary<string, TextureInfo>();
        }
    }
}
