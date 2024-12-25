using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public class SpriteManager
    {
        private static Texture2D _spriteSheet;
        private  Dictionary<string, Sprite> _sprites;
        private List<Sprite> _activeSprites;
        private static volatile SpriteManager instance= null;
        public float AdjustedRotation ;


        public SpriteManager(Texture2D spriteSheet)
        {
            _spriteSheet = spriteSheet;
            _sprites = new Dictionary<string, Sprite>();
            _activeSprites = new List<Sprite>();
            instance= this;
        }
        public static SpriteManager getInstance(){
            if(instance==null){
                instance=new SpriteManager(_spriteSheet);
            }
            return instance;
        }

        public void AddSprite(String name, Vector2 position, Rectangle sourceRectangle,int _rotatedSheet, float scale = 1f,  float rotation = 0f, Vector2? origin = null, Color? tint = null)
        {
            AdjustedRotation = _rotatedSheet == 1 ? rotation - 1.6f : rotation;
            var sprite = new Sprite(_spriteSheet, position, sourceRectangle, _rotatedSheet ,scale,AdjustedRotation , origin,  tint);
            _sprites[name] = sprite;
            ActivateSprite(name, position);
        }
        public void AddSprite(Sprite sprite, String name)
        {
            _sprites[name] = sprite;
            ActivateSprite(name, sprite.Position);
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
        public void ActivateSprite(string name, Vector2 position)
        {
            if (_sprites.ContainsKey(name))
            {
                var originalSprite = _sprites[name];
                var activeSprite = new Sprite(
                    originalSprite.Texture,
                    position,
                    originalSprite.SourceRectangle,
                    originalSprite.RotatedSheet,
                    originalSprite.Scale,
                    originalSprite.Rotation,
                    originalSprite.Origin,
                    originalSprite.Tint
                );
                System.Console.WriteLine("Active sprite: " + activeSprite);
                _activeSprites.Add(activeSprite);
            }
        }
        public void DeactivateSprite(Sprite sprite)
        {
            _activeSprites.Remove(sprite);
        }
        public void DeactivateSprite(string name)
        {
            _activeSprites.RemoveAll(s => _sprites.ContainsKey(name) && s.Texture == _sprites[name].Texture);
        }
         public void DrawActiveSprites(SpriteBatch spriteBatch)
        {
            foreach (var sprite in _activeSprites)
            {
                sprite.Draw(spriteBatch);
            }
        }
        public IEnumerable<Sprite> GetActiveSprites() => _activeSprites;


        // public void DrawComposable(Organizm o){

        // }

        public void DrawPart(){
            
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
            return _sprites.ContainsKey(name) ? _sprites[name].AdjustedRotation : (float?)null;
        }

        public Vector2? GetOrigin(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Origin : (Vector2?)null;
        }

        public Color? GetTint(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name].Tint : (Color?)null;
        }

        public int? GetRotatedSheet(string name)
        {
            return _sprites.ContainsKey(name) ? _sprites[name]._rotatedSheet : (int?)null;
        }


        
    }
}
