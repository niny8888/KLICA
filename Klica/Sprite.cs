using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public class Sprite
    {
        private Texture2D _texture;
        public Vector2 _position;
        private Rectangle _sourceRectangle;
        public Rectangle _size;
        private float _scale;
        public float _rotation;
        public Vector2 _origin;
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
            _size= new Rectangle(0,0, (int)(_sourceRectangle.Width * scale), (int)(_sourceRectangle.Height*scale));//to je treba porihtat ker pol k dam shit gor ne gre_size= new Rectangle(0,0,_sourceRectangle.Width * scale, _sourceRectangle.Height*scale);

        } // dodej layer depth

        
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

        // public void SetPosition(){
        //     //spremen se oregin
        // }
        
        
        //getters:
        public Texture2D Texture => _texture;
        public Vector2 Position => _position;
        public Rectangle SourceRectangle => _sourceRectangle;
        public float Scale => _scale;
        public float Rotation => _rotation;
        public Vector2 Origin => _origin;
        public Color Tint => _tint;
    }
}
