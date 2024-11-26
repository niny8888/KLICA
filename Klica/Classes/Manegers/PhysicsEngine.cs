using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Klica.Classes
{
    public class PhysicsEngine
    {
        private Level _level;
        private List<Food> _foodItems;

        
        public PhysicsEngine(Level level)
        {
            _level = level;
            _foodItems = new List<Food>();
        }

        
        public void AddFood(Food food)
        {
            _foodItems.Add(food);
        }

        
        public void Update(GameTime gameTime, Vector2 playerPosition, ref int score)
        {
            foreach (var food in _foodItems)
            {
                
                food.Update(gameTime, _level.Bounds);

                
                if (Vector2.Distance(food.Position, playerPosition) < food.CollisionRadius)
                {
                    food.OnConsumed(ref score);
                }
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
    }
}
