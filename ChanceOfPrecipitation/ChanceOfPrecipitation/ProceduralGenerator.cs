using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    class ProceduralGenerator
    {

        public static List<PBlock> bases;
        public static List<PBlock> additions;

        public ProceduralGenerator()
        {
            bases = new List<PBlock>();
            additions = new List<PBlock>();
            bases.Add(new PBlock(
                new List<Exit>() {
                    new Exit(-32, 32, false),
                    new Exit(96, 32, false)
                }, 
                new List<IPlacementInfo>() {
                    new BlockPlacementInfo("stage1_platform_top_middle", 0, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 32, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 64, 0),
                    new BlockPlacementInfo("stage1_platform_top_middle", 96, 0)
                }
            ));
            bases.Add(new PBlock(
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
            ));
        }

        public List<IPlacementInfo> GenLevel()
        {
            PLevel ans = new PLevel();
            for (int i = 0; i < 10; i++) ans.GenBase();
            return ans.Build();
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


            }
            ans.Add(next);
            blocks.Add(ans);
        }

        void GenPiece(List<PBlock> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                var b = l[i];
                List<Exit> exits = b.exits;
                if (i == 0) exits = exits.Where(e => e.vertical).ToList();
                int tries = 0;
                bool done = false;
                while (tries++ < 10 && !done)
                {
                    Exit s = ChooseRandom(exits);

                }
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
