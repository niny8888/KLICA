using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;
using System.Linq;

namespace Klica.Classes
{
    public class PhysicsEngine
    {// This class is used for managing food colisions 
        private Level _level;
        public List<Food> _foodItems = new List<Food>();
        // public List<PeacefulEnemy> PeacefulEnemies { get; } = new();
        

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
        

        public void Update(GameTime gameTime, Vector2 playerMouthPosition, ref int score, Player player, List<Enemy> enemies)
        {
            if (enemies == null)
                enemies = new List<Enemy>();

            player._player_mouth.Close();

            foreach (var food in _foodItems)
            {
                food.Update(gameTime, _level.Bounds, playerMouthPosition, ref score);
                if (!food.IsConsumed && Vector2.Distance(food.Position, playerMouthPosition) <= food.CollisionRadius * 1.8f)
                {
                    player._player_mouth.Open();
                }
            }
            // Player - FOOD
            foreach (var food in _foodItems)
            {
                if (IsCollision(player.GetBaseCollider(), food))
                {
                    HandleCollision(player, food);
                }
            }

            // Enemy - FOOD
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

            // FOOD - FOOD
            for (int i = 0; i < _foodItems.Count; i++)
            {
                for (int j = i + 1; j < _foodItems.Count; j++)
                {
                    if (IsFoodCollision(_foodItems[i], _foodItems[j]))
                    {
                        HandleFoodCollision(_foodItems[i], _foodItems[j]);
                    }
                }
            }

            _foodItems.RemoveAll(food => food.IsConsumed);

            if (_foodItems.Count < _level._FoodNum)
            {
                _foodItems.Add(CreateRandomFood());
            }
        }

        private bool IsCollision(Collider entityCollider, Food food)
        {
            float distance = Vector2.Distance(entityCollider.Position, food.Position);
            float combinedRadius = entityCollider.Radius + food.CollisionRadius;

            return distance < combinedRadius;
        }
        
        private void HandleCollision(dynamic entity, Food food)
        {
            Vector2 normal = Vector2.Normalize(food.Position - entity.GetBaseCollider().Position);
            Vector2 relativeVelocity = food.Velocity - entity.Velocity;
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);
            if (velocityAlongNormal > 0) return;
            float combinedRestitution = entity.Restitution * food.Restitution;
            float invMassEntity = entity.Mass > 0 ? 1 / entity.Mass : 0;
            float invMassFood = food.Mass > 0 ? 1 / food.Mass : 0;
            float impulse = -(1 + combinedRestitution) * velocityAlongNormal;
            impulse /= invMassEntity + invMassFood;
            Vector2 impulseVector = impulse * normal;
            food.Velocity += impulseVector * invMassFood;

            if (invMassEntity > 0)
            {
                entity.Velocity -= impulseVector * invMassEntity;
            }
        }
        private bool IsFoodCollision(Food food1, Food food2)
        {
            float distance = Vector2.Distance(food1.Position, food2.Position);
            float combinedRadius = food1.CollisionRadius + food2.CollisionRadius;

            return distance < combinedRadius;
        }

        private void HandleFoodCollision(Food food1, Food food2)
        {
            Vector2 normal = Vector2.Normalize(food2.Position - food1.Position);
            Vector2 relativeVelocity = food2.Velocity - food1.Velocity;
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);
            if (velocityAlongNormal > 0) return;

            float combinedRestitution = food1.Restitution * food2.Restitution;

            float invMass1 = food1.Mass > 0 ? 1 / food1.Mass : 0;
            float invMass2 = food2.Mass > 0 ? 1 / food2.Mass : 0;

            float impulse = -(1 + combinedRestitution) * velocityAlongNormal;
            impulse /= invMass1 + invMass2;

            Vector2 impulseVector = impulse * normal;
            food1.Velocity -= impulseVector * invMass1;
            food2.Velocity += impulseVector * invMass2;

            float overlap = (food1.CollisionRadius + food2.CollisionRadius) - Vector2.Distance(food1.Position, food2.Position);
            if (overlap > 0)
            {
                Vector2 separation = normal * (overlap / 2);
                food1.Position -= separation;
                food2.Position += separation;
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

            float x = _level.Random.Next(bounds.Left, bounds.Right);
            float y = _level.Random.Next(bounds.Top, bounds.Bottom);
            Vector2 position = new Vector2(x, y);

            float angle = (float)(_level.Random.NextDouble() * Math.PI * 2);

            float dirX = (float)Math.Cos(angle);
            float dirY = (float)Math.Sin(angle);

            Vector2 direction = new Vector2(dirX, dirY);

            direction.Normalize();

            float speed = _level.Random.Next(20, 50);

            float restitution = (float)_level.Random.NextDouble() * 0.5f + 0.5f; 

            Food food = new Food(position, direction, speed)
            {
                Restitution = restitution
            };

            return food;
        }

        
        private bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }
        public List<Food> GetAllFood()
        {
            return _foodItems;
        }
        public List<Vector2> GetAllFoodPositions()
        {
            return _foodItems.Select(f => f.Position).ToList();
        }

        public void ClearFood()
        {
            _foodItems.Clear();
        }
    }
}
