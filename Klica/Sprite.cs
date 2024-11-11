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
        private Vector2 _origin;
        private Color _tint;

        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRectangle, float scale = 1f, float rotation = 0f, Vector2? origin = null, Color? tint = null)
        {
            _texture = texture;
            _position = position;
            _sourceRectangle = sourceRectangle;
            _scale = scale;
            _rotation = rotation;
            _origin = origin ?? new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2); // Default to center
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
                _origin,  
                _scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
