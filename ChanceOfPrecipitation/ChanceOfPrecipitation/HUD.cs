using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    public class HUD : GameObject {
        Rectangle bounds;
        Texture2D tex;
        TextureInfo info;

        Player player;

        Ability one;
        Ability two;
        Ability three;
        Ability four;

        Texture2D sheet;

        TextureInfo onet;
        TextureInfo twot;
        TextureInfo threet;
        TextureInfo fourt;

        public HUD(Player player) {
            this.player = player;
            one = player.abilityOne;
            two = player.abilityTwo;
            three = player.abilityThree;
            four = player.abilityFour;

            info = TextureManager.blocks["HUD"];
            tex = TextureManager.textures[info.texName];

            onet = TextureManager.abilities[one.GetType()];
            twot = TextureManager.abilities[two.GetType()];
            threet = TextureManager.abilities[three.GetType()];
            fourt = TextureManager.abilities[four.GetType()];

            sheet = TextureManager.textures["abilities"];

            bounds = new Rectangle(1280 / 2, 720 - 150, info.src.Width, info.src.Height);
            bounds.X -= bounds.Width / 2;

            //Game1.Instance.drawEnd += () => DrawCooldown();
        }

        public override void Update(EventList<GameObject> objects) {}

        public override void Draw(SpriteBatch sb) {
            sb.Draw(tex, bounds, Color.White);
            p = new Polygon();
            DrawAbility(sb, 1, bounds.X + 4, bounds.Y + 5);
            DrawAbility(sb, 2, bounds.X + 4 + 90 + 44, bounds.Y + 5);
            DrawAbility(sb, 3, bounds.X + 4 + 90 + 44 + 90 + 44, bounds.Y + 5);
            DrawAbility(sb, 4, bounds.X + 4 + 90 + 44 + 90 + 44 + 90 + 44, bounds.Y + 5);
        }

        public void DrawAbility(SpriteBatch sb, int a, int x, int y) {
            var info = a == 1 ? onet : a == 2 ? twot : a == 3 ? threet : fourt;
            var ab = a == 1 ? one : a == 2 ? two : a == 3 ? three : four;
            sb.Draw(sheet, new Rectangle(x, y, 90, 90), info.src, Color.White);
            //TODO: Draw cooldown
            AddCooldown(a, x, y);
        }

        Polygon p;

        public void AddCooldown(int a, int x, int y) {
            y = 720 - y;
            var ab = a == 1 ? one : a == 2 ? two : a == 3 ? three : four;
            float percent = (float) ab.cd / ab.Cooldown();
            if (percent > 0) {
                //first traingle
                Vector3 t = percent > .125f
                    ? new Vector3(x + 90, y, 0)
                    : new Vector3(x + 45 + (45 * (percent / .125f)), y, 0);
                p.AddTriangle(new Vector3(x + 45, y - 45, 0), new Vector3(x + 45, y, 0), t);
            }
            if (percent > .125f) {
                //second triangle
                Vector3 t = percent > .375f
                    ? new Vector3(x + 90, y - 90, 0)
                    : new Vector3(x + 90, y - (90 * ((percent - .125f) / .25f)), 0);
                p.AddTriangle(new Vector3(x + 45, y - 45, 0), new Vector3(x + 90, y, 0), t);
            }
            if (percent > .375f) {
                //third triangle
                Vector3 t = percent > .625f
                    ? new Vector3(x, y - 90, 0)
                    : new Vector3(x + 90 - (90 * ((percent - .375f) / .25f)), y - 90, 0);
                p.AddTriangle(new Vector3(x + 45, y - 45, 0), new Vector3(x + 90, y - 90, 0), t);
            }
            if (percent > .625f) {
                //fourth triangle
                Vector3 t = percent > .875f
                    ? new Vector3(x, y, 0)
                    : new Vector3(x, y - (90 - (90 * ((percent - .625f) / .25f))), 0);
                p.AddTriangle(new Vector3(x + 45, y - 45, 0), new Vector3(x, y - 90, 0), t);
            }
            if (percent > .875) {
                //fifth triangle
                Vector3 t = new Vector3(x + (45 * ((percent - .875f) / .125f)), y, 0);
                p.AddTriangle(new Vector3(x + 45, y - 45, 0), new Vector3(x, y, 0), t);
            }
        }

        public void DrawCooldown() {
            if (p.inds.Count == 0) return;
            var device = Game1.Instance.GraphicsDevice;
            BasicEffect basicEffect = new BasicEffect(device);
            basicEffect.Texture = TextureManager.textures["Square"];
            basicEffect.TextureEnabled = true;

            basicEffect.Alpha = .75f;

            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveOffCenter(0, 1280 / 2, 0, 720 / 2, 1, 2);

            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes) {
                effectPass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, p.vpts.ToArray(), 0, p.inds.Count,
                    p.inds.ToArray(), 0, p.inds.Count / 3);
            }
        }
    }

    class Polygon {
        public List<VertexPositionTexture> vpts;
        public List<short> inds;

        public Polygon() {
            vpts = new List<VertexPositionTexture>();
            inds = new List<short>();
        }

        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
            vpts.Add(new VertexPositionTexture(a, Vector2.Zero));
            vpts.Add(new VertexPositionTexture(b, Vector2.Zero));
            vpts.Add(new VertexPositionTexture(c, Vector2.Zero));
            inds.Add((short) inds.Count);
            inds.Add((short) inds.Count);
            inds.Add((short) inds.Count);
        }
    }
}