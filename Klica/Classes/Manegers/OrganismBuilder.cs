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
        public Physics _physics;
        private PhysicsEngine _physicsEngine;
        private Random _random;
        private Vector2 _lastMovementDirection = Vector2.Zero;
        public int _health { get; internal set; }
        public Vector2 _position { get; internal set; }

        public OrganismBuilder(Base baseSprite, Eyes eye, Mouth mouth, PhysicsEngine physicsEngine){
            _organism_base = baseSprite;
            _random = new Random();
            _organism_eye = eye;
            _organism_mouth = mouth;
            _physics = new Physics(new Vector2(_random.Next(100, 1700), _random.Next(100, 950)));
            _physicsEngine = physicsEngine;
        }
        public void UpdateOrganism(GameTime gameTime)
        {
            // Use existing position from physics
            _organism_base.SetPosition(_physics.GetPosition());
            _position = _organism_base.GetPosition();

            // Get velocity for rotation
            Vector2 vel = _physics._velocity;
            if (vel.LengthSquared() > 0.001f)
            {
                _organism_base.SetRotation((float)Math.Atan2(vel.Y, vel.X) + 1.6f);
            }

            _organism_eye.SetPosition(_organism_base._position_eyes);
            _organism_eye.SetRotation(_organism_base.GetRotation());

            _organism_mouth.SetPosition(_organism_base._position_mouth, vel.X, vel.Y);
            _organism_mouth.SetRotation(_organism_base.GetRotation());

            bool foodConsumed = false;
            if (_physicsEngine != null && gameTime.TotalGameTime.Milliseconds % 100 == 0)
            {
                foodConsumed = _physicsEngine._foodItems.Exists(food => food.IsConsumed);
            }

            _organism_mouth.CheckFoodCollisions(_organism_base._position_mouth, _organism_base.GetRotation(), ref foodConsumed);
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