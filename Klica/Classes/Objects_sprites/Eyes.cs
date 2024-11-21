using System;
using Klica;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites{
    public class Eyes{
        private static Sprite _eyes_white= SpriteManager.GetSprite("ucki_zun");
        private static Sprite _eyes_version1= SpriteManager.GetSprite("ucki_not1");
        private static Sprite _eyes_version2= SpriteManager.GetSprite("ucki_not2");
        private static Sprite _eyes_version3= SpriteManager.GetSprite("ucki_not3");

        public static Sprite _currentEye = _eyes_version1;
    
        public Eyes(int version){
            SetSpriteEye(version);
        }
        
        public static void SetSpriteEye(int spriteIndex)
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

    }   
}