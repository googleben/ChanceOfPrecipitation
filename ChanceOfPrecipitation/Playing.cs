﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation
{
    internal class Playing : IGameState
    {
        private static Playing instance;
        public static Random random = new Random();

        public static Playing Instance => instance ?? new Playing();

        public EventList<GameObject> objects;

        public Vector2 gravity = new Vector2(0, 1);

        public KeyboardState state;
        public KeyboardState lastState;

        public List<Player> players;

        public Vector2 offset = Vector2.Zero;

        public Vector2 spawnRange = new Vector2(1000, 400);
        public Vector2 noSpawnRange = new Vector2(300, 100);
        private int ticksToSpawn = 360;
        private int minTicksToSpawn = 360;
        private int ticksToSpawnRange = 120;

        ProceduralGenerator pgen = new ProceduralGenerator();

        public QuadTree quad;

        bool update = true;

        bool drawQuad = Game1.Instance.settings.drawQuads;

        RectangleF worldBounds;

        public Playing()
        {
            objects = new EventList<GameObject>();
            instance = this;
            random = new Random();
            lastState = state = Keyboard.GetState();
            players = new List<Player>();
            players.Add(new Player(64, -180, 16, 32));


            //LoadStage("level");
            GenStage();
        }

        Parallax parallax = new Parallax(TextureManager.textures["background"], .1f);

        public void Draw(SpriteBatch sb)
        {
            var b = players[0].bounds;
            //sb.Draw(TextureManager.textures["background"], new Rectangle(0, 0, 1280, 720),  new Rectangle((int)((b.x/worldBounds.width)*720), (int)((b.y/worldBounds.height)*720), 1280, 720), Color.White);
            parallax.Draw(sb, b);
            var x = players[0];
            x.UpdateViewport();
            x.UpdateHealthBar();
            foreach (var o in objects) if (!(o is Player)) o.Draw(sb);
            foreach (var p in players) if (p.health > 0) p.Draw(sb);
            if (drawQuad) quad.Draw(sb);

            /*//draw spawn ranges
            sb.Draw(TextureManager.textures["Square"], (Rectangle)(new RectangleF(b.x - spawnRange.X / 2, b.y - spawnRange.Y / 2, spawnRange.X,
                    spawnRange.Y) + offset), Color.White);
            sb.Draw(TextureManager.textures["Square"], (Rectangle)(new RectangleF(b.x - noSpawnRange.X / 2, b.y - noSpawnRange.Y / 2,
                    noSpawnRange.X, noSpawnRange.Y)+offset), Color.Red);*/
        }

        void printQuad(QuadTree q)
        {
            if (q.nodes != null) foreach (var n in q.nodes) printQuad(n);
            else if (q.dynamics.Count != 0) Console.WriteLine(q.dynamics.Count + " " + q.statics.Count);
        }

        public IGameState Update()
        {
            if (!update) return this;

            lastState = state;
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape) || players.All(p => p.health <= 0)) {
                Menu.lastState = state;
                instance = null;
                return new MainMenu();
            }

            var collidables = new LinkedList<ICollidable>();
            var statics = new LinkedList<ICollider>();

            for (var i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (o.ToDestroy) {
                    objects.RemoveAt(i--);
                    continue;
                }
                o.Update(objects);
                if (o.ToDestroy) {
                    if (o is Player) objects.RemoveAt(i--);
                } else {
                    if (o is ICollidable) collidables.AddLast(o as ICollidable);
                    if (o is ICollider) statics.AddLast(o as ICollider);
                }
            }


            quad.RunCollision();
            quad.CheckDynamics();

            if (ticksToSpawn == 0) {
                SpawnEnemy();
                ticksToSpawn = minTicksToSpawn + random.Next(ticksToSpawnRange);
            }
            ticksToSpawn--;

            return this;
        }

        void SpawnEnemy()
        {
            var spawnAreas = new List<RectangleF>();
            var spawnRestrictions = new List<RectangleF>();
            foreach (var p in players) {
                var bounds = p.Bounds();
                spawnAreas.Add(new RectangleF(bounds.x - spawnRange.X / 2, bounds.y - spawnRange.Y / 2, spawnRange.X,
                    spawnRange.Y));
                spawnRestrictions.Add(new RectangleF(bounds.x - noSpawnRange.X / 2, bounds.y - noSpawnRange.Y / 2,
                    noSpawnRange.X, noSpawnRange.Y));
            }
            var enemy = new BasicEnemy(0, 0);
            var height = enemy.Bounds().height;
            var width = enemy.Bounds().width;
            var blocks = new List<Block>();
            var qss = new List<QuadTree>();
            foreach (var r in spawnAreas) quad.GetPos(r).ForEach(qss.Add);
            foreach (var q in qss) q.statics.OfType<Block>().ToList().ForEach(x => blocks.Add(x as Block));
            var spawns = new List<RectangleF>();
            foreach (var b in blocks) {
                var bounds = b.bounds;
                var spawn = new RectangleF(bounds.x, bounds.y - height, width, height);
                var works = true;
                foreach (var x in spawnAreas)
                    if (!spawn.Intersects(x)) {
                        works = false;
                        break;
                    }
                foreach (var x in spawnRestrictions)
                    if (!works || spawn.Intersects(x)) {
                        works = false;
                        break;
                    }
                if (works) {
                    var qs = quad.GetPos(spawn);
                    foreach (var q in qs)
                        if (q.DoesCollide(spawn)) {
                            works = false;
                            break;
                        }
                }
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

        void LoadStage(string stage)
        {
            objects.Clear();
            Level l = Level.levels[stage];
            foreach (var x in l.blocks) {
                x.Build(this);
            }
            foreach (var p in players) objects.Add(p);
        }

        PLevel l;

        void GenStage()
        {
            update = false;
            objects.Clear();
            l = pgen.GenLevel();
            var b = l.Build();
            foreach (var x in b) x.Build(this);
            foreach (var p in players) objects.Add(p);
            var size = worldBounds = l.Bounds();
            var len = Math.Max(size.width, size.height) + 2000;
            quad = new QuadTree(size.x - (len - size.width) / 2, size.y - (len - size.height) / 2, len, len,
                objects.OfType<ICollider>().ToList(), null);
            objects.OfType<ICollidable>().ToList().ForEach(quad.AddDynamic);
            objects.OnAdd += (e, args) => {
                if (e is ICollidable) quad.AddDynamic(e as ICollidable);
                else if (e is ICollider) quad.AddStatic(e as ICollider);
            };
            update = true;
            players.ForEach(quad.AddDynamic);
            players.ForEach(p => { p.bounds.x = 64; p.bounds.y = -180; });
            GenObjs();
        }

        public void GenObjs()
        {
            List<Block> possible = objects.OfType<Block>().Where(b => b.type == "stage1_platform_top_middle").ToList();
            int placed = 0;
            for (int times = 0; times < 20 && placed < 10; times++) {
                int iter = 0;
                for (int i = random.Next(possible.Count); iter < possible.Count && placed < 10; i++, iter++) {
                    if (i >= possible.Count) i = 0;
                    Block b = possible[i];
                    RectangleF pos = new RectangleF(b.bounds.x, b.bounds.y - 144, 192, 144);
                    var qs = quad.GetPos(pos);

                    foreach (var q in qs) if (q.DoesCollide(pos)) goto bottom;
                    for (int j = 1; j < 7; j++) {
                        RectangleF here = new RectangleF(b.bounds.x + (32 * j), b.bounds.y, 32, 32);
                        qs = quad.GetPos(here);
                        foreach (var q in qs) if (!q.DoesCollide(here)) goto bottom;
                    }
                    placed++;
                    ShopPlacementInfo p = new ShopPlacementInfo(pos.x, pos.y, "money", "damage", "healing");
                    p.Build(this);
                    Console.WriteLine("Shop");
                    break;
                    bottom:
                    ;
                }
            }
        }

        public void NextStage()
        {
            GenStage();
        }
    }
}