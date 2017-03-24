using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    internal class Playing : IGameState {

        private static Playing instance;

        public static Playing Instance => instance ?? new Playing();

        public List<GameObject> objects;

        public Vector2 gravity = new Vector2(0, 1);

        public KeyboardState state;
        public KeyboardState lastState;

        public List<Player> players;

        public Playing() {
            objects = new List<GameObject>();
            instance = this;
            lastState = state = Keyboard.GetState();
            players = new List<Player>() {new Player(0, 0, 16, 32)};

            objects.Add(players[0]);
            objects.Add(new Block(0, 600,  "stage1_platform_top_left"));
            objects.Add(new Block(1280 - 16, 600, "stage1_platform_top_right"));
            for (var i = 16; i < 1280 - 16; i += 16) {
                objects.Add(new Block(i, 600, "stage1_platform_top_middle"));
            }

            objects.Add(new BasicEnemy(600, 0));
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(TextureManager.textures["Square"], new Rectangle(0, 0, 1280, 720), Color.MidnightBlue);
            foreach (var o in objects) o.Draw(sb);
        }

        public IGameState Update() {

            lastState = state;
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape)) {
                Menu.lastState = state;
                return new MainMenu();
            }

            var collidables = new LinkedList<ICollidable>();
            var statics = new LinkedList<ICollider>();

            for (var i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (o.ToDestroy) { objects.RemoveAt(i--); continue; }
                o.Update(objects);
                if (o.ToDestroy) objects.RemoveAt(i--);
                else
                {
                    if (o is ICollidable) collidables.AddLast(o as ICollidable);
                    if (o is ICollider) statics.AddLast(o as ICollider);
                }
            }

            foreach (var s in statics)
            {
                foreach (var c in collidables) s.Collide(c);
            }

            return this;
        }
    }
}
