﻿using System;
using System.Collections.Generic;
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
        public Keys up = Keys.Up;
        public Keys down = Keys.Down;
        public Keys jump = Keys.Space;
        private Keys abilityOneKey = Keys.J;

        public Ability abilityOne;
        public Ability abilityTwo;
        public Ability abilityThree;
        public Ability abilityFour;

        private TextureDrawer texture;

        public RopeSegment rope;
        public bool ropeCollide;

        public float maxSpeed = 5f;

        public float jumpSpeed = 10f;

        public float health = MaxHealth;

        public const float MaxHealth = 100;

        private bool isHealing;
        private const int HealTimerReset = 60;
        private int healTimer = HealTimerReset;

        public int passiveHealingAmount = 5;

        private bool shouldHeal;
        private const int ShouldHealTimerReset = 120;
        private int shouldHealTimer = ShouldHealTimerReset;

        private int money;

        private Direction facing = Direction.Right;

        private readonly FloatingIndicatorBuilder healBuilder;
        private readonly FloatingIndicatorBuilder damageBuilder;

        private readonly List<Item> items;

        private float itemLoc = 32;

        private MoneyDisplay moneyDisplay;

        public Player(float x, float y, float width, float height) {
            bounds = new RectangleF(x, y, width, height);
            texture = new TextureDrawer("playerIdle");

            healthBar = new HealthBarBuilder() { Position = new Vector2(x, y), Width = (int)width + 10 }.Build();

            abilityOne = new BurstFireAbility(this);
            abilityTwo = new BurstFireAbility(this);
            abilityThree = new BurstFireAbility(this);
            abilityFour = new BurstFireAbility(this);

            healBuilder = new FloatingIndicatorBuilder() { Color = Color.Green };
            damageBuilder = new FloatingIndicatorBuilder() { Color = Color.Red };
            moneyDisplay = new MoneyDisplay(new Vector2(15, 15));
            items = new List<Item>();

            rope = null;
        }

        public override void Update(EventList<GameObject> objects) {
            var state = Playing.Instance.state;

            if (rope != null) {
                velocity.X = 0;
                if (state.IsKeyDown(up))
                    velocity.Y = -jumpSpeed / 2;
                else if (state.IsKeyDown(down))
                    velocity.Y = jumpSpeed / 2;
                else
                    velocity = Vector2.Zero;

                bounds.x -= bounds.Center.X - rope.bounds.Center.X;

                //update rope
                rope.UpdatePlayer(this);

                foreach (var i in items) i.Update(objects);

                UpdatePosition(true);
                UpdatePassiveHealing();
                ChangeAnimation("playerIdle");
                return;
            }

            abilityOne.Update();
            
            if (state.IsKeyDown(left)) {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
                ChangeAnimation("playerWalking");
            } else if (state.IsKeyDown(right)) {
                facing = Direction.Right;
                velocity.X = maxSpeed;
                ChangeAnimation("playerWalking");
            } else {
                velocity.X = 0;
                ChangeAnimation("playerIdle");
            }

            foreach (var i in items) i.Update(objects);

            if (state.IsKeyDown(jump) && collision.HasFlag(Collision.Bottom)) {
                Jump();
            }

            if (state.IsKeyDown(abilityOneKey)) abilityOne.Fire(objects);

            collision = Collision.None;

            UpdatePosition(false);

            UpdatePassiveHealing();
        }

        public void UpdateHealthBar()
        {
            healthBar.AlignHorizontally((Rectangle)(bounds));
            healthBar.SetY((bounds).y - 20);
            healthBar.SetHealth(health);
        }

        public void UpdatePosition(bool onRope) {
            if (!onRope)
                velocity += Playing.Instance.gravity;

            velocity.Y = MathHelper.Clamp(velocity.Y, -15, 15);
            bounds.x += velocity.X;
            bounds.y += velocity.Y;
        }

        public void UpdateViewport()
        {
            Playing.Instance.offset.X = 1280 / 2 - bounds.Center.X;
            Playing.Instance.offset.Y = 720 / 2 - bounds.Center.Y;
        }

        public void UpdatePassiveHealing()
        {
            if (!shouldHeal)
            {
                shouldHealTimer--;

                isHealing = false;

                if (shouldHealTimer <= 0)
                {
                    shouldHealTimer = ShouldHealTimerReset;

                    if (health < MaxHealth) shouldHeal = true;
                }
            }
            else
            {
                isHealing = true;
            }

            if (isHealing) healTimer--;

            if (healTimer <= 0)
            {
                healTimer = HealTimerReset;
                Heal(passiveHealingAmount);
            }

            if (health >= MaxHealth)
            {
                shouldHeal = false;

                if (health > MaxHealth)
                {
                    health = MaxHealth;
                }
            }
        }

        public void Jump()
        {
            velocity.Y = -jumpSpeed;
        }

        public override void Draw(SpriteBatch sb) {
            texture.Draw(sb, (Rectangle)(bounds + Playing.Instance.offset), Facing());
            foreach (var i in items) i.Draw(sb);
            healthBar.Draw(sb);
            moneyDisplay.Draw(sb);
            
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

            Playing.Instance.objects.Add(healBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));
        }

        public void Damage(float amount)
        {
            health -= amount;
            shouldHeal = false;
            shouldHealTimer = ShouldHealTimerReset;

            Playing.Instance.objects.Add(damageBuilder.Build((int)amount, new Vector2(bounds.Center.X, bounds.y)));

            if (health <= 0) {
                Destroy();
            }
        }

        public void AddMoney(int amount) {
            money += amount;
            moneyDisplay.SetMoney(money);
        }

        public void SpendMoney(int amount) {
            money -= amount;
            moneyDisplay.SetMoney(money);
        }

        public bool HasEnoughMoney(int amount) {
            return money >= amount;
        }

        public void Collide(Collision side, float amount, ICollider origin)
        {
            if (rope != null)
            {
                ropeCollide = true;

                return;
            }

            collision |= side;

            if (side == Collision.Right) {
                bounds.x += amount;
                velocity.X = 0;
            }
            else if (side == Collision.Left) {
                bounds.x -= amount;
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

        private void ChangeAnimation(string name) {
            if (texture.name != name)
                texture = new TextureDrawer(name);
        }

        public void AddItem(Item i) {
            items.Add(i);
            i.AddedToPlayer(this, ref itemLoc);
        }

    }
}
