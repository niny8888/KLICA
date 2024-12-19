using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Objects_sprites
{
    public class Mouth
    {
        private Sprite _leftMouth;
        private Sprite _rightMouth;

        private Vector2 _position; // Center position of the mouth
        private float _rotationAngle; // Current rotation angle for animation
        private float _rotationSpeed = 0.05f; // Speed of rotation during opening/closing
        private bool _isOpening = false; // Whether the mouth is opening
        private float _openThreshold = 50f; // Distance to trigger opening

        public Mouth(Sprite leftMouth, Sprite rightMouth, Vector2 position)
        {
            _leftMouth = leftMouth;
            _rightMouth = rightMouth;
            _position = position;
        }

       public void Update(Vector2 playerPosition, bool isOpening)
        {
            _position = playerPosition;
            _isOpening = isOpening;

            // Animate mouth opening/closing
            if (_isOpening && _rotationAngle < 0.5f) // Open up to a certain angle
            {
                _rotationAngle += _rotationSpeed;
            }
            else if (!_isOpening && _rotationAngle > 0f) // Close if not opening
            {
                _rotationAngle -= _rotationSpeed;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw left and right parts of the mouth
            _leftMouth._position = _position; // Adjusted for left position
            _rightMouth._position = _position; // Adjusted for right position

            _leftMouth._rotation = -_rotationAngle; // Rotate left mouth counter-clockwise
            _rightMouth._rotation = _rotationAngle; // Rotate right mouth clockwise

            _leftMouth.Draw(spriteBatch);
            _rightMouth.Draw(spriteBatch);
        }
    }
}
