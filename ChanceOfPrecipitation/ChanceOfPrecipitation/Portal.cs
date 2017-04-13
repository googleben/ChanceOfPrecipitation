using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    public class Portal : GameObject, ICollider {
        private TextureInfo info;
        private Texture2D texture;
        private RectangleF bounds;
        private bool? activated;
        private bool playerHover;

        private Text text;
        private Keys key;

        public Portal(float x, float y) {
            info = TextureManager.blocks["portal"];
            texture = TextureManager.textures[info.texName];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);

            activated = null;

            key = Keys.E;
            text = new Text("press " + key + " to activate", Vector2.Zero);
            text.SetPos(bounds.Center.X - text.width / 2, bounds.y - bounds.height);
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)(bounds + Playing.Instance.offset), info.src, Color.White);
            text.Draw(sb);
        }

        public override void Update(List<GameObject> objects) {
            text.IsVisible = playerHover && (activated == null || activated == true);
        }

        public void Collide(ICollidable c) {
            if (c is Player) {
                if (c.Bounds().Intersects(bounds)) {
                    playerHover = true;

                    if (Playing.Instance.state.IsKeyDown(key) && !Playing.Instance.lastState.IsKeyDown(key)) {
                        if (activated.HasValue) {
                            if (!activated.Value) activated = true;
                            else Pressed();
                        }
                        else {
                            activated = false;
                        }
                    }
                } else {
                    playerHover = false;
                }
            }
        }

        public void Pressed() {
            // TODO: do watevr her
        }
    }
}
