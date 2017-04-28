﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{

    interface IPlacementInfo {
        void Build(Playing instance);
    }

    class BlockPlacementInfo : IPlacementInfo {
        public string type;
        public Vector2 position;

        public BlockPlacementInfo(string type, float x, float y) {
            this.type = type;
            this.position = new Vector2(x, y);
        }

        public void Build(Playing instance) {
            instance.objects.Add(new Block(position.X, position.Y, type));
        }
    }

    class RopePlacementInfo : IPlacementInfo
    {
        private int length;
        public Vector2 position;

        public RopePlacementInfo(float x, float y, int length)
        {
            this.length = length;
            position = new Vector2(x, y);
        }

        public void Build(Playing instance)
        {
            instance.objects.Add(new Rope(position.X, position.Y, length));
        }
    }

    class PlayerPlacementInfo : IPlacementInfo {
        public Vector2 position;

        public PlayerPlacementInfo(float x, float y)
        {
            this.position = new Vector2(x, y);
        }

        public void Build(Playing instance) {
            Player p = new Player(position.X, position.Y, 16, 32);
            instance.objects.Add(p);
            instance.players.Add(p);
        }
    }

    class ShopPlacementInfo : IPlacementInfo {
        public Vector2 position;
        private string item1;
        private string item2;
        private string item3;

        public ShopPlacementInfo(float x, float y, string item1, string item2, string item3)
        {
            this.position = new Vector2(x, y);
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        static Item buildItem(string name) {
            Item ans = null;
            if (name == "money") ans = new MoneyUpgrade();
            if (name == "damage") ans = new DamageUpgrade();
            if (name == "healing") ans = new HealingUpgrade();
            return ans;
        }

        public void Build(Playing instance)
        {
            instance.objects.Add(new ItemShop(position.X, position.Y, buildItem(item1), buildItem(item2), buildItem(item3), 20, 50));
        }
    }

    class PortalPlacementInfo : IPlacementInfo {
        public Vector2 position;

        public PortalPlacementInfo(float x, float y)
        {
            this.position = new Vector2(x, y);
        }

        public void Build(Playing instance)
        {
            instance.objects.Add(new Portal(position.X, position.Y));
        }
    }

    class Level {

        public static Dictionary<string, Level> levels;

        public List<IPlacementInfo> blocks;

        public Level(string raw, string name) {
            this.blocks = new List<IPlacementInfo>();
            var split = Regex.Split(raw, "\\s+");
            var scanner = split.Select<string, Func<Type, object>>((string s) => {
                return t =>
                (s as IConvertible).ToType(t, System.Globalization.CultureInfo.InvariantCulture);
            }).GetEnumerator();
            while (scanner.MoveNext()) {
                string type = (string)scanner.Current(typeof(string));
                scanner.MoveNext();
                float x = (float)scanner.Current(typeof(float));
                scanner.MoveNext();
                float y = (float)scanner.Current(typeof(float));

                if (type == "shop") {
                    scanner.MoveNext();
                    string item1 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item2 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item3 = (string)scanner.Current(typeof(string));
                    blocks.Add(new ShopPlacementInfo(x, y, item1, item2, item3));
                } else if (type == "player") {
                    blocks.Add(new PlayerPlacementInfo(x, y));
                } else if (type == "portal") {
                    blocks.Add(new PortalPlacementInfo(x, y));
                } else if (type == "rope") {
                    scanner.MoveNext();
                    int length = (int)scanner.Current(typeof(int));
                    blocks.Add(new RopePlacementInfo(x, y, length));
                } else {
                    blocks.Add(new BlockPlacementInfo(type, x, y));
                }
                
            }
            if (levels == null) levels = new Dictionary<string, Level>();
            levels[name] = this;

            scanner.Dispose();
        }

    }
}
