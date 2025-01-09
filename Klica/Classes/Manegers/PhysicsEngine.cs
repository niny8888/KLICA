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

            // Create initial food items
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
            // Update all food items
            foreach (var food in _foodItems)
            {
                food.Update(gameTime, _level.Bounds, playerMouthPosition, ref score);
            }

            // Check for collisions between food items
            for (int i = 0; i < _foodItems.Count; i++)
            {
                for (int j = i + 1; j < _foodItems.Count; j++)
                {
                    var foodA = _foodItems[i];
                    var foodB = _foodItems[j];

                    // Check if the food items are colliding
                    float distance = Vector2.Distance(foodA.Position, foodB.Position);
                    float collisionDistance = foodA.CollisionRadius + foodB.CollisionRadius;

                    if (distance < collisionDistance)
                    {
                        // Resolve collision using the Food's HandleCollision method
                        foodA.HandleCollision(foodB);
                    }
                }
            }

            // Remove consumed food
            _foodItems.RemoveAll(food => food.IsConsumed);

            // Add new food if needed
            if (_foodItems.Count < _level._FoodNum)
            {
                _foodItems.Add(CreateRandomFood());
            }
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

            // Generate a random angle in radians (0 to 2π)
            float angle = (float)(_level.Random.NextDouble() * Math.PI * 2);

            // Compute the direction vector using trigonometry
            float dirX = (float)Math.Cos(angle);
            float dirY = (float)Math.Sin(angle);

            // Create the direction vector
            Vector2 direction = new Vector2(dirX, dirY);

            direction.Normalize();

            // Randomize speed within a reasonable range (e.g., 50 to 150)
            float speed = _level.Random.Next(20, 50);

            // Randomize mass and restitution for diversity
            float mass = (float)_level.Random.NextDouble() * 2 + 0.5f; // Mass between 0.5 and 2.5
            float restitution = (float)_level.Random.NextDouble() * 0.5f + 0.5f; // Restitution between 0.5 and 1.0

            Food food = new Food(position, direction, speed)
            {
                Mass = mass,
                Restitution = restitution
            };

            return food;
        }

        
        private bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }
    }
}
