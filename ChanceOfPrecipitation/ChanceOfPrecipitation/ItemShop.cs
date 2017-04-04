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
        private readonly ItemShop origin;
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

    public class Coin : GameObject, ICollidable, ICollider {
        public const int Value = 5;
        private const int VelRange = 1;
        private const float Damper = -0.9f;

        private readonly Random rand;
        private Vector2 velocity;
        private readonly Texture2D texture;
        private RectangleF bounds;
        private readonly BlockInfo info;

        public Coin(float x, float y) {
            info = TextureManager.blocks["coin"];
            bounds = new RectangleF(x, y, info.src.Width, info.src.Height);

            rand = new Random();
            velocity.Y = rand.Next(-VelRange, 0);
            velocity.X = rand.Next(-VelRange, VelRange);

            texture = TextureManager.textures[info.texName];
        }

        public override void Update(List<GameObject> objects) {
            velocity += Playing.Instance.gravity;

            bounds.x += velocity.X;
            bounds.y += velocity.Y;

            if (Math.Abs(velocity.X) < 0.01f) velocity.X = 0;
            if (Math.Abs(velocity.Y) < 0.01f) velocity.Y = 0;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), info.src, Color.White);
        }

        public void Collide(ICollidable c)
        {
            if (c is Player && c.Bounds().Intersects(bounds)) {
                Destroy();
                ((Player) c).AddMoney(Value);
            }
        }

        public void Collide(Collision side, float amount, ICollider origin) {
            if (side == Collision.Right && velocity.X > 0) {
                velocity.X *= Damper;
            }
            else if (side == Collision.Left && velocity.X < 0) {
                velocity.X *= Damper;
            }
            else if (side == Collision.Top && velocity.Y < 0) {
                velocity.Y *= Damper;
            }
            else if (side == Collision.Bottom && velocity.Y > 0) {
                velocity.X *= -Damper;
                velocity.Y *= Damper;
            }
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public Direction Facing() {
            return velocity.X >= 0 ? Direction.Right : Direction.Left;
        }
    }
}
