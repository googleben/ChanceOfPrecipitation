using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    [Flags]
    public enum Collision
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public interface IEntity
    {
        float Health();
        void Damage(float amount);
    }

    public abstract class GameObject
    {
        public bool ToDestroy { get; private set; }

        public void Destroy()
        {
            ToDestroy = true;
        }

        public abstract void Update(EventList<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }

    /// <summary>
    /// Handles the collision with an ICollider
    /// </summary>
    public interface ICollidable
    {
        void Collide(Collision side, float amount, ICollider origin);

        RectangleF Bounds();

        Direction Facing();
    }

    /// <summary>
    /// Checks if the collision is happening
    /// </summary>
    public interface ICollider
    {
        void Collide(ICollidable c);

        RectangleF Bounds();
    }

    public class TextureInfo
    {
        public float scale;
        public string texName;
        public Rectangle src;
        public int Frames { get; set; } = 1;
        public int Delay { get; set; } = 10;

        public TextureInfo(string texName, Rectangle src, float scale = 1)
        {
            this.src = src;
            this.texName = texName;
            this.scale = scale;
        }
    }

    public interface IValuable
    {
        /// <summary>
        /// The value of the <see cref="IValuable"/>.
        /// </summary>
        /// <returns>The number of coins dropped by the <see cref="IValuable"/>.</returns>
        int Value();

        void DropCoins();
    }

    public class EventList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item)
        {
            if (OnAdd != null) {
                OnAdd(item, null);
            }
            base.Add(item);
        }
    }
}