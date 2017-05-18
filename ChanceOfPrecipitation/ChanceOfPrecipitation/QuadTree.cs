using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChanceOfPrecipitation {
    class QuadTree {

        public List<ICollidable> dynamics;
        public List<ICollider> statics;

        public QuadTree[] nodes;

        public RectangleF bounds;

        const int MAX_STATICS = 16;

        QuadTree parent;

        public QuadTree(float x, float y, float width, float height, List<ICollider> possibleStatics, QuadTree parent) {
            this.parent = parent;
            bounds = new RectangleF(x, y, width, height);
            statics = new List<ICollider>();
            dynamics = new List<ICollidable>();
            foreach (ICollider c in possibleStatics) {
                if (c.Bounds().Intersects(bounds)) statics.Add(c);
            }
            if (statics.Count>MAX_STATICS) {
                partition();
            }
        }

        public void partition() {
            var w = bounds.width / 2;
            var h = bounds.width / 2;
            var x = bounds.x;
            var y = bounds.y;
            nodes = new QuadTree[] {
                new QuadTree(x, y, w, h, statics, this),
                new QuadTree(x+w, y, w, h, statics, this),
                new QuadTree(x, y+h, w, h, statics, this),
                new QuadTree(x+w, y+h, w, h, statics, this)
            };
            statics = null;
            dynamics = null;
        }

        public void MoveDynamic(ICollidable c) {
            if (bounds.Contains(c.Bounds())) AddDynamic(c);
            else if (parent!=null) parent.MoveDynamic(c);
        }

        public void AddDynamic(ICollidable c) {
            if (c.Bounds().Intersects(bounds)) {
                if (nodes!=null) {
                    foreach (QuadTree q in nodes) q.AddDynamic(c);
                } else if (!dynamics.Contains(c)) {
                    dynamics.Add(c);
                    if (c is ICollider) statics.Add(c as ICollider);
                }
            }
        }

        public void AddStatic(ICollider c) {
            if (c.Bounds().Intersects(bounds)) {
                if (nodes != null) {
                    foreach (QuadTree q in nodes) q.AddStatic(c);
                }
                else if (!statics.Contains(c)) {
                    statics.Add(c);
                }
            }
        }

        public void CheckDynamics() {
            if (nodes == null) {
                for (int i = dynamics.Count - 1; i >= 0; i--) {
                    var d = dynamics[i];

                    if (d is GameObject && (d as GameObject).ToDestroy) {
                        dynamics.RemoveAt(i);
                        if (d is ICollider) statics.Remove(d as ICollider);
                        continue;
                    }

                    if (!bounds.Contains(d.Bounds())) {
                        dynamics.RemoveAt(i);
                        if (d is ICollider) statics.Remove(d as ICollider);
                        MoveDynamic(d);
                    }
                }
                for (int i = statics.Count-1; i>=0; i--) {
                    var s = statics[i];
                    if (s is GameObject && (s as GameObject).ToDestroy) {
                        statics.RemoveAt(i);
                    }
                }
            }
            else foreach (QuadTree q in nodes) q.CheckDynamics();
        }

        public void RunCollision() {
            if (nodes == null)
                for (int i = 0; i < statics.Count; i++)
                    for (int j = 0; j < dynamics.Count; j++) statics[i].Collide(dynamics[j]);
            else foreach (QuadTree q in nodes) q.RunCollision();
        }

        public void RunCollision(Func<ICollider, bool> pred) {
            if (nodes == null)
                foreach (var s in statics.Where(pred)) {
                    foreach (var c in dynamics) s.Collide(c);
                }
            else foreach (QuadTree q in nodes) q.RunCollision(pred);
        }

        public bool DoesCollide(RectangleF r)
        {
            foreach (var s in statics) if (s.Bounds().Intersects(r)) return true;
            return false;
        }

        public List<QuadTree> GetPos(RectangleF r)
        {
            if (r.Intersects(bounds))
            {
                List<QuadTree> ans = new List<QuadTree>();
                if (nodes == null) ans.Add(this);
                else
                {
                    foreach (var n in nodes) n.GetPos(r).ForEach(ans.Add);
                }
                return ans;
            }
            return new List<QuadTree>();
        }

    }
}
