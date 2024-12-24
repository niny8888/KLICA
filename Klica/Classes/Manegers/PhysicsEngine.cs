using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Klica.Classes
{
    public class PhysicsEngine
    {
        private Level _level;
        public List<Food> _foodItems = new List<Food>();

        public PhysicsEngine(Level level)
        {
            _level = level;

            for (int i = 0; i < level._FoodNum - 1; i++)
            {
                _foodItems.Add(CreateRandomFood());
            }
        }

        public void AddFood(Food food)
        {
            _foodItems.Add(food);
        }

        public void Update(GameTime gameTime, Vector2 playerMouthPosition, ref int score)
        {
            foreach (var food in _foodItems)
            {
                food.Update(gameTime, _level.Bounds, playerMouthPosition, ref score);
            }
            _foodItems.RemoveAll(food => food.IsConsumed);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var food in _foodItems)
            {
                food.Draw(spriteBatch);
            }
        }

        private Food CreateRandomFood()
        {
            Rectangle bounds = _level.Bounds;

            // Randomize position within bounds
            float x = _level.Random.Next(bounds.Left, bounds.Right);
            float y = _level.Random.Next(bounds.Top, bounds.Bottom);
            Vector2 position = new Vector2(x, y);

            // Randomize direction (normalized vector)
            float dirX = (float)(_level.Random.NextDouble() * 2 - 1); // -1 to 1
            float dirY = (float)(_level.Random.NextDouble() * 2 - 1);
            Vector2 direction = new Vector2(dirX, dirY);
            direction.Normalize();

            // Randomize speed within a reasonable range (e.g., 50 to 150)
            float speed = _level.Random.Next(50, 150);

            return new Food(position, direction, speed);
        }

        /// <summary>
        /// Checks for a collision between two rectangles.
        /// </summary>
        private bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }
    }
}
