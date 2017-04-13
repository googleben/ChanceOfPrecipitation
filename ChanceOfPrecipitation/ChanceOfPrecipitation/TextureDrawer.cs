using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public class TextureDrawer {
        private readonly Texture2D texture;
        private Rectangle srcBounds;
        private readonly int frames;
        private int currentFrame;

        public TextureDrawer(string name) {
            var info = TextureManager.blocks[name];

            texture = TextureManager.textures[info.texName];
            srcBounds = info.src;
            frames = info.frames;
        }

        public void Draw(SpriteBatch sb, Rectangle bounds) {
            if (currentFrame == frames) {
                currentFrame = 1;
                srcBounds.X = 0;
            } else {
                currentFrame++;
                srcBounds.X += srcBounds.Width;
            }

            sb.Draw(texture, bounds, srcBounds, Color.White);
        }

        public void Reset() {
            currentFrame = 1;
            srcBounds.X = 0;
        }
    }
}
