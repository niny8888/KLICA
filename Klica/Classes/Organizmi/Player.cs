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
        private Vector2 _lastMovementDirection = Vector2.Zero;

        ///FIZKA
        public Vector2 _position { get; internal set; }
        public int _health { get; internal set; }
        private bool _hasStarted = false;
        public Vector2 Velocity { get; set; }
        public float Mass { get; private set; } = 5f;
        public float Restitution { get; private set; } = 0.5f;


        // Collision properties
        private Collider _baseCollider;
        private Collider _mouthCollider;
        
        private Collider _mouthProximityCollider;

        /// DASH
        private float _dashCooldown = 1.5f;     // Seconds between dashes
        private float _dashTimer = 0f;          // Time since last dash
        private float _dashStrength = 20f;     // Velocity impulse
        private bool _canDash = true;


        public Player(PhysicsEngine physicsEngine)
        {   
            _position = new Vector2(1920 / 2, 1080 / 2);
            _physics = new Physics(_position);
            _player_base.SetPosition(_position);
            _baseCollider = new Collider(_player_base.GetPosition(), _player_base.Width/2f, this);
            _baseCollider.Owner = this; // or pass owner from Player if needed

            _mouthCollider = new Collider(_player_base._position_mouth, 10f, this);
            _mouthCollider.Owner = this; // or pass owner from Player if needed

            _mouthProximityCollider = new Collider(_player_base._position_mouth, 25f, this); // Larger radius than mouth
            _health = 100;
            Mass = 5f;
        }
        

       
// ==============================================
// ============== UPDATE  =================
// ==============================================

        public void UpdatePlayer(GameTime gameTime, Rectangle bounds)
        {
            Vector2 movementDirection = Vector2.Zero;
            
            // Keyboard input
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                movementDirection.Y -= 1;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                movementDirection.X -= 1;

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                movementDirection.Y += 1;

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                movementDirection.X += 1;


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
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            
            // Apply movement if started
            if (_hasStarted || movementDirection != Vector2.Zero)
            {
                _hasStarted = true;

                if (movementDirection != Vector2.Zero) movementDirection.Normalize();

                // Update physics based on movement direction
                _physics.Update(movementDirection);

                // Apply velocity (bouncing effect)
                _position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // DASH SYSTEM — ADD THIS BLOCK HERE
                _dashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_dashTimer >= _dashCooldown)
                {
                    _canDash = true;
                }

                if (_canDash && Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Vector2 dashDirection = movementDirection;
                    if (dashDirection == Vector2.Zero)
                        dashDirection = _lastMovementDirection;

                    if (dashDirection != Vector2.Zero)
                    {
                        dashDirection.Normalize();
                        _physics._velocity += dashDirection * _dashStrength;
                        _canDash = false;
                        _dashTimer = 0f;
                        Console.WriteLine("DASH!");
                    }
                }



                Vector2 newPosition = _physics.GetPosition();

                float halfWidth = _player_base.Width / 2f;
                float halfHeight = _player_base.Height / 2f;

                newPosition.X = MathHelper.Clamp(newPosition.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);
                

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
            _player_mouth.Update();
            _player_mouth.SetPosition(_player_base._position_mouth, movementDirection.X, movementDirection.Y);
            _player_mouth.SetRotation(_player_base.GetRotation());

            // Update colliders
            _baseCollider.Position = _position;
            _mouthCollider.Position = _player_base._position_mouth;
            _mouthProximityCollider.Position = _player_base._position_mouth;


            // Update last movement direction
            if (movementDirection != Vector2.Zero)
            {
                _lastMovementDirection = movementDirection;
            }
        }
        
// ==============================================
// ============== DRAW  =================
// ==============================================
        public void DrawPlayer(SpriteBatch _spriteBatch, GameTime _gameTime){
             _player_base.Draw(_spriteBatch);
             _player_eye.Draw(_spriteBatch,_gameTime);
             _player_mouth.Draw(_spriteBatch);
         }

// ==============================================
// ============== GETTERS  =================
// ==============================================
         public float GetRotation()
        {
            return _player_base.GetRotation();
        }
        public Collider GetBaseCollider() => _baseCollider;
        public Collider GetMouthCollider() => _mouthCollider;
        public Collider GetMouthProximityCollider() => _mouthProximityCollider;

// ==============================================
// ============== FIZKA =================
// ==============================================
        /// to ne dela...
        /// timer za ta cas da ga nemorem kontrolirat
        /// impulse    
        public void ApplyBounce(Vector2 direction, float strength)
        {
            //ne ber imput
            Console.WriteLine("Before Bounce: " + Velocity);
            if (direction != Vector2.Zero)
                direction.Normalize();
            Velocity += direction * strength;
            Console.WriteLine("After Bounce: " + Velocity);
            //lahko nazaj bere input
        }
        
// ==============================================
// ============== DMG =================
// ==============================================
    public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
           {
                System.Console.WriteLine("Game over! U died!");
                _health=0;
            } 
        }


    public void DrawHealthBar(SpriteBatch spriteBatch)
    {
        int barWidth = 40;
        int barHeight = 5;
        int offsetY = -50;

        float healthPercent = MathHelper.Clamp(_health / 100f, 0f, 1f);
        Vector2 barPosition = _position + new Vector2(-barWidth / 2, offsetY);

        // Background
        spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
        // Fill
        spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercent), barHeight), Color.Blue);
    }

    }

    
}
