using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation {
    class Character {
        public float scale;
        public RectangleF bounds;
        public Texture2D texture;
        public TextureInfo info;
        public string type;
        public Color color;
        public bool isStatic;

        public Character(string type, float scale, bool isStatic = false) : this(type, scale, Color.White, isStatic) {}

        public Character(string type, float scale, Color color, bool isStatic = false) {
            this.scale = scale;
            this.type = type;
            this.color = color;
            this.isStatic = isStatic;
            this.info = TextureManager.blocks[type];
            this.texture = TextureManager.textures[info.texName];
            this.bounds = new RectangleF(0, 0, info.src.Width * scale, info.src.Height * scale);
        }

        public void SetPos(float x, float y) {
            bounds.x = x;
            bounds.y = y;
        }

        public void Draw(SpriteBatch sb, float alpha = 1) {
            if (isStatic)
                sb.Draw(texture, (Rectangle) bounds, info.src, color);
            else
                sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), info.src, color * alpha);
        }
    }
}