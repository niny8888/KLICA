using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;

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

        public void Update(GameTime gameTime, Vector2 playerMouthPosition, ref int score, Player player, List<Enemy> enemies)
        {
            // Update food items
            foreach (var food in _foodItems)
            {
                food.Update(gameTime, _level.Bounds, playerMouthPosition, ref score);
            }

            // Handle player-food collisions
            foreach (var food in _foodItems)
            {
                if (IsCollision(player.GetBaseCollider(), food))
                {
                    HandleCollision(player, food);
                }
            }

            // Handle enemy-food collisions
            foreach (var enemy in enemies)
            {
                foreach (var food in _foodItems)
                {
                    if (IsCollision(enemy.GetBaseCollider(), food))
                    {
                        HandleCollision(enemy, food);
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

        private bool IsCollision(Collider entityCollider, Food food)
        {
            // Check if the distance between the two colliders is less than the sum of their radii
            float distance = Vector2.Distance(entityCollider.Position, food.Position);
            float combinedRadius = entityCollider.Radius + food.CollisionRadius;

            return distance < combinedRadius;
        }
        
        private void HandleCollision(dynamic entity, Food food)
        {
            // Calculate collision normal
            Vector2 normal = Vector2.Normalize(food.Position - entity.GetBaseCollider().Position);

            // Relative velocity
            Vector2 relativeVelocity = food.Velocity - entity.Velocity;

            // Velocity along the normal
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // If objects are separating, skip collision
            if (velocityAlongNormal > 0) return;

            // Combined restitution coefficient
            float combinedRestitution = entity.Restitution * food.Restitution;

            // Inverse masses
            float invMassEntity = entity.Mass > 0 ? 1 / entity.Mass : 0;
            float invMassFood = food.Mass > 0 ? 1 / food.Mass : 0;

            // Impulse scalar
            float impulse = -(1 + combinedRestitution) * velocityAlongNormal;
            impulse /= invMassEntity + invMassFood;

            // Apply impulse
            Vector2 impulseVector = impulse * normal;
            food.Velocity += impulseVector * invMassFood;

            // Optional: If entity is dynamic (e.g., can move), apply impulse to entity
            if (invMassEntity > 0)
            {
                entity.Velocity -= impulseVector * invMassEntity;
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
            //float mass = (float)_level.Random.NextDouble() * 2 + 0.5f; // Mass between 0.5 and 2.5
            float restitution = (float)_level.Random.NextDouble() * 0.5f + 0.5f; // Restitution between 0.5 and 1.0

            Food food = new Food(position, direction, speed)
            {
                //Mass = mass,
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
