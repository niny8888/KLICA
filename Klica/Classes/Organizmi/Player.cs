using System;
using Klica.Classes.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites
{
    public class Player
    {
        // Player properties
        private Base _player_base = new Base(0);
        private Eyes _player_eye= new Eyes(0);
        public Mouth _player_mouth = new Mouth(0);

        // Physics properties
        private Physics _physics;
        private PhysicsEngine _physicsEngine;
        private Vector2 _lastMovementDirection = Vector2.Zero;

        public Vector2 _position { get; internal set; }
        public int _health { get; internal set; }
        private bool _hasStarted = false;
        public Vector2 Velocity { get; set; }
        public float Mass { get; private set; } = 5f;
        public float Restitution { get; private set; } = 0.5f;


        // Collision properties
        private Collider _baseCollider;
        private Collider _mouthCollider;


        public Player(PhysicsEngine physicsEngine)
        {   
            _position = new Vector2(1920 / 2, 1080 / 2);
            _physics = new Physics(_position);
            _physicsEngine = physicsEngine;
            _player_base.SetPosition(_position);
            _baseCollider = new Collider(_player_base.GetPosition(), _player_base.Width/2f, this);
            _mouthCollider = new Collider(_player_base._position_mouth, 10f, this);
            _health = 100;
            Mass = 5f;
        }
        

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                System.Console.WriteLine("Game over! U died!");
                _health=0;
            }
        }

       

        public void UpdatePlayer(GameTime gameTime, Rectangle bounds)
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

            // Apply movement if started
            if (_hasStarted || movementDirection != Vector2.Zero)
            {
                _hasStarted = true;

                if (movementDirection != Vector2.Zero) movementDirection.Normalize();

                // Update physics based on movement direction
                _physics.Update(movementDirection);

                // Apply velocity (bouncing effect)
                _position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                Vector2 newPosition = _physics.GetPosition();

                System.Console.WriteLine("1 ..... Position: " + newPosition   + "Bounds:  " + bounds);
                
                float halfWidth = _player_base.Width / 2f;
                float halfHeight = _player_base.Height / 2f;

                newPosition.X = MathHelper.Clamp(newPosition.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);

                System.Console.WriteLine("2 ..... Position: " + newPosition   + "Bounds:  " + bounds);

                // Apply the clamped position to physics and player
                _physics._positon = newPosition;
                _player_base.SetPosition(newPosition);
                _position = newPosition;

                // Apply friction to slow down bounce velocity
                Velocity *= 0.90f;

                
                // Set position based on physics after user input
                // _player_base.SetPosition(_physics.GetPosition());
                // _position = _player_base.GetPosition();
            }

            _player_base.SetRotation((float)Math.Atan2(_physics._velocity.Y, _physics._velocity.X) + 1.6f);

            // Update eye position
            _player_eye.SetPosition(_player_base._position_eyes);
            _player_eye.SetRotation(_player_base.GetRotation());

            // Update mouth position
            _player_mouth.SetPosition(_player_base._position_mouth, movementDirection.X, movementDirection.Y);
            _player_mouth.SetRotation(_player_base.GetRotation());

            // Update colliders
            _baseCollider.Position = _position;
            _mouthCollider.Position = _player_base._position_mouth;

            // Update last movement direction
            if (movementDirection != Vector2.Zero)
            {
                _lastMovementDirection = movementDirection;
            }
        }
        public Collider GetBaseCollider() => _baseCollider;
        public Collider GetMouthCollider() => _mouthCollider;

        public void DrawPlayer(SpriteBatch _spriteBatch, GameTime _gameTime){
             _player_base.Draw(_spriteBatch);
             _player_eye.Draw(_spriteBatch,_gameTime);
             _player_mouth.Draw(_spriteBatch);
         }
         public float GetRotation()
        {
            return _player_base.GetRotation();
        }
        public void ApplyBounce(Vector2 direction, float strength)
        {
            Velocity += direction * strength;
        }


    }
}
