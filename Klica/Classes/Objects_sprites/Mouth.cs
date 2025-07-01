using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Objects_sprites
{
    public class Mouth
    {
        private SpriteManager _spriteManager = SpriteManager.getInstance();
        private Sprite _leftMouth;
        private Sprite _rightMouth;
        private Sprite _oneMouth;

        public Vector2 _position = new Vector2(0, 0);
        private float _rotationAngle;
        private float _rotationSpeed = 0.05f; // Speed of mouth opening/closing
        private bool _isOpening = false;
        private float _openThreshold = 100f;
        Boolean isSingular = false;

        public Mouth(int type)
        {
            SetMouth(type);
            if (type == 2)
                isSingular = true;

        }
        public void Update()
        {
            // animacija odpirat zapirat usta
            if (_isOpening && _rotationAngle < 0.5f)
            {
                _rotationAngle += _rotationSpeed;
            }
            else if (!_isOpening && _rotationAngle > 0f)
            {
                _rotationAngle -= _rotationSpeed;
            }
        }

        public void SetMouth(int mouthType)
        {
            switch (mouthType)
            {
                case 0:
                    _leftMouth = _spriteManager.GetSprite("ustaL").Clone();
                    _rightMouth = _spriteManager.GetSprite("ustaD").Clone();
                    break;
                case 1:
                    _leftMouth = _spriteManager.GetSprite("ustaL2").Clone();
                    _rightMouth = _spriteManager.GetSprite("ustaD2").Clone();
                    break;
                case 2:
                    _oneMouth = _spriteManager.GetSprite("usta3").Clone();
                    break;
                default:
                    Console.WriteLine("Invalid mouth type");
                    break;
            }
        }


        public void CheckFoodCollisions(Vector2 foodPosition, float collisionRadius, ref bool foodConsumed)
        {
            float distanceToFood = Vector2.Distance(_position, foodPosition);

            if (!foodConsumed && distanceToFood <= _openThreshold)
            {
                _isOpening = true;
                if (distanceToFood <= collisionRadius)
                {
                    foodConsumed = true;
                    _isOpening = false;
                }
            }
            else
            {
                _isOpening = false;
            }

            // to je da se odprejo k je hrana bliz
            if (_isOpening && _rotationAngle < 0.5f)
            {
                _rotationAngle += _rotationSpeed;
            }
            else if (!_isOpening && _rotationAngle > 0f)
            {
                _rotationAngle -= _rotationSpeed;
            }
        }




        public void SetPosition(Vector2 position, float directionX, float directionY)
        {
            float separation = 1f;
            _position = position;

            if (isSingular)
            {
                _oneMouth._position = position;
                return;
            }
            else
            {
                Vector2 leftBaseOffset = CalculateOffset(-separation, _rotationAngle, directionX, directionY);
                Vector2 rightBaseOffset = CalculateOffset(separation, _rotationAngle, directionX, directionY);

                Vector2 leftOffset = RotateVector(leftBaseOffset, _rotationAngle);
                Vector2 rightOffset = RotateVector(rightBaseOffset, _rotationAngle);

                _leftMouth._position = position + leftOffset;
                _rightMouth._position = position + rightOffset;
            }


        }

        private Vector2 CalculateOffset(float separation, float rotation, float directionX, float directionY)
        {
            float direction = MathHelper.ToDegrees(MathF.Atan2(directionY, directionX));  //iz X, Y v smeri

            float xOffset = 0f;
            float yOffset = 0f;

            if (direction == 0 || direction == 360)
            {
                yOffset = separation;
            }
            //DOWNNN
            else if (direction == 90)
            {
                xOffset = -separation;
            }
            //LEFT
            else if (direction == 180)
            {
                yOffset = -separation;
            }
            //UP
            else if (direction == -90)
            {
                xOffset = separation;
            }

            ///Problem z rotit+ranjem vrce
            else if (direction > 0 && direction < 90)
            {
                yOffset = separation;
                xOffset = separation;
            }
            else if (direction > 90 && direction < 180)
            {
                xOffset = separation;
            }
            else if (direction > 180 && direction < 270)
            {
                yOffset = -separation;
            }
            else if (direction > 270 && direction < 360)
            {
                xOffset = -separation;
            }
            return new Vector2(xOffset, yOffset);
        }

        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector2(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos
            );
        }


        public void SetRotation(float baseRotation)
        {
            if (isSingular)
            {
                _oneMouth._rotation = baseRotation + _rotationAngle;
            }
            else
            {
                _leftMouth._rotation = baseRotation + _rotationAngle;
                _rightMouth._rotation = baseRotation - _rotationAngle;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (isSingular)
            {
                _oneMouth.Draw(spriteBatch);
            }
            else
            {
                _leftMouth.Draw(spriteBatch);
                _rightMouth.Draw(spriteBatch);
            }
        }
        public void Draw(SpriteBatch _spriteBatch, Color overrideTint)
        {
            if (isSingular)
            {
                _oneMouth.Draw(_spriteBatch, overrideTint);
            }
            else
            {
                _leftMouth.Draw(_spriteBatch, overrideTint);
                _rightMouth.Draw(_spriteBatch, overrideTint);
            }
        }
        public void Open()
        {
            _isOpening = true;

        }

        public void Close()
        {
            _isOpening = false;
        }

        public void DebugLogs()
        {
            System.Console.WriteLine("Mouth created");
            System.Console.WriteLine("Left Mouth pos:"+ _leftMouth._position);
            System.Console.WriteLine("Right Mouth pos:"+ _rightMouth._position);
            System.Console.WriteLine("Mouth D rotation: "+ _rightMouth._rotation);
            System.Console.WriteLine("Mouth L rotation: "+ _leftMouth._rotation);
            System.Console.WriteLine("Mouth pos:"+ _position);
            System.Console.WriteLine("Mouth rotation:"+ _rotationAngle);
            System.Console.WriteLine("Mouth Left oregin:"+ _leftMouth._origin);
            System.Console.WriteLine("Mouth Right oregin:"+ _rightMouth._origin);
        }


    }
}
