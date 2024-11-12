
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica{
    public class OrganismBuilder{
        private Sprite _base;
        private Sprite _eye;
        private Sprite _mouthL;
        private Sprite _mouthD;

        private Vector2 _origin; 
        private Vector2 _offsetEye;
        private Vector2 _offsetMouthL;
        private Vector2 _offsetMouthD;
        
        private float _rotation;
        private float _scale;
        private Vector2 _position;

       public OrganismBuilder(Sprite baseSprite, Sprite eye, Sprite mouthL, Sprite mouthD)
        {
            _base = baseSprite;
            _eye = eye;
            _mouthL = mouthL;
            _mouthD = mouthD;

            _origin = new Vector2(_base.SourceRectangle.Width / 2, _base.SourceRectangle.Height / 2);  // Central origin for rotation
            _offsetEye = new Vector2(0, -_base.SourceRectangle.Height / 4); // Adjust based on organism structure
            _offsetMouthL = new Vector2(-_base.SourceRectangle.Width / 4, _base.SourceRectangle.Height / 4);
            _offsetMouthD = new Vector2(_base.SourceRectangle.Width / 4, _base.SourceRectangle.Height / 4);

            _rotation = 0f;
            _scale = 1f;
            _position = Vector2.Zero;
        }
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetRotation(float rotation)
        {
            _rotation = rotation;
        }

        public void SetScale(float scale)
        {
            _scale = scale;
        }
        

    }
}