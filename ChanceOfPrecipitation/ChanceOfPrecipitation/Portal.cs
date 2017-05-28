using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    public class Portal : GameObject, ICollider {
        private TextureInfo info;
        private TextureDrawer texture;
        private RectangleF bounds;
        private bool playerHover;
        private BossEnemy boss;

        // null: has not been activated yet
        // false: has been activated and the boss is alive
        // true: the boss is dead and can proceed to the next stage
        private bool? activated;

        private Text text;
        private Keys key;

        public Portal(float x, float y) {
            info = TextureManager.blocks["portal1"];
            texture = new TextureDrawer("portal1");
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);

            activated = null;

            key = Keys.E;
            text = new Text("press " + key + " to activate", Vector2.Zero);
            text.SetPos(bounds.Center.X - text.width / 2, bounds.y - bounds.height);
        }

        public override void Draw(SpriteBatch sb) {
            texture.Draw(sb, (Rectangle) (bounds + Playing.Instance.offset));
            text.Draw(sb);
        }

        public override void Update(EventList<GameObject> objects) {
            text.IsVisible = playerHover && (activated == null || activated == true);

            if (activated.HasValue)
                if (!activated.Value && boss.ToDestroy) {
                    activated = true;
                    texture = new TextureDrawer("portal3");
                }
        }

        public void Collide(ICollidable c) {
            if (c is Player) {
                if (c.Bounds().Intersects(bounds)) {
                    playerHover = true;

                    if (Playing.Instance.state.IsKeyDown(key) && !Playing.Instance.lastState.IsKeyDown(key)) {
                        if (activated.HasValue) {
                            if (activated.Value) {
                                Pressed();
                                texture = new TextureDrawer("portal4");
                            }
                        }
                        else {
                            texture = new TextureDrawer("portal2");
                            activated = false;
                            boss = new TestBoss(bounds.x - 32, bounds.y - 128);
                            Playing.Instance.objects.Add(boss);
                        }
                    }
                }
                else {
                    playerHover = false;
                }
            }
        }

        public void Pressed() {
            Playing.Instance.NextStage();
        }

        public RectangleF Bounds() {
            return bounds;
        }
    }
}