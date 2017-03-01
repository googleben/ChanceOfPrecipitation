using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    class Enemies {

        public static Enemy enemy1;

        static Enemies() {
            enemy1 = new Enemy(0, 0, 16, 32);
            enemy1.MaxHealth = enemy1.health = 100;
        }


    }
}
