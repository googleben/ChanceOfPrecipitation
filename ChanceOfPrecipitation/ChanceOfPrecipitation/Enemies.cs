using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChanceOfPrecipitation {
    class Enemies {

        public static EnemyBuilder enemy1 = new EnemyBuilder(0, 0, 16, 32) { MaxHealth = 100 };
    }
}
