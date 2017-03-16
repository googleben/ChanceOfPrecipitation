namespace ChanceOfPrecipitation {
    internal class Enemies {

        public static Enemy enemy1 = new Enemy(0, 0, 16, 32);

        static Enemies() {
            enemy1.MaxHealth = 100;
        }
    }
}
