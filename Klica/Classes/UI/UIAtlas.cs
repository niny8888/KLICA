using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Klica.Classes
{
    public class UIAtlas
    {
        private Texture2D _atlasTexture;

        // Sprite source rectangles (coordinates match your UI image layout)
        public Rectangle BlueBar { get; private set; }
        public Rectangle WhiteBar { get; private set; }
        public Rectangle BlueCircle { get; private set; }
        public Rectangle Button { get; private set; }
        public Rectangle GearIcon { get; private set; }
        public Rectangle DNAIconWhite { get; private set; }
        public Rectangle DNAIconBlue { get; private set; }

        public UIAtlas(Texture2D atlasTexture)
        {
            _atlasTexture = atlasTexture;

            // Define sprites by their position in the image (565x2048 assumed resolution)
            BlueBar        = new Rectangle(0,    0, 1200, 80);//poprav
            WhiteBar       = new Rectangle(0,  100, 1200, 80);//poprav
            Button         = new Rectangle(0,  200, 900, 200);//poprav
            GearIcon       = new Rectangle(1000,  250, 128, 128);//poprav
            BlueCircle     = new Rectangle(1300, 0, 128, 128);//poprav
            DNAIconWhite   = new Rectangle(3520, 100, 780, 390);
            DNAIconBlue    = new Rectangle(3520, 500, 780, 390);
        }

        public Texture2D GetTexture() => _atlasTexture;
    }
}
