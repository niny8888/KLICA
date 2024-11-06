using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Managers
{
    public class AssetManager
    {
        // Singleton instance
        private static AssetManager _instance;
        
        // Static property to access the singleton instance
        public static AssetManager Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new AssetManager();
                }
                return _instance;
            }
        }

        // Private ContentManager and loaded assets
        private ContentManager _content;
        public Texture2D SpriteSheet { get; private set; }

        // Private constructor to enforce singleton pattern
        private AssetManager() { }

        // Initialize method to set ContentManager
        public void Initialize(ContentManager content)
        {
            if (_content == null)
            {
                _content = content;
            }
        }

        // Method to load all assets
        public void LoadAssets()
        {
            if (_content == null)
            {
                throw new System.InvalidOperationException("AssetManager not initialized. Call Initialize(ContentManager) first.");
            }

            // Load your sprite sheet and other assets here
            SpriteSheet = _content.Load<Texture2D>("Sprites/sprite_sheet");
            // Load other assets like fonts, sounds if needed
        }
    }
}
