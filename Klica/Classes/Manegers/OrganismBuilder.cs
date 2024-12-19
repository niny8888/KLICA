using Klica.Classes;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Klica.Classes.Objects_sprites;

namespace Klica{
    public class OrganismBuilder{
        private Base _base;
        private Eyes _eye;
        private Sprite _mouthL;
        private Sprite _mouthD;

        private Vector2 _origin; 
        private Vector2 _offsetEye;
        private Vector2 _offsetMouthL;
        private Vector2 _offsetMouthD;
        
        private float _rotation;
        private float _scale;
        private Vector2 _position;

       public OrganismBuilder(Base baseSprite, Eyes eye, Sprite mouthL, Sprite mouthD)
        {
            _base = baseSprite;
            _eye = eye;
            _mouthL = mouthL;
            _mouthD = mouthD;

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