using System;
using Klica.Classes.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites
{
    public class Player
    {
        private Base _player_base = new Base(0);
        private Eyes _player_eye= new Eyes(0);
        public Mouth _player_mouth = new Mouth(0);
        private Physics _physics;

        private PhysicsEngine _physicsEngine;
        private Vector2 _lastMovementDirection = Vector2.Zero;

         public Vector2 _position { get; internal set; }
         public int _health { get; internal set; }
        
        public Player(PhysicsEngine physicsEngine)
        {
            _physics = new Physics();
            _physicsEngine = physicsEngine;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                System.Console.WriteLine("Game over! U died!");
            }
        }

       

         public void UpdatePlayer(GameTime gameTime)
        {
            Vector2 movementDirection = Vector2.Zero;

            // Keyboard input
            if (Keyboard.GetState().IsKeyDown(Keys.W)) movementDirection.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) movementDirection.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) movementDirection.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) movementDirection.X += 1;

            // Normalize movement direction
            if (movementDirection == Vector2.Zero && _lastMovementDirection != Vector2.Zero)
            {
                movementDirection = _lastMovementDirection;
            }


            // Gamepad input
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState.IsConnected)
            {
                movementDirection += new Vector2(gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y); // Y is inverted
            }

            // Normalize again if gamepad input was added
            if (movementDirection != Vector2.Zero) movementDirection.Normalize();

            // Update physics
            _physics.Update(movementDirection);
            _player_base.SetPosition(_physics.GetPosition());
            _position = _player_base.GetPosition();
            _player_base.SetRotation((float)Math.Atan2(_physics._velocity.Y, _physics._velocity.X) + 1.6f);

            // Update eye position
            _player_eye.SetPosition(_player_base._position_eyes);
            _player_eye.SetRotation(_player_base.GetRotation());
            
            // Update mouth position
            _player_mouth.SetPosition(_player_base._position_mouth,movementDirection.X,movementDirection.Y);
            _player_mouth.SetRotation(_player_base.GetRotation());
           
           ///###################################za popravt!!!!!
           ///  V V V V V


            // Determine if the mouth should open based on proximity to food
            bool isMouthOpening = false;
            bool FoodConsumed= false;
            if (gameTime.TotalGameTime.Milliseconds % 100 == 0) // Check every 100 milliseconds
            {
                FoodConsumed = _physicsEngine._foodItems.Exists(food =>
                    food.IsConsumed); 
            }
            //System.Console.WriteLine("Food consumed: " + FoodConsumed);
            // Update mouth (animate open/close)
            _player_mouth.CheckFoodCollisions(_player_base._position_mouth,_player_base.GetRotation(),ref FoodConsumed);
            
            if (movementDirection != Vector2.Zero)
            {
                _lastMovementDirection = movementDirection;
            }
            
        }

        public void DrawPlayer(SpriteBatch _spriteBatch, GameTime _gameTime){
             _player_base.Draw(_spriteBatch);
             _player_eye.Draw(_spriteBatch,_gameTime);
             _player_mouth.Draw(_spriteBatch);
         }


    }
}
