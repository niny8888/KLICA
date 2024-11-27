using System;
using Klica;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites{
    public class Base
    {
        private SpriteManager _spriteManager = SpriteManager.getInstance();

        private Sprite _base_sprite_blue;
        private Sprite _base_sprite_orange;
        private Sprite _base_sprite_green;
        private Sprite _base_sprite_pink;

        private  bool Rotatable = true;
        private bool Composable= true;
        private bool Movable = true;
        private  bool Colidable= true;  //colision shape --> sphere !! TODO

        public Vector2 _position_eyes =new Vector2(0,0);

        public Sprite _currentSprite;
    


        public Base(int _spriteID){
            _base_sprite_blue= _spriteManager.GetSprite("base_blue");
            _base_sprite_orange= _spriteManager.GetSprite("base_orange");
            _base_sprite_green= _spriteManager.GetSprite("base_green");
            _base_sprite_pink= _spriteManager.GetSprite("base_pink");

            SetSprite(_spriteID);
            SetEyePosition();
        }

        public void SetSprite(int spriteIndex)
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

        internal void SetEyePosition()
        {
            Vector2 offset = new Vector2(0, -_currentSprite.SourceRectangle.Height / 4);

            
            float cos = (float)Math.Cos(_currentSprite.Rotation);
            float sin = (float)Math.Sin(_currentSprite.Rotation);

            Vector2 rotatedOffset = new Vector2(
                offset.X * cos - offset.Y * sin,
                offset.X * sin + offset.Y * cos
            );

            
            _position_eyes = _currentSprite.Position + rotatedOffset;
        }



        internal void SetPosition(Vector2 vector2)
        {
            _currentSprite._position = vector2;
            SetEyePosition();
        }

        internal Vector2 GetPosition()
        {
            return _currentSprite._position;
        }

        internal void SetRotation(float rotation_new){
            _currentSprite._rotation = rotation_new;
        }


    

        public void Draw(SpriteBatch _spriteBatch){
            _currentSprite.Draw(_spriteBatch);
        }

        //custop prorps: rotatable itd.

    }
}
