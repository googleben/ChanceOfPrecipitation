using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public class TextureDrawer {
        public readonly string name;

        private readonly Texture2D texture;
        private Rectangle srcBounds;
        private readonly int frames;
        private int currentFrame;
        private readonly int delay;
        private int currentDelay;
        private SpriteEffects effects;

        public TextureDrawer(string key) : this(key, 5) { }

        /// <summary>
        /// Initializes a new <see cref="TextureDrawer"./>
        /// </summary>
        /// <param name="key">The key for the <see cref="TextureInfo"/> for the animation or texture.</param>
        /// <param name="delay">The amount of game ticks that will pass between each frame change.</param>
        public TextureDrawer(string key, int delay) {
            name = key;

            this.delay = delay;
            currentDelay = delay;

            var info = TextureManager.blocks[key];

            texture = TextureManager.textures[info.texName];
            srcBounds = info.src;
            frames = info.frames;
        }

        public void Draw(SpriteBatch sb, Rectangle bounds, Direction facing = Direction.Right) {
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

            if (facing == Direction.Right)
                effects = SpriteEffects.None;
            else
                effects = SpriteEffects.FlipHorizontally;

            sb.Draw(texture, bounds, srcBounds, Color.White, 0f, Vector2.Zero, effects, 1f);
        }

        public void Reset() {
            currentFrame = 1;
            srcBounds.X = 0;
        }
    }
}
