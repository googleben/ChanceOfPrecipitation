using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    public class EnemyBuilder
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Width { get; set; } = 16;
        public float Height { get; set; } = 32;
        public float MaxSpeed { get; set; } = 2f;
        public float jumpSpeed { get; set; } = 10f;
        public float MaxHealth { get; set; } = 100;
        public Direction Facing { get; set; } = Direction.Right;

        public EnemyBuilder(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public EnemyBuilder()
        {

        }

        public Enemy Build(float x, float y)
        {
            X = x;
            Y = y;
            return new Enemy(this);
        }

        public Enemy Build()
        {
            return new Enemy(this);
        }
    }
}
