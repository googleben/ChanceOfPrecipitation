namespace ChanceOfPrecipitation {

    public class BasicEnemy : Enemy {
        public BasicEnemy(float x, float y) : base(x, y, 16, 32) {
            texture = TextureManager.textures["Square"];
        }
    }
}
