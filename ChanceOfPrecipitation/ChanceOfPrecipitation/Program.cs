namespace ChanceOfPrecipitation {
#if WINDOWS || XBOX
    internal static class Program {
        private static void Main(string[] args) {
            using (var game = new Game1()) {
                game.Run();
            }
        }
    }
#endif
}