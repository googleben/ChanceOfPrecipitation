﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            for (var i = 0; i < 1280 - 64; i += 64) {
                objects.Add(new Block(i, 600, "Square"));
            }
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

            LinkedList<ICollidable> collidables = new LinkedList<ICollidable>();
            LinkedList<IStaticObject> statics = new LinkedList<IStaticObject>();

            for (var i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (o.toDestroy) { objects.RemoveAt(i--); continue; }
                o.Update(objects);
                if (o.toDestroy) objects.RemoveAt(i--);
                else
                {
                    if (o is ICollidable) collidables.AddLast(o as ICollidable);
                    if (o is IStaticObject) statics.AddLast(o as IStaticObject);
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
