using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ChanceOfPrecipitation {
    public class Settings {

        [XmlAttribute()]
        public int screenWidth = 1280;

        [XmlAttribute()]
        public int screenHeight = 720;

        [XmlAttribute()]
        public bool fullscreen = false;

    }
}
