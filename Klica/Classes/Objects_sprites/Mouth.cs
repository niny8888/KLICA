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

        public Vector2 _position=new Vector2(0,0); // Center position of the mouth
        private float _rotationAngle; // Current rotation angle for animation
        private float _rotationSpeed = 0.05f; // Speed of rotation during opening/closing
        private bool _isOpening = false; // Whether the mouth is opening
        private float _openThreshold = 10f; // Distance to trigger opening
        Boolean isSingular=false;

        public Mouth(int type)
        {
            SetMouth(type);
            if(type==2)
                isSingular=true;
            
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
                    // Handle invalid mouthType if necessary
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
                    foodConsumed = true; // Mark food as consumed
                    _isOpening = false; // Close mouth after consuming
                }
            }
            else
            {
                _isOpening = false; // Close mouth when food is far
            }

            // Animate mouth opening/closing
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

            if(isSingular){
                _oneMouth._position = position;
                return;
            }else{
                Vector2 leftBaseOffset = CalculateOffset(-separation,_rotationAngle,directionX,directionY);  // Left offset along the X-axis
                Vector2 rightBaseOffset = CalculateOffset(separation,_rotationAngle,directionX,directionY);  // Right offset along the X-axis
                //System.Console.WriteLine("Left offset: "+ leftBaseOffset);
                //System.Console.WriteLine("Right offset: "+ rightBaseOffset);

                Vector2 leftOffset = RotateVector(leftBaseOffset, _rotationAngle);
                Vector2 rightOffset = RotateVector(rightBaseOffset, _rotationAngle);
               
                _leftMouth._position = position + leftOffset;
                _rightMouth._position = position + rightOffset;
            }
            
            
        }

        private Vector2 CalculateOffset(float separation, float rotation, float directionX, float directionY)
        {
            float direction = MathHelper.ToDegrees(MathF.Atan2(directionY, directionX));  //iz X, Y v smeri
            //System.Console.WriteLine("Direction: "+ direction);
            
            float xOffset = 0f;
            float yOffset = 0f;



            //Right -->
            if (direction == 0 || direction == 360)
            {
                yOffset = separation;
            }
            //DOWNNN
            else if (direction==90)
            {
                xOffset = -separation;
            }
            //LEFT
            else if (direction==180)
            {
                yOffset = -separation;
            }
            //UP
            else if (direction==-90)
            {
                xOffset = separation;
            }

            ///Problem z rotit+ranjem vrce
            else if (direction > 0 && direction < 90)
            {
                // Facing right (0 degrees) -> left and right are along the Y-axis
                yOffset = separation;  // Up or down
                xOffset = separation;
            }
            else if (direction > 90 && direction < 180)
            {
                // Facing down (90 degrees) -> left and right are along the X-axis (inverted)
                xOffset = separation;  // Left or right
            }
            else if (direction > 180 && direction < 270)
            {
                // Facing left (180 degrees) -> left and right are along the Y-axis (inverted)
                yOffset = -separation;  // Up or down
            }
            else if (direction > 270 && direction < 360)
            {
                // Facing up (270 degrees) -> left and right are along the X-axis
                xOffset = -separation;  // Left or right
            }




            // Apply the offsets to the vector
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






        public void SetRotation(float rotation)
        {
            _leftMouth._rotation = rotation;
            _rightMouth._rotation = rotation;
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // System.Console.WriteLine("Mouth created");
            // System.Console.WriteLine("Left Mouth pos:"+ _leftMouth._position);
            // System.Console.WriteLine("Right Mouth pos:"+ _rightMouth._position);
            // System.Console.WriteLine("Mouth D rotation: "+ _rightMouth._rotation);
            // System.Console.WriteLine("Mouth L rotation: "+ _leftMouth._rotation);
            // System.Console.WriteLine("Mouth pos:"+ _position);
            // System.Console.WriteLine("Mouth rotation:"+ _rotationAngle);
            // System.Console.WriteLine("Mouth Left oregin:"+ _leftMouth._origin);
            // System.Console.WriteLine("Mouth Right oregin:"+ _rightMouth._origin);
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

    }
}
