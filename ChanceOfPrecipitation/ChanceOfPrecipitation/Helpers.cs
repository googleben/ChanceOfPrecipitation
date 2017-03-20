using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{

    [Flags]
    public enum Collision {
        None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8
    }

    public enum Direction {
        Left, Right, Up, Down
    }

    public interface IEntity {
        float Health();
        void Damage(float amount);
    }

    public abstract class GameObject {
        public bool ToDestroy { get; private set; }

        public void Destroy() {
            ToDestroy = true;
        }

        public abstract void Update(List<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }

    public interface ICollidable {

        void Collide(Collision side, float amount, ICollider origin);

        RectangleF Bounds();

        Direction Facing();

    }

    public interface ICollider {

        void Collide(ICollidable c);

    }

    public class BlockInfo {

        public float scale;
        public string texName;
        public Rectangle src;

        public BlockInfo(string texName, Rectangle src) : this(texName, src, 1) {}
        public BlockInfo(string texName, Rectangle src, float scale)  {
            this.src = src;
            this.texName = texName;
            this.scale = scale;
        }

    }
}
