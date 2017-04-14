using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation {

    #region Enemies
    public class BasicEnemy : Enemy {
        public BasicEnemy(float x, float y) : base(x, y, 16, 32) {
            texture = TextureManager.textures["Square"];
            abilities = new EnemyAbility[] { new EnemyMeleeAbility(this), new EnemyLazerAbility(this), };
        }
    }

    public class ShooterEnemy : Enemy {
        public ShooterEnemy(float x, float y) : base(x, y, 12, 32) {
            texture = TextureManager.textures["Square"];
            abilities = new EnemyAbility[] { new EnemyMeleeAbility(this), new EnemyBulletAbility(this) };
            color = Color.Green;

            if (!healthBar.IsBoss)
            {
                healthBar.AlignHorizontally((Rectangle)(bounds + Playing.Instance.offset));
                healthBar.SetY((bounds + Playing.Instance.offset).y - 20);
            }
        }

        public override void Update(List<GameObject> objects) {
            Playing.Instance.players.ForEach(p => {
                if (p.Bounds().Center.X < bounds.Center.X)
                    facing = Direction.Left;
                else
                    facing = Direction.Right;
            });

            UpdateAbilities(objects);
            UpdatePosition();
            UpdateHealthBar();
        }
    }
    #endregion

    #region Bosses
    public abstract class BossEnemy : Enemy {
        public BossEnemy(float x, float y, int width, int height, int health = 1000) : base(x, y, width, height) {
            MaxHealth = health;
            healthBar = new HealthBarBuilder { IsBoss = true, MaxHealth = health }.Build();
        }
    }

    public class TestBoss : BossEnemy {
        public TestBoss(float x, float y) : base(x, y, 64, 128) {
            texture = TextureManager.textures["Square"];
            abilities = new EnemyAbility[] { new EnemyMeleeAbility(this) };
        }

        public new int Value() {
            return 5 * Coin.Value;
        }
    }
    #endregion
}
