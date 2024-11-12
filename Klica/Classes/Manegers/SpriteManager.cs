using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
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

        public void AddSprite(String name, Vector2 position, Rectangle sourceRectangle, float scale = 1f, float rotation = 0f, Vector2? origin = null, Color? tint = null)
        {
            var sprite = new Sprite(_spriteSheet, position, sourceRectangle, scale, rotation, origin,  tint);
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


        ///getters:

        public Vector2? GetPosition(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Position : (Vector2?)null;
        }

        public Rectangle? GetSourceRectangle(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].SourceRectangle : (Rectangle?)null;
        }

        public float? GetScale(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Scale : (float?)null;
        }

        public float? GetRotation(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Rotation : (float?)null;
        }

        public Vector2? GetOrigin(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Origin : (Vector2?)null;
        }

        public Color? GetTint(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Tint : (Color?)null;
        }


        
    }
}
