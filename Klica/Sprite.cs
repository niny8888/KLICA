using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public class Sprite
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _sourceRectangle;
        private float _scale;
        private float _rotation;
        private Color _tint;

        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRectangle, float scale = 1f, float rotation = 0f, Color? tint = null)
        {
            _texture = texture;
            _position = position;
            _sourceRectangle = sourceRectangle;
            _scale = scale;
            _rotation = rotation;
            _tint = tint ?? Color.White; // Default to white if tint is not provided
        } // dodej layer depth

        // Draw method to render the sprite
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                _position,
                _sourceRectangle,
                _tint,
                _rotation,
                new Vector2(_sourceRectangle.Width / 2, _sourceRectangle.Height / 2), // Adjust the origin as needed
                _scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
