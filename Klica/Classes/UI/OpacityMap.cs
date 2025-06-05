using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Klica.Classes.Managers
{
    public class OpacityMap
    {
        private Texture2D _texture;
        private Color[] _pixelData;
        private int _width;
        private int _height;

        public OpacityMap(Texture2D texture)
        {
            _texture = texture;
            _width = _texture.Width;
            _height = _texture.Height;
            _pixelData = new Color[_width * _height];
            _texture.GetData(_pixelData);
        }

        // Returns brightness at screen-space position (0 to 1)
        public float GetBrightness(Vector2 screenPosition, int screenWidth, int screenHeight)
        {
            int x = (int)(screenPosition.X / screenWidth * _width);
            int y = (int)(screenPosition.Y / screenHeight * _height);

            x = MathHelper.Clamp(x, 0, _width - 1);
            y = MathHelper.Clamp(y, 0, _height - 1);

            Color pixel = _pixelData[y * _width + x];
            return pixel.R / 255f; // since grayscale: R=G=B
        }

        public bool IsInDangerZone(Vector2 screenPosition, int screenWidth, int screenHeight, float threshold = 0.8f)
        {
            return GetBrightness(screenPosition, screenWidth, screenHeight) >= threshold;
        }
    }
}
///Add fields:
/// private OpacityMap _dangerZoneMap;
/// 
/// In LoadContent:
/// var dangerTexture = content.Load<Texture2D>("danger_map1"); // or whatever name
///_dangerZoneMap = new OpacityMap(dangerTexture);
/// 
/// in Update
/// if (_dangerZoneMap.IsInDangerZone(_player._position, Game1.ScreenWidth, Game1.ScreenHeight))
// {
//     _player.TakeDamage(1);
// }



