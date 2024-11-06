using System.Collections.Generic;
using Klica.Classes.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Managers
{
    public class GameManager
    {
        private Organism _playerOrganism;
        private List<Food> _foods;
        private List<Enemy> _enemies;

        public GameManager()
        {
            // Initialize game objects
            _playerOrganism = new Organism();
            _foods = new List<Food>();
            _enemies = new List<Enemy>();
        }

        public void Update(GameTime gameTime)
        {
            _playerOrganism.Update(gameTime);

            foreach (var food in _foods)
                food.Update(gameTime);

            foreach (var enemy in _enemies)
                enemy.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _playerOrganism.Draw(spriteBatch);

            foreach (var food in _foods)
                food.Draw(spriteBatch);

            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch);
        }
    }
}
