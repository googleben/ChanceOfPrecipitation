using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{

    [Flags]
    public enum Collision : int {
        None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8
    }

    public abstract class GameObject {
        public bool toDestroy { get; private set; }

        public void Destroy() {
            toDestroy = true;
        }

        public abstract void Update(IEnumerable<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }

    public interface Collidable {

        void Collide(Collision side, float amount);

        RectangleF Bounds();

    }

    public interface StaticObject {

        void Collide(Collidable c);

    }

}
