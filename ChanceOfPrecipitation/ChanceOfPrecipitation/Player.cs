using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    public class Player : GameObject, ICollidable, IEntity {
        public RectangleF bounds;
        private Vector2 velocity;
        private Collision collision;

        private readonly HealthBar healthBar;

        private Keys left = Keys.A;
        private Keys right = Keys.D;
        public Keys up = Keys.W;
        public Keys down = Keys.S;
        public Keys jump = Keys.Space;
        private Keys abilityOneKey = Keys.J;
        private Keys abilityTwoKey = Keys.K;
        private Keys abilityThreeKey = Keys.L;
        private Keys abilityFourKey = Keys.OemSemicolon;

        public Ability abilityOne;
        public Ability abilityTwo;
        public Ability abilityThree;
        public Ability abilityFour;

        public HUD hud;
        public Text healthText;

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

            healthBar = new HealthBarBuilder() {IsPlayer = true}.Build();
            healthBar.AlignHorizontally(new Rectangle(0, 0, 1280, 720));

            abilityOne = new BurstFireAbility(this);
            abilityTwo = new PenetratingAbility(this);
            abilityThree = new JumpAbility(this);
            abilityFour = new FastFireAbility(this);
            hud = new HUD(this);
            healthText = new Text(health + "/" + MaxHealth, Vector2.Zero, 1f, Color.White, true) {IsVisible = true};
            healthText.AlignToCenter(healthBar.BorderBounds);

            healBuilder = new FloatingIndicatorBuilder() {Color = Color.Green};
            damageBuilder = new FloatingIndicatorBuilder() {Color = Color.Red};
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

                UpdateAbilities();

                bounds.x -= bounds.Center.X - rope.bounds.Center.X;

                //update rope
                rope.UpdatePlayer(this);

                hud.Update(objects);

                foreach (var i in items) i.Update(objects);

                UpdatePosition(true);
                UpdatePassiveHealing();
                ChangeAnimation("playerIdle");
                return;
            }

            UpdateAbilities();

            if (state.IsKeyDown(left)) {
                facing = Direction.Left;
                velocity.X = -maxSpeed;
                ChangeAnimation("playerWalking");
            }
            else if (state.IsKeyDown(right)) {
                facing = Direction.Right;
                velocity.X = maxSpeed;
                ChangeAnimation("playerWalking");
            }
            else {
                velocity.X = 0;
                ChangeAnimation("playerIdle");
            }

            foreach (var i in items) i.Update(objects);

            if (state.IsKeyDown(jump) && collision.HasFlag(Collision.Bottom)) {
                Jump();
            }

            if (state.IsKeyDown(abilityOneKey)) abilityOne.Fire(objects);
            if (state.IsKeyDown(abilityTwoKey)) abilityTwo.Fire(objects);
            if (state.IsKeyDown(abilityThreeKey)) abilityThree.Fire(objects);
            if (state.IsKeyDown(abilityFourKey)) abilityFour.Fire(objects);

            collision = Collision.None;

            UpdatePosition(false);

            UpdatePassiveHealing();
        }

        public void UpdateHealthBar() {
            healthBar.SetHealth(health);
            healthText.SetText(health + "/" + MaxHealth);
            healthText.AlignHorizontally(healthBar.BorderBounds);
        }

        public void UpdatePosition(bool onRope) {
            if (!onRope)
                velocity += Playing.Instance.gravity;

            velocity.Y = MathHelper.Clamp(velocity.Y, -15, 15);
            bounds.x += velocity.X;
            bounds.y += velocity.Y;
        }

        public void UpdateAbilities() {
            abilityOne.Update();
            abilityTwo.Update();
            abilityThree.Update();
            abilityFour.Update();
        }

        public void UpdateViewport() {
            Playing.Instance.offset.X = 1280 / 2 - bounds.Center.X;
            Playing.Instance.offset.Y = 720 / 2 - bounds.Center.Y;
        }

        public void UpdatePassiveHealing() {
            if (!shouldHeal) {
                shouldHealTimer--;

                isHealing = false;

                if (shouldHealTimer <= 0) {
                    shouldHealTimer = ShouldHealTimerReset;

                    if (health < MaxHealth) shouldHeal = true;
                }
            }
            else {
                isHealing = true;
            }

            if (isHealing) healTimer--;

            if (healTimer <= 0) {
                healTimer = HealTimerReset;
                Heal(passiveHealingAmount);
            }

            if (health >= MaxHealth) {
                shouldHeal = false;

                if (health > MaxHealth) {
                    health = MaxHealth;
                }
            }
        }

        public void Jump() {
            velocity.Y = -jumpSpeed;
        }

        public void Jump(float scale) {
            velocity.Y = -jumpSpeed * scale;
        }

        public override void Draw(SpriteBatch sb) {
            texture.Draw(sb, (Rectangle) (bounds + Playing.Instance.offset), Facing());
            foreach (var i in items) i.Draw(sb);
            moneyDisplay.Draw(sb);
            hud.Draw(sb);
            healthBar.Draw(sb);
            healthText.Draw(sb);
        }

        public RectangleF Bounds() {
            return bounds;
        }

        public float Health() {
            return health;
        }

        public void Heal(float amount) {
            health += amount;

            Playing.Instance.objects.Add(healBuilder.Build((int) amount, new Vector2(bounds.Center.X, bounds.y)));
        }

        public void Damage(float amount) {
            health -= amount;
            shouldHeal = false;
            shouldHealTimer = ShouldHealTimerReset;

            Playing.Instance.objects.Add(damageBuilder.Build((int) amount, new Vector2(bounds.Center.X, bounds.y)));

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

        public void Collide(Collision side, float amount, ICollider origin) {
            if (rope != null) {
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

        float itemLocY = 620;

        public void AddItem(Item i) {
            items.Add(i);
            i.AddedToPlayer(this, ref itemLoc, ref itemLocY);
            if (itemLoc < 890 && itemLoc > 390 - 40) itemLoc = 890 + 4;
            if (itemLoc > 1280 - 40)
            {
                itemLoc = 32;
                itemLocY += 32;
            }
        }
    }
}