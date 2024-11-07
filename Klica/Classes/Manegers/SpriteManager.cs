using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public class SpriteManager
    {
        private Texture2D _spriteSheet;
        private Dictionary<string, Sprite> _sprites;

        public SpriteManager(Texture2D spriteSheet)
        {
            _spriteSheet = spriteSheet;
            _sprites = new Dictionary<string, Sprite>();
        }

        public void AddSprite(string name, Vector2 position, Rectangle sourceRectangle, float scale = 1f, float rotation = 0f, Color? tint = null)
        {
            var sprite = new Sprite(_spriteSheet, position, sourceRectangle, scale, rotation, tint);
            _sprites[name] = sprite;
        }

        public Sprite GetSprite(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name] : null;
        }

    
        public void DrawSpriteNamed(SpriteBatch spriteBatch, string name)
        {
            if (_sprites.TryGetValue(name, out Sprite sprite))
            {
                sprite.Draw(spriteBatch);
            }
        }

        public void DrawSprites(SpriteBatch spriteBatch)
        {
            foreach (var sprite in _sprites.Values)
            {
                sprite.Draw(spriteBatch);
            }
        }
    }
}
