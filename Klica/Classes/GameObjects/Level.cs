using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes
{
    public class Level
    {
        public Rectangle Bounds { get; private set; } //velikost k je playable
        public Texture2D BackgroundTexture { get; private set; }
        public GameplayRules Rules { get; private set; } //Pravila
        public Random Random { get; } = new Random();

        public int _FoodNum { get; set; }

        public Level(Rectangle bounds, Texture2D backgroundTexture, GameplayRules rules, int FoodNumber)
        {
            Bounds = bounds;
            BackgroundTexture = backgroundTexture;
            Rules = rules;
            _FoodNum=FoodNumber;
        }


        // Metoda k forca position
        public Vector2 ClampPosition(Vector2 position)
        {
            float clampedX = MathHelper.Clamp(position.X, Bounds.Left, Bounds.Right);
            float clampedY = MathHelper.Clamp(position.Y, Bounds.Top, Bounds.Bottom);
            return new Vector2(clampedX, clampedY);
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackgroundTexture, Bounds, Color.White);
        }

        public bool IsOutOfBounds(Vector2 position)
        {
            return !Bounds.Contains(position);
        }
    }
}
