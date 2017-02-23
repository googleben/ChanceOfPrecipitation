using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    class FloatingIndicator : GameObject
    {
        private Vector2 position;
        private int size;
        private float center;
        private float upSpeed;
        private float oscillationDist;
        private Timer oscillationPeriod;
        private Timer life;

        private Color color;

        public FloatingIndicator(Vector2 position, int size, float upSpeed, float oscillationDist, float oscillationPeriod, float life)
        {
            this.position = position;
            this.size = size;
            this.upSpeed = upSpeed;
            this.oscillationDist = oscillationDist;
            this.oscillationPeriod = new Timer(oscillationPeriod);
            this.life = new Timer(life);

            center = position.X;
        }

        public override void Update(IEnumerable<GameObject> objects)
        {
            oscillationPeriod.Update();
            life.Update();

            position.Y -= upSpeed;


        }

        public override void Draw(SpriteBatch sb)
        {
            
        }
    }
}
