using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class ItemShop : GameObject, ICollider {
        public const int SizeMultiplier = 3;

        public RectangleF bounds;

        private readonly ItemStand itemA;
        private readonly ItemStand itemB;
        private readonly ItemStand itemC;

        public bool BoughtItem { get; set; }

        // TODO: ADD CURRENCY
        public ItemShop(float x, float y, Item a, Item b, Item c) {
            itemA = new ItemStand(this, a, x, y);
            itemB = new ItemStand(this, b, x + 24 * SizeMultiplier, y);
            itemC = new ItemStand(this, c, x + 48 * SizeMultiplier, y);

            var info = TextureManager.blocks["shop"];
            bounds = new RectangleF(x, y, info.src.Width * info.scale * SizeMultiplier, info.src.Height * info.scale * SizeMultiplier);
        }

        public override void Update(List<GameObject> objects) {
            itemA.Update(objects);
            itemB.Update(objects);
            itemC.Update(objects);
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
        private RectangleF itemBounds;
        private readonly BlockInfo info;
        private readonly Texture2D texture;

        private float origY;
        private float amplitude;
        private const float Wavelength = 0.002f;
        private float waveCounter;
        private readonly float phase;

        private Text text;
        private readonly Keys buyKey;
        private readonly int cost;
        private Player player;
        private bool intersectingPlayer = false;

        public ItemStand(ItemShop origin, Item item, float x, float y) {
            this.origin = origin;
            this.item = item;

            player = new Player(0, 0, 0, 0);

            info = TextureManager.blocks["stand"];
            texture = TextureManager.textures[info.texName];

            bounds = new RectangleF(x, y, info.src.Width * info.scale * ItemShop.SizeMultiplier, info.src.Height * info.scale * ItemShop.SizeMultiplier);
            itemBounds = new RectangleF(bounds.Center.X - item.info.src.Width / ItemShop.SizeMultiplier * 4 / 3, bounds.Center.Y - bounds.height / 5, item.info.src.Width * ItemScale, item.info.src.Height * ItemScale);

            origY = itemBounds.y;
            amplitude = bounds.height / 14.4f;
            phase = (float) Playing.random.NextDouble() * 10;

            buyKey = Keys.E;
            text = new Text("press " + buyKey + " to buy item", Vector2.Zero);
            text.SetPos(bounds.Center.X - text.width / 2, bounds.Bottom + 5);

            cost = 10;
        }

        public override void Update(List<GameObject> objects) {
            waveCounter += Wavelength;

            itemBounds.y = (float) (amplitude * Math.Sin(2 * Math.PI * waveCounter + phase) + origY);

            text.IsVisible = intersectingPlayer;

            if (Playing.Instance.state.IsKeyDown(buyKey) && !Playing.Instance.lastState.IsKeyDown(buyKey) && intersectingPlayer) {
                if (player.HasEnoughMoney(cost)) {
                    player.SpendMoney(cost);
                    player.AddItem(item);
                    origin.BoughtItem = true;
                }
            }

            intersectingPlayer = false;
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle) (bounds + Playing.Instance.offset), info.src, Color.White);
            sb.Draw(item.texture, (Rectangle) (itemBounds + Playing.Instance.offset), item.info.src, Color.White * .5f);
            text.Draw(sb);
        }

        public void Collide(ICollidable c) {
            if (c is Player && c.Bounds().Intersects(bounds)) {
                player = c as Player;
                intersectingPlayer = true;
            }

        }
    }

    public class Coin : GameObject, ICollidable, ICollider {
        public const int Value = 9;
        private const int VelRange = 2;
        private const float VelDamper = -0.9f;

        private Vector2 velocity;
        private readonly Texture2D texture;
        private RectangleF bounds;
        private readonly BlockInfo info;

        private FloatingIndicatorBuilder indicator;

        public Coin(float x, float y) {
            info = TextureManager.blocks["coin"];
            bounds = new RectangleF(x, y, info.src.Width * info.scale, info.src.Height * info.scale);

            velocity.Y = Playing.random.Next(-VelRange, 0);
            velocity.X = Playing.random.Next(-VelRange * 2, VelRange * 2);

            texture = TextureManager.textures[info.texName];

            indicator = new FloatingIndicatorBuilder() { Color = Color.Gold };
        }

        public override void Update(List<GameObject> objects) {
            velocity += Playing.Instance.gravity;

            bounds.x += velocity.X;
            bounds.y += velocity.Y;

            if (Math.Abs(velocity.X) < 0.1f) velocity.X = 0;
            if (Math.Abs(velocity.Y) < 0.1f) velocity.Y = 0;
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
                Playing.Instance.objects.Add(indicator.Build(Value, bounds.Center));
            }
        }

        public void Collide(Collision side, float amount, ICollider origin) {
            if (side == Collision.Right) {
                bounds.x -= amount;
                velocity.X *= VelDamper;
            }
            else if (side == Collision.Left) {
                bounds.x += amount;
                velocity.X *= VelDamper;
            }
            else if (side == Collision.Top) {
                bounds.y += amount;
                velocity.Y *= VelDamper;
            }
            else if (side == Collision.Bottom) {
                bounds.y -= amount;
                velocity.X *= -VelDamper;
                velocity.Y *= VelDamper;
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
