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
        public Vector2 _position_mouth = new Vector2(0,0);
        private float _rotation_mouth = 0f;
        Vector2 organismCenter = new Vector2(0, 0);


        public Sprite _currentSprite;
    


        public Base(int _spriteID){
            _base_sprite_blue= _spriteManager.GetSprite("base_blue");
            _base_sprite_orange= _spriteManager.GetSprite("base_orange");
            _base_sprite_green= _spriteManager.GetSprite("base_green");
            _base_sprite_pink= _spriteManager.GetSprite("base_pink");

            SetSprite(_spriteID);
            UpdateComponentPositions();
            organismCenter = new Vector2(_currentSprite._size.Width / 2, _currentSprite._size.Height / 2);
        }
         private void UpdateComponentPositions()
        {
            SetEyePosition();
            SetMouthPosition();
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
            Vector2 offset = new Vector2(0,- _currentSprite._size.Height / 4);
            _position_eyes= CalculateRotatedPosition(_currentSprite._position, offset, _currentSprite._rotation);
           
        }
        internal void SetMouthPosition()
        {
            Vector2 offset = new Vector2(12,- _currentSprite._size.Height/2); // Adjust offset for visual alignment
            _position_mouth = CalculateRotatedPosition(_currentSprite._position, offset, _currentSprite._rotation);
        }

        private Vector2 CalculateRotatedPosition(Vector2 basePosition, Vector2 offset, float rotation)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);

            Vector2 rotatedOffset = new Vector2(
                offset.X * cos - offset.Y * sin,
                offset.X * sin + offset.Y * cos
            );

            return basePosition + rotatedOffset;
        }

        internal void SetPosition(Vector2 vector2)
        {
            _currentSprite._position = vector2;
            UpdateComponentPositions();
        }

        internal Vector2 GetPosition()
        {
            return _currentSprite._position;
        }

        internal void SetRotation(float rotation_new)
        {
            _currentSprite._rotation = rotation_new;
            UpdateComponentPositions();
        }
        
        public float GetRotation(){
            return _currentSprite._rotation;
        }

        internal Vector2 GetMouthPosition()
        {
            return _position_mouth;
        }

        internal float GetMouthRotation()
        {
            return _rotation_mouth;
        }

        public void Draw(SpriteBatch _spriteBatch){
            _currentSprite.Draw(_spriteBatch);
        }

        //custop prorps: rotatable itd.

    }
}
