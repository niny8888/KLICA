using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes
{
    public class Sprite
    {
        public Texture2D _texture;
        public Vector2 _position;
        public Rectangle _sourceRectangle;
        public Rectangle _size;
        public float _scale;
        public float _rotation;
        public Vector2 _origin;
        public Color _tint;

        public int _rotatedSheet;

        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRectangle, int rotatedSheet, float scale = 0.4f, float rotation = 0f, Vector2? origin = null, Color? tint = null)
        {
            _texture = texture;
            _position = position;
            _sourceRectangle = sourceRectangle;
            _scale = scale;
            _rotation = rotation;
            _origin = origin ?? new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2); 
            _tint = tint ?? Color.White; 
            _size= new Rectangle(0,0, (int)(_sourceRectangle.Width * scale), (int)(_sourceRectangle.Height*scale));//to je treba porihtat ker pol k dam shit gor ne gre_size= new Rectangle(0,0,_sourceRectangle.Width * scale, _sourceRectangle.Height*scale);
            _rotatedSheet= rotatedSheet;
        } // dodej layer depth

        public float AdjustedRotation => _rotatedSheet == 1 ? _rotation - 1.6f : _rotation;

        public void Draw(SpriteBatch spriteBatch)
        {   
            spriteBatch.Draw(
                _texture,
                _position,
                _sourceRectangle,
                _tint,
                AdjustedRotation,
                _origin,  
                _scale,
                SpriteEffects.None,
                0f
            );
        }
        public void Draw(SpriteBatch spriteBatch, Color overrideTint)
        {   
            spriteBatch.Draw(
                _texture,
                _position,
                _sourceRectangle,
                overrideTint,
                AdjustedRotation,
                _origin,
                _scale,
                SpriteEffects.None,
                0f
            );
        }

        public Sprite Clone()
        {
            return new Sprite(
                this._texture,
                this._position,
                this._sourceRectangle,
                this._rotatedSheet,
                this._scale,
                this._rotation,
                this._origin,
                this._tint
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
        public float Rotation => AdjustedRotation;
        public Vector2 Origin => _origin;
        public Color Tint => _tint;
        public Rectangle Size => _size;
        public int RotatedSheet => _rotatedSheet;
        
    }
}
