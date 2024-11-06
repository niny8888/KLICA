using Klica.Classes.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.GameObjects
{
    public class Organism
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;

        public Organism()
        {
            Position = new Vector2(100, 100);
            _texture = AssetManager.Instance.SpriteSheet; // Use texture from AssetManager
        }

        public void Update(GameTime gameTime)
        {
            // Update organism movement and behavior here
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 64, 64), Color.White); // Draw a specific part of the sprite sheet
        }
    }
}
