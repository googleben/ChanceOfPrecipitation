using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChanceOfPrecipitation {
    class Playing : GameState {

        private static Playing instance;

        public static Playing Instance => instance ?? new Playing();

        public List<GameObject> objects;

        public Vector2 gravity = new Vector2(0, 1);

        public KeyboardState state;
        public KeyboardState lastState;

        public Player player;

        public Playing() {
            objects = new List<GameObject>();
            instance = this;
            lastState = state = Keyboard.GetState();
            player = new Player(0, 0, 64, 64);
            objects.Add(player);
        }

        public void Draw(SpriteBatch sb) {
            foreach (var o in objects) o.Draw(sb);
        }

        public GameState Update() {

            lastState = state;
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape)) {
                Menu.lastState = state;
                return new MainMenu();
            }

            return this;
        }
    }
}
