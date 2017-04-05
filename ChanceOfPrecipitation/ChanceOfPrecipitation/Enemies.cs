using System.Collections.Generic;

namespace ChanceOfPrecipitation {

    #region Enemies
    public class BasicEnemy : Enemy {
        public BasicEnemy(float x, float y) : base(x, y, 16, 32) {
            texture = TextureManager.textures["Square"];
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
    }
    #endregion
}
