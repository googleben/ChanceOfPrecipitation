using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{

    class BlockPlacementInfo {
        public string type;
        public Vector2 position;

        public BlockPlacementInfo(string type, float x, float y) {
            this.type = type;
            this.position = new Vector2(x, y);
        }
    }

    class Level {

        public static Dictionary<string, Level> levels;

        public List<BlockPlacementInfo> blocks;

        public Level(string raw, string name) {
            this.blocks = new List<BlockPlacementInfo>();
            var split = Regex.Split(raw, "\\s+");
            var scanner = split.Select<string, Func<Type, object>>((string s) => {
                return t =>
                (s as IConvertible).ToType(t, System.Globalization.CultureInfo.InvariantCulture);
            }).GetEnumerator();
            while (scanner.MoveNext()) {
                string type = (string)scanner.Current(typeof(string));
                scanner.MoveNext();
                float x = (float) scanner.Current(typeof(float));
                scanner.MoveNext();
                float y = (float)scanner.Current(typeof(float));
                blocks.Add(new BlockPlacementInfo(type, x, y));
            }
            if (levels==null) levels = new Dictionary<string, Level>();
            levels[name] = this;

            scanner.Dispose();
        }

    }
}
