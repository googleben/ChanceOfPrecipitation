using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace ChanceOfPrecipitation
{
    class ProceduralGenerator
    {

        public static List<PBlock> bases;
        public static List<PBlock> additions;
        Random rand;

        public ProceduralGenerator()
        {
            rand = new Random();
            bases = new List<PBlock>();
            additions = new List<PBlock>();

            //bases.Add(LoadBlock(File.ReadAllText("Content/Levels/base1.txt")));
            //additions.Add(LoadBlock(File.ReadAllText("Content/Levels/addition1.txt")));

            foreach (var s in Directory.EnumerateFiles("Content/Levels/")) {
                if (s.Substring("Content/Levels/".Length).StartsWith("base")) {
                    bases.Add(LoadBlock(File.ReadAllText(s)));
                }
                if (s.Substring("Content/Levels/".Length).StartsWith("addition")) {
                    additions.Add(LoadBlock(File.ReadAllText(s)));
                }
            }

            /*bases.Add(new PBlock(
                new List<Exit>() {
                    new Exit(-32, -32, false),
                    new Exit(64, -32, true),
                    new Exit(256, -32, false)
                }, 
                new List<IPlacementInfo>() {
                    new BlockPlacementInfo("stage1_platform_top_middle", 0, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 32, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 64, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 96, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 128, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 160, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 192, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 224, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 256, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 64, -32)
                }
            ));
            additions.Add(new PBlock(
                new List<Exit>()
                {
                    new Exit(0, 0, true)
                },
                new List<IPlacementInfo>()
                {
                    new BlockPlacementInfo("stage1_platform_top_middle", 0, -32)
                }
            ));*/
            /*bases.Add(new PBlock(
                new List<Exit>() {
                    new Exit(-32, 32, false),
                    new Exit(96, 96, false)
                },
                new List<IPlacementInfo>() {
                    new BlockPlacementInfo("stage1_platform_top_middle", 0, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 32, 32),
                    new BlockPlacementInfo("stage1_platform_top_middle", 64, 64),
                    new BlockPlacementInfo("stage1_platform_top_middle", 96, 64)
                }
            ));*/
        }

        public List<IPlacementInfo> GenLevel()
        {
            PLevel ans = new PLevel();
            for (int i = 0; i < 2; i++) ans.GenBase();
            for (int i = 0; rand.Next(i) < 5; i++) ans.GenPiece();
            return ans.Build();
        }

        public PBlock LoadBlock(string raw)
        {
            var blocks = new List<IPlacementInfo>();
            var exits = new List<Exit>();
            var split = Regex.Split(raw, "\\s+");
            var scanner = split.Select<string, Func<Type, object>>((string s) => {
                return t =>
                (s as IConvertible).ToType(t, System.Globalization.CultureInfo.InvariantCulture);
            }).GetEnumerator();
            while (scanner.MoveNext())
            {
                string type = (string)scanner.Current(typeof(string));
                scanner.MoveNext();
                float x = (float)scanner.Current(typeof(float));
                scanner.MoveNext();
                float y = (float)scanner.Current(typeof(float));

                if (type == "shop")
                {
                    scanner.MoveNext();
                    string item1 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item2 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item3 = (string)scanner.Current(typeof(string));
                    blocks.Add(new ShopPlacementInfo(x, y, item1, item2, item3));
                }
                else if (type == "player")
                {
                    blocks.Add(new PlayerPlacementInfo(x, y));
                }
                else if (type == "portal")
                {
                    blocks.Add(new PortalPlacementInfo(x, y));
                }
                else if (type == "rope")
                {
                    scanner.MoveNext();
                    int length = (int)scanner.Current(typeof(int));
                    blocks.Add(new RopePlacementInfo(x, y, length));
                }
                else if (type=="exit")
                {
                    scanner.MoveNext();
                    bool vertical = (bool)scanner.Current(typeof(bool));
                    exits.Add(new Exit(x, y, vertical));
                }
                else
                {
                    blocks.Add(new BlockPlacementInfo(type, x, y));
                }

            }
            scanner.Dispose();
            return new PBlock(exits, blocks);
        }

    }

    class PLevel
    {
        List<List<PBlock>> blocks;
        Random rand;
        List<PBlock> allBlocks => blocks.SelectMany(x => x).ToList();

        public PLevel()
        {
            blocks = new List<List<PBlock>>();
            rand = new Random();
        }

        T ChooseRandom<T>(List<T> l)
        {
            return l[rand.Next(l.Count)];
        }

        public void GenBase()
        {
            List<PBlock> ans = new List<PBlock>();
            PBlock next = null;
            var all = allBlocks;
            int tries = 0;
            while (tries++<10 && (next==null || (all.Count==0 || all.Any(a => a.Intersects(next))))) next = (PBlock)ChooseRandom(ProceduralGenerator.bases).Clone();
            if (next == null) Console.WriteLine("null");
            if (blocks.Count!=0)
            {
                PBlock last = blocks.Last().First();
                Exit e = last.exits.Last();
                var amount = e.GetOffset(next.exits.First());
                Console.WriteLine(amount);
                next.Offset(amount);
                //last.Bind(next);
                last.exits.Remove(e);
                next.exits.RemoveAt(0);


            } else {
                next.Offset(new Exit(0, 0, false).GetOffset(next.exits.First()));
            }
            ans.Add(next);
            blocks.Add(ans);
        }

        public void GenPiece()
        {
            Console.WriteLine("piece");
            int count = allBlocks.Count;
            for (int i = 0; i<10 && count==allBlocks.Count; i++)
            {
                GenPiece(ChooseRandom(blocks));
            } 
        }

        public void GenPiece(List<PBlock> l)
        {
            int mainInd = rand.Next(l.Count-1);
            for (int i = mainInd+1; true; i++)
            {
                if (i >= l.Count) i = 0;
                if (i == mainInd && l.Count!=1) break;
                var b = l[i%l.Count];
                List<Exit> exits = b.exits;
                if (i == 0) exits = exits.Where(e => e.vertical).ToList();
                if (exits.Count == 0)
                {
                    if (l.Count == 1) break;
                    continue;
                }
                int tries = 0;
                bool done = false;
                while (tries++ < 10 && !done)
                {
                    Exit s = ChooseRandom(exits);
                    List<PBlock> choices = ProceduralGenerator.additions.Where(x => (s.vertical ? x.HasVertical : x.HasHorizontal)).ToList();
                    int startInd = rand.Next(choices.Count);
                    PBlock ans = choices[startInd];
                    for (int ind = startInd+1; ind!=startInd && !done; ind++)
                    {
                        if (ind >= choices.Count) ind = 0;
                        foreach (Exit ex in ans.exits.Where(x => x.vertical==s.vertical))
                        {
                            if (done) break;
                            PBlock cop = (PBlock)ans.Clone();
                            var offset = s.GetOffset(ex);
                            cop.Offset(offset);
                            if (!allBlocks.Any(pb => pb.Intersects(cop)))
                            {
                                Console.WriteLine("add");
                                done = true;
                                l.Add(cop);
                                cop.exits.Remove(ex + offset);
                                b.exits.Remove(s);
                            }
                        }
                        ans = choices[ind];
                        if (choices.Count == 1) break;
                    }
                }
                if (done) break;
                if (l.Count == 1) break;
            }
        }

        public List<IPlacementInfo> Build()
        {
            return allBlocks.ConvertAll(pb => pb.placement).SelectMany(a => a).ToList();
        }

    }

    struct Exit
    {

        public float x;
        public float y;
        public bool vertical;

        public Exit(float x, float y, bool vertical)
        {
            this.x = x;
            this.y = y;
            this.vertical = vertical;
        }

        public Vector2 GetOffset(Exit b)
        {
            return new Vector2(x - b.x, y - b.y);
        }

        public static Exit operator+(Exit a, Vector2 amount)
        {
            return new Exit(a.x + amount.X, a.y + amount.Y, a.vertical);
        }

    }

    class PBlock : ICloneable
    {

        public RectangleF bounds;
        public List<Exit> exits;
        public List<IPlacementInfo> placement;

        public bool HasVertical => exits.Exists(e => e.vertical);
        public bool HasHorizontal => exits.Exists(e => !e.vertical);

        public PBlock(List<Exit> exits, List<IPlacementInfo> placement)
        {
            this.exits = new List<Exit>(exits);
            this.placement = placement.ConvertAll(a => (IPlacementInfo)a.Clone());
            CalcBounds();
        }

        void CalcBounds()
        {
            var b = placement.ConvertAll(p => p.Bounds());
            float minX = b.ConvertAll(r => r.x).Min();
            float maxX = b.ConvertAll(r => r.x + r.width).Min();
            float minY = b.ConvertAll(r => r.y).Min();
            float maxY = b.ConvertAll(r => r.y + r.height).Min();
            this.bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public void Offset(Vector2 amount)
        {
            foreach (IPlacementInfo p in placement) p.Offset(amount);
            for (int i = 0; i < exits.Count; i++) exits[i] += amount;
            CalcBounds();
        }

        public object Clone()
        {
            PBlock ans = new PBlock(exits, placement);
            return ans;
        }

        public bool Intersects(PBlock other)
        {
            return bounds.Intersects(other.bounds);
        }

        public void Bind(PBlock other)
        {
            for (int i = 0; i<exits.Count; i++)
            {
                var x = exits[i];
                for (int j = 0; j<other.exits.Count; j++)
                {
                    if (x.GetOffset(other.exits[j])==Vector2.Zero)
                    {
                        exits.RemoveAt(i);
                        other.exits.RemoveAt(j);
                        i--;
                        goto next;
                    }
                }
                next:;
            }
        }

    }
}
