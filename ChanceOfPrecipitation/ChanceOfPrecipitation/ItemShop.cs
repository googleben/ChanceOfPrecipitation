using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public class ItemShop : GameObject, ICollider {
        private BlockInfo info;
        public RectangleF bounds;

        private ItemStand itemA, itemB, itemC;

        // TODO: ADD CURRENCY
        public ItemShop(float x, float y, Item a, Item b, Item c) {
            itemA = new ItemStand(this, a, 0);
            itemB = new ItemStand(this, b, 1);
            itemC = new ItemStand(this, c, 2);

            info = TextureManager.blocks["shop"];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public override void Draw(SpriteBatch sb) {
        }

        public void Collide(ICollidable c) {
            itemA.Collide(c);
            itemB.Collide(c);
            itemC.Collide(c);
        }
    }

    public class ItemStand : GameObject, ICollider {
        private Item item;
        private ItemShop origin;
        private RectangleF bounds;

        public ItemStand(ItemShop origin, Item item, int itemIndex) {
            this.origin = origin;
            this.item = item;

            // TODO: Instantiate bounds
            bounds = new RectangleF();
        }

        public override void Update(List<GameObject> objects) {
            
        }

        public override void Draw(SpriteBatch sb) {
            
        }

        public void Collide(ICollidable c) {
            
        }
    }
}
