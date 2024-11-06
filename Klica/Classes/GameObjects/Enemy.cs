using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Klica.Classes.GameObjects
{
    public class Enemy
    {
        // Position, velocity, and size of the enemy
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        private readonly int _width;
        private readonly int _height;
        
        // Sprite and texture source rectangle
        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        // Health and damage properties
        public int Health { get; private set; }
        private int _damage;

        // Constructor
        public Enemy(Texture2D texture, Rectangle sourceRectangle, Vector2 startPosition, int health = 10, int damage = 1)
        {
            _texture = texture;
            _sourceRectangle = sourceRectangle;
            Position = startPosition;
            Health = health;
            _damage = damage;
            _width = sourceRectangle.Width;
            _height = sourceRectangle.Height;

            // Initial velocity - can be random or specific based on enemy type
            Random random = new Random();
            Velocity = new Vector2((float)(random.NextDouble() - 0.5) * 2, (float)(random.NextDouble() - 0.5) * 2);
        }

        // Update method to handle movement and AI behavior
        // Method to update position based on velocity
        public void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            HandleBoundsCollision();
        }

        // Method to reverse velocity on collision with bounds
        private void HandleBoundsCollision()
        {
            if (Position.X <= 0 || Position.X + _texture.Width >= Game1.ScreenWidth)
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse X velocity
            }
            if (Position.Y <= 0 || Position.Y + _texture.Height >= Game1.ScreenHeight)
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse Y velocity
            }
        }

        // Method to handle collisions with the player or projectiles
        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                OnDeath();
            }
        }

        // Called when the enemy dies
        private void OnDeath()
        {
            // Handle enemy death (e.g., drop loot, play sound, remove from game)
            Console.WriteLine("Enemy defeated!");
        }

        // Draw method to render the enemy on the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _sourceRectangle, Color.White);
        }

        // Collision rectangle for detecting interactions with the player
        public Rectangle GetCollisionRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
        }
    }
}
