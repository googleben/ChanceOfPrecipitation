using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChanceOfPrecipitation
{
    public abstract class GameObject
    {
        public bool toDestroy { get; private set; }

        public void Destroy()
        {
            toDestroy = true;
        }

        public abstract void Update(IEnumerable<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }
}
