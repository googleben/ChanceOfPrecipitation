﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{

    [Flags]
    public enum Collision : int {
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
        public bool toDestroy { get; private set; }

        public void Destroy() {
            toDestroy = true;
        }

        public abstract void Update(List<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }

    public interface ICollidable {

        void Collide(Collision side, float amount, IStaticObject origin);

        RectangleF Bounds();

        Direction Facing();

    }

    public interface IStaticObject {

        void Collide(ICollidable c);

    }

}
