﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    public class Player : GameObject, ICollidable, IEntity {
        private RectangleF bounds;
        private Vector2 velocity;
        private Collision collision;

        private readonly HealthBar healthBar;

        private Keys left = Keys.Left;
        private Keys right = Keys.Right;
        private Keys up = Keys.Up;
        private Keys down = Keys.Down;
        private Keys jump = Keys.Space;
        private Keys abilityOneKey = Keys.J;

        public Ability abilityOne;

        private readonly Texture2D texture;

        public float maxSpeed = 5f;

        public float jumpSpeed = 10f;

        public float health = MaxHealth;

        public const float MaxHealth = 100;

        private bool isHealing;
        private const int HealTimerReset = 60;
        private int healTimer = HealTimerReset;

        private const int PassiveHealingAmount = 5;

        private bool shouldHeal;
        private const int ShouldHealTimerReset = 120;
        private int shouldHealTimer = ShouldHealTimerReset;

        private Direction facing = Direction.Right;

        private readonly FloatingIndicatorBuilder healBuilder;
        private readonly FloatingIndicatorBuilder damageBuilder;

        private readonly List<Item> items;

        private float itemLoc = 32;

        public Player(float x, float y, float width, float height) {
            bounds = new RectangleF(x, y, width, height);
            texture = TextureManager.textures["Square"];

            healthBar = new HealthBarBuilder() { Position = new Vector2(x, y), Width = (int)width + 10 }.Build();

            abilityOne = new BurstFireAbility(this);

            healBuilder = new FloatingIndicatorBuilder() { Color = Color.Green };
            damageBuilder = new FloatingIndicatorBuilder() { Color = Color.Red };
            items = new List<Item>();
        }

        public override void Update(List<GameObject> objects) {
            var state = Playing.Instance.state;

            abilityOne.Update();
            
            if (state.IsKeyDown(left)) {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
            } else if (state.IsKeyDown(right)) {
                facing = Direction.Right;
                velocity.X = maxSpeed;
            } else {
                velocity.X = 0;
            }

            foreach (var i in items) i.Update(objects);

            if (state.IsKeyDown(jump) && collision.HasFlag(Collision.Bottom)) {
                velocity.Y = -jumpSpeed;
            }

            if (state.IsKeyDown(abilityOneKey)) abilityOne.Fire(objects); // TODO: Add ability

            collision = Collision.None;

            velocity += Playing.Instance.gravity;
            bounds.x += velocity.X;
            bounds.y += velocity.Y;

            healthBar.AlignHorizontally((Rectangle)Bounds());
            healthBar.SetY(Bounds().y - 20);

            if (!shouldHeal) {
                shouldHealTimer--;

                isHealing = false;

                if (shouldHealTimer <= 0) {
                    shouldHealTimer = ShouldHealTimerReset;

                    if (health < MaxHealth) shouldHeal = true;
                }
            } else {
                isHealing = true;
            }

            if (isHealing) healTimer--;

            if (healTimer <= 0) {
                healTimer = HealTimerReset;
                Heal(PassiveHealingAmount);
            }
        }

        public override void Draw(SpriteBatch sb) {
            sb.Draw(texture, (Rectangle)bounds, Color.White);
            foreach (var i in items) i.Draw(sb);
            healthBar.Draw(sb);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public float Health() {
            return health;
        }

        public void Heal(float amount)
        {
            health += amount;
            healthBar.Heal(amount);

            Playing.Instance.objects.Add(healBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));
        }

        public void Damage(float amount)
        {
            health -= amount;
            healthBar.Damage(amount);
            shouldHeal = false;
            shouldHealTimer = ShouldHealTimerReset;

            Playing.Instance.objects.Add(damageBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));

            if (health <= 0)
                Destroy();
        }

        public void Collide(Collision side, float amount, ICollider origin)
        {
            collision |= side;

            if (side == Collision.Right) {
                bounds.x -= amount;
                velocity.X = 0;
            }
            else if (side == Collision.Left) {
                bounds.x += amount;
                velocity.X = 0;
            }
            else if (side == Collision.Top) {
                bounds.y += amount;
                velocity.Y = 0;
            }
            else if (side == Collision.Bottom) {
                bounds.y -= amount;
                velocity.Y = 0;
            }
        }

        public Direction Facing() {
            return facing;
        }

        public void AddItem(Item i) {
            items.Add(i);
            i.AddedToPlayer(this, ref itemLoc);
        }

    }
}
