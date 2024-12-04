using System;
using System.Collections.Generic;
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
        private GameTime _gameTime;

        public Sprite _currentEye;
        public AnimatedSprite _currentEyeAnimation;
        public List<Sprite> _eyeFrames;

    
        public Eyes(int version){
            _eyeFrames= new List<Sprite>();
            _eyes_white= _spriteManager.GetSprite("ucki_zun");
            _eyes_version1= _spriteManager.GetSprite("ucki_not1");
            _eyes_version2= _spriteManager.GetSprite("ucki_not2");
            _eyes_version3= _spriteManager.GetSprite("ucki_not3");

            for (int i = 1; i <= 9; i++)
            {
                string spriteName = $"mezikanje_e{i}";
                _eyeFrames.Add(_spriteManager.GetSprite(spriteName)); // Add frames for animation
            }
            _currentEyeAnimation= new AnimatedSprite(_eyeFrames);
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
            _eyes_white._position= vector2;
            if (_currentEyeAnimation != null)
                _currentEyeAnimation._position = vector2;
        }

        internal void SetRotation(float rotation_new){
            _currentEye._rotation = rotation_new;
            _eyes_white._rotation = rotation_new;
            if (_currentEyeAnimation != null)
                _currentEyeAnimation._rotation = rotation_new;
        }
        

        public void Draw(SpriteBatch _spriteBatch){
            _eyes_white.Draw(_spriteBatch);
            _currentEye.Draw(_spriteBatch);
            _currentEyeAnimation.Update(_gameTime);
            _currentEyeAnimation?.Draw(_spriteBatch);
        }

        //base-position je za popravt size od sprita , da se * scale da ce se je scalu
        /// k se rotira se mu rab spremenit origin rotacije na oregin od base sprita(parenta)
    }   
}