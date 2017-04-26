using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    internal class Playing : IGameState {

        private static Playing instance;
        public static Random random = new Random();

        public static Playing Instance => instance ?? new Playing();

        public List<GameObject> objects;

        public Vector2 gravity = new Vector2(0, 1);

        public KeyboardState state;
        public KeyboardState lastState;

        public List<Player> players;

        public Vector2 offset = Vector2.Zero;

        public Vector2 spawnRange = new Vector2(1000, 300);
        public Vector2 noSpawnRange = new Vector2(200, 50);
        private int ticksToSpawn = 0;
        private int minTicksToSpawn = 360;
        private int ticksToSpawnRange = 120;

        public Playing() {
            objects = new List<GameObject>();
            instance = this;
            random = new Random();
            lastState = state = Keyboard.GetState();
            players = new List<Player>();
            players.Add(new Player(0, 50, 16, 32));

            objects.Add(players[0]);
            objects.Add(new Block(0, 600,  "stage1_platform_top_left"));
            objects.Add(new Block(1280 - 16, 600, "stage1_platform_top_right"));
            for (var i = 32; i < 1280 - 32; i += 32) {
                objects.Add(new Block(i, 600, "stage1_platform_top_middle"));
            }

            objects.Add(new BasicEnemy(600, 0));
            objects.Add(new ItemEntity<DamageUpgrade>(100, 550, DamageUpgrade.type));

            LoadStage("level");
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(TextureManager.textures["Square"], new Rectangle(0, 0, 1280, 720), Color.MidnightBlue);
            foreach (var o in objects) if (!(o is Player)) o.Draw(sb);
            foreach (var p in players) if (p.health>0) p.Draw(sb);
        }

        public IGameState Update() {

            lastState = state;
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape)) {
                Menu.lastState = state;
                instance = null;
                return new MainMenu();
            }

            var collidables = new LinkedList<ICollidable>();
            var statics = new LinkedList<ICollider>();

            for (var i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (o.ToDestroy) { objects.RemoveAt(i--); continue; }
                o.Update(objects);
                if (o.ToDestroy) { if (o is Player) objects.RemoveAt(i--); }
                else
                {
                    if (o is ICollidable) collidables.AddLast(o as ICollidable);
                    if (o is ICollider) statics.AddLast(o as ICollider);
                }
            }

            

            foreach (var s in statics) {
                foreach (var c in collidables) s.Collide(c);
            }

            if (ticksToSpawn == 0) {
                SpawnEnemy();
                ticksToSpawn = minTicksToSpawn + random.Next(ticksToSpawnRange);
            }
            ticksToSpawn--;

            return this;
        }

        void SpawnEnemy() {
            var spawnAreas = new List<RectangleF>();
            var spawnRestrictions = new List<RectangleF>();
            foreach (var p in players)
            {
                var bounds = p.Bounds();
                spawnAreas.Add(new RectangleF(bounds.x - spawnRange.X / 2, bounds.y - spawnRange.Y / 2, spawnRange.X, spawnRange.Y));
                spawnRestrictions.Add(new RectangleF(bounds.x - noSpawnRange.X / 2, bounds.y - noSpawnRange.Y / 2, noSpawnRange.X, noSpawnRange.Y));
            }
            var enemy = new BasicEnemy(0, 0);
            var height = enemy.Bounds().height;
            var width = enemy.Bounds().width;
            var blocks = objects.OfType<Block>();
            var spawns = new List<RectangleF>();
            foreach (var b in blocks) {
                var bounds = b.bounds;
                var spawn = new RectangleF(bounds.x, bounds.y-height, width, height);
                var works = true;
                foreach (var x in spawnAreas) if (!spawn.Intersects(x)) { works = false; break; }
                foreach (var x in spawnRestrictions) if (!works || spawn.Intersects(x)) { works = false; break; }
                if (works) foreach (var x in blocks) if (x.bounds.Intersects(spawn)) { works = false; break; }
                if (!works) continue;
                spawns.Add(spawn);
            }
            if (spawns.Count == 0) {
                return;
            }
            var ans = spawns[random.Next(spawns.Count)];
            enemy.SetPos(ans.x, ans.y);
            objects.Add(enemy);
        }

        void LoadStage(string stage) {
            objects.Clear();
            Level l = Level.levels[stage];
            foreach (var x in l.blocks) {
                x.Build(this);
            }
            foreach (var p in players) objects.Add(p);
        }

        public void NextStage() {
            LoadStage("level");
        }

    }
}
