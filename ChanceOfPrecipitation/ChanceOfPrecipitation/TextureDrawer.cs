﻿using System;
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
        private readonly int delay;
        private int currentDelay;

        public TextureDrawer(TextureInfo info) : this(info, 5) { }

        /// <summary>
        /// Initializes a new <see cref="TextureDrawer"./>
        /// </summary>
        /// <param name="info">The <see cref="TextureInfo"/> for the animation or texture.</param>
        /// <param name="delay">The amount of game ticks that will pass between each frame change.</param>
        public TextureDrawer(TextureInfo info, int delay) {
            this.delay = delay;
            currentDelay = delay;

            texture = TextureManager.textures[info.texName];
            srcBounds = info.src;
            frames = info.frames;
        }

        public void Draw(SpriteBatch sb, Rectangle bounds) {
            if (currentDelay <= 0) {
                if (currentFrame == frames) {
                    currentFrame = 1;
                    srcBounds.X = 0;
                }
                else {
                    currentFrame++;
                    srcBounds.X += srcBounds.Width;
                }

                currentDelay = delay;
            } else {
                currentDelay--;
            }

            sb.Draw(texture, bounds, srcBounds, Color.White);
        }

        public void Reset() {
            currentFrame = 1;
            srcBounds.X = 0;
        }
    }
}
