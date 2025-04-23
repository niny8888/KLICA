using Klica.Classes;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Klica.Classes.Objects_sprites;

namespace Klica{
    public class OrganismBuilder{
        public Base _organism_base;
        public Eyes _organism_eye;
        public Mouth _organism_mouth;
        private Physics _physics;
        private PhysicsEngine _physicsEngine;
        private Vector2 _lastMovementDirection = Vector2.Zero;
        public int _health { get; internal set; }
        public Vector2 _position { get; internal set; }

        public OrganismBuilder(Base baseSprite, Eyes eye, Mouth mouth, PhysicsEngine physicsEngine){
            _organism_base = baseSprite;
            _organism_eye = eye;
            _organism_mouth = mouth;
            _physics = new Physics(_organism_base.GetPosition());
            _physicsEngine = physicsEngine;
        }
        public void UpdateOrganism(Vector2 movementDirection, GameTime gameTime)
        {
            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            _physics.Update(movementDirection);
            _organism_base.SetPosition(_physics.GetPosition());
            _position = _organism_base.GetPosition();
            _organism_base.SetRotation((float)Math.Atan2(_physics._velocity.Y, _physics._velocity.X) + 1.6f);

            _organism_eye.SetPosition(_organism_base._position_eyes);
            _organism_eye.SetRotation(_organism_base.GetRotation());
            _organism_mouth.SetPosition(_organism_base._position_mouth, movementDirection.X, movementDirection.Y);
            _organism_mouth.SetRotation(_organism_base.GetRotation());
            
            bool isMouthOpening = false;
            bool FoodConsumed = false;
            if (_physicsEngine != null && gameTime.TotalGameTime.Milliseconds % 100 == 0) // Check every 100 milliseconds
            {
                FoodConsumed = _physicsEngine._foodItems.Exists(food => food.IsConsumed);
            }

            _organism_mouth.CheckFoodCollisions(_organism_base._position_mouth, _organism_base.GetRotation(), ref FoodConsumed);
        }


        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                System.Console.WriteLine("Enemy died!");
                _health=0;
            }
        }

        public void DrawOrganism(SpriteBatch _spriteBatch, GameTime _gameTime){
            _organism_base.Draw(_spriteBatch);
            _organism_eye.Draw(_spriteBatch, _gameTime);
            _organism_mouth.Draw(_spriteBatch);
        }
        
    }
}