using System;
using System.Security.Cryptography.X509Certificates;
using Klica;
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
        private Physics _physics;

        private PhysicsEngine _physicsEngine;
        private Mouth _playerMouth;

        
        public Player(PhysicsEngine physicsEngine){
            _physics = new Physics();
            _physicsEngine=physicsEngine;
            Sprite leftMouth = SpriteManager.getInstance().GetSprite("ustaL");
            Sprite rightMouth = SpriteManager.getInstance().GetSprite("ustaD");
            _playerMouth = new Mouth(leftMouth, rightMouth, Vector2.Zero);
        }

        public Vector2 _position { get; internal set; }

         public void UpdatePlayer(GameTime gameTime)
        {
            Vector2 movementDirection = Vector2.Zero;

            // Keyboard input
            if (Keyboard.GetState().IsKeyDown(Keys.W)) movementDirection.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) movementDirection.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) movementDirection.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) movementDirection.X += 1;

            // Normalize movement direction
            if (movementDirection != Vector2.Zero) movementDirection.Normalize();

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

            // Determine if the mouth should open based on proximity to food
            bool isMouthOpening = _physicsEngine._foodItems.Exists(food =>
                !food.IsConsumed && Vector2.Distance(_player_base.GetMouthPosition(), food.Position) <= 20f); // 20f is the threshold

            // Update mouth (animate open/close)
            _playerMouth.Update(_player_base.GetMouthPosition(), isMouthOpening);
        }

        public void DrawPlayer(SpriteBatch _spriteBatch){
             _player_base.Draw(_spriteBatch);
             _player_eye.Draw(_spriteBatch);
             _playerMouth.Draw(_spriteBatch);
         }


    }
}
