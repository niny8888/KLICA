using System;
using Klica;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites{
    public class Base
    {
        private static Sprite _base_sprite_blue= SpriteManager.GetSprite("base_blue");
        private static Sprite _base_sprite_orange= SpriteManager.GetSprite("base_orange");
        private static Sprite _base_sprite_green= SpriteManager.GetSprite("base_green");
        private static Sprite _base_sprite_pink= SpriteManager.GetSprite("base_pink");

        private static bool Rotatable = true;
        private static bool Composable= true;
        private static bool Movable = true;
        private static bool Colidable= true;  //colision shape --> sphere !! TODO


        public static Sprite _currentSprite = _base_sprite_blue;
    


        public Base(int _spriteID){
            SetSprite(_spriteID);
        }

        public static void SetSprite(int spriteIndex)
        {
            _currentSprite = spriteIndex switch
            {
                0 => _base_sprite_blue,
                1 => _base_sprite_orange,
                2 => _base_sprite_green,
                3 => _base_sprite_pink,
                _ => throw new ArgumentException("Invalid sprite index")
            };
        }

        internal void SetPosition(Vector2 vector2)
        {
            _currentSprite._position = vector2;
        }

        internal Vector2 GetPosition()
        {
            return _currentSprite._position;
        }

        internal void SetRotation(float rotation_new){
            _currentSprite._rotation = rotation_new;
        }


    

        // public void Draw(SpriteBatch _spriteBatch){
        //     _currentSprite.Draw(_spriteBatch);
        // }

        //custop prorps: rotatable itd.

    }
}
