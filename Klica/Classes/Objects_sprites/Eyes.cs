using System;
using Klica;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites{
    public class Eyes{
        private SpriteManager _spriteManager = SpriteManager.getInstance();
        private Sprite _eyes_white;
        private Sprite _eyes_version1;
        private  Sprite _eyes_version2;
        private  Sprite _eyes_version3;

        public Sprite _currentEye;

    
        public Eyes(int version){
            _eyes_white= _spriteManager.GetSprite("ucki_zun");
            _eyes_version1= _spriteManager.GetSprite("ucki_not1");
            _eyes_version2= _spriteManager.GetSprite("ucki_not2");
            _eyes_version3= _spriteManager.GetSprite("ucki_not3");

            SetSpriteEye(version);
        }
        
        public void SetSpriteEye(int spriteIndex)
        {
            _currentEye = spriteIndex switch
            {
                0 => _eyes_version1,
                1 => _eyes_version2,
                2 => _eyes_version3,
                _ => throw new ArgumentException("Invalid sprite index")
            };
        }
        internal void SetPosition(Vector2 vector2)
        {
            _currentEye._position = vector2;
        }

        internal void SetRotation(float rotation_new){
            _currentEye._rotation = rotation_new;
        }
        

        public void Draw(SpriteBatch _spriteBatch){
            _currentEye.Draw(_spriteBatch);
        }

    }   
}