using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class ItemShop : GameObject, ICollider {
        public const int SizeMultiplier = 3;

        private readonly BlockInfo info;
        public RectangleF bounds;

        private readonly ItemStand itemA;
        private readonly ItemStand itemB;
        private readonly ItemStand itemC;

        public bool BoughtItem { get; set; } = false;

        // TODO: ADD CURRENCY
        public ItemShop(float x, float y, Item a, Item b, Item c) {
            itemA = new ItemStand(this, a, x, y);
            itemB = new ItemStand(this, b, x + 24 * SizeMultiplier, y);
            itemC = new ItemStand(this, c, x + 48 * SizeMultiplier, y);

            info = TextureManager.blocks["shop"];
            bounds = new RectangleF(x, y, info.src.Width * info.scale * SizeMultiplier, info.src.Height * info.scale * SizeMultiplier);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public override void Draw(SpriteBatch sb) {
            itemA.Draw(sb);
            itemB.Draw(sb);
            itemC.Draw(sb);
        }

        public void Collide(ICollidable c) {
            itemA.Collide(c);
            itemB.Collide(c);
            itemC.Collide(c);
        }
    }

    public class ItemStand : GameObject, ICollider {
        private const float ItemScale = ItemShop.SizeMultiplier * 0.3f;

        private readonly Item item;
        private ItemShop origin;
        private readonly RectangleF bounds;
        private readonly RectangleF itemBounds;
        private readonly BlockInfo info;
        private readonly Texture2D texture;

        public ItemStand(ItemShop origin, Item item, float x, float y) {
            this.origin = origin;
            this.item = item;

            info = TextureManager.blocks["stand"];
            texture = TextureManager.textures[info.texName];

            bounds = new RectangleF(x, y, info.src.Width * info.scale * ItemShop.SizeMultiplier, info.src.Height * info.scale * ItemShop.SizeMultiplier);
            itemBounds = new RectangleF(bounds.Center.X - item.info.src.Width / 2 * ItemScale, bounds.Center.Y - bounds.height / 4, item.info.src.Width * ItemScale, item.info.src.Height * ItemScale);
        }

        public override void Update(List<GameObject> objects) {

        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), info.src, Color.White);
            sb.Draw(item.texture, (Rectangle) (itemBounds + Playing.Instance.offset), item.info.src, Color.White);
        }

        public void Collide(ICollidable c) {
            if (!(c is Player) || !c.Bounds().Intersects(bounds)) return;

            // TODO: Prompt player to select item

            if (false) { // if player selected item
                ((Player) c).AddItem(item);
                origin.BoughtItem = true;
            }
        }
    }
}
