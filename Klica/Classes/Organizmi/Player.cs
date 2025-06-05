using System;
using System.Collections.Generic;
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
        private Eyes _player_eye = new Eyes(0);
        public Mouth _player_mouth = new Mouth(0);

        // Physics properties
        private Physics _physics;
        private Vector2 _lastMovementDirection = Vector2.Zero;

        ///FIZKA
        public Vector2 _position { get; internal set; }
        public int _health { get; internal set; }

        public int _maxhealth { get; internal set; }
        private bool _hasStarted = false;
        public Vector2 Velocity { get; set; }
        public float Mass { get; private set; } = 5f;
        public float Restitution { get; private set; } = 0.5f;

        private Vector2 _bounceImpulse = Vector2.Zero;
        private float _inputLockTimer = 0f;
        private bool _bounceActive = false;
        private float _bounceTimer = 0f;
        private float _bounceDuration = 0f;
        private Vector2 _currentBounceDirection = Vector2.Zero;
        private float _currentBounceStrength = 0f;






        // Collision properties
        private Collider _baseCollider;
        private Collider _mouthCollider;

        private Collider _mouthProximityCollider;

        /// DASH
        private float _dashCooldown = 1.5f;
        private float _dashTimer = 0f;
        private float _dashStrength = 20f;
        public int _dashCharges = 1;
        public int _maxDashCharges = 1;
        private float _dashRechargeTime = 2.5f;
        private float _dashRechargeTimer = 0f;
        public bool _canDash = false;
        public bool _isDashing = false;
        private bool _spacePreviouslyPressed = false;
        public List<EvolutionTrait> ActiveTraits { get; private set; } = new();

        /// TRAIT SPECIFIC
        private float _regenTimer = 0f;
        private const float RegenRate = 1f; // 1 HP/sec
        private const float RegenCooldown = 3f;
        private float _lastDamageTime = 0f;
        private float _frenzyTimer = 0f;
        private float _frenzyDuration = 0f;

        public bool _hasShellArmor = false;
        public bool _hasStunDash = false;
        public bool _hasSlowTouch = false;
        public bool _hasFeederMode = false;
        public bool _hasFrenzyMode = true;
        private bool _isFrenzy = false;





        public Player(PhysicsEngine physicsEngine)
        {
            _position = new Vector2(1920 / 2, 1080 / 2);
            _physics = new Physics(_position);
            _player_base.SetPosition(_position);
            _baseCollider = new Collider(_player_base.GetPosition(), _player_base.Width / 2f, this);
            _baseCollider.Owner = this; // or pass owner from Player if needed

            _mouthCollider = new Collider(_player_base._position_mouth, 10f, this);
            _mouthCollider.Owner = this; // or pass owner from Player if needed

            _mouthProximityCollider = new Collider(_player_base._position_mouth, 25f, this); // Larger radius than mouth
            _health = 100;
            _maxhealth = _health;
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

            if (_inputLockTimer <= 0f)
            {
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
            }




            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_inputLockTimer > 0)
                _inputLockTimer -= dt;

            if (_frenzyDuration > 0f)
            {
                _frenzyTimer += dt;
                if (_frenzyTimer <= _frenzyDuration)
                {
                    _physics._velocity *= 1.1f;
                    _dashStrength = 30f;
                }
                else
                {
                    _frenzyTimer = 0f;
                    _frenzyDuration = 0f;
                    _dashStrength = 20f;
                    _isFrenzy = false;
                }
            }

            // Apply movement if started
            if (_hasStarted || movementDirection != Vector2.Zero)
            {
                _hasStarted = true;
                if (movementDirection != Vector2.Zero) movementDirection.Normalize();

                // _physics.Update(movementDirection);

                if (_bounceTimer > 0f)
                {
                    _bounceTimer -= dt;

                    // Reduce the strength gradually over time
                    float decayFactor = _bounceTimer / _bounceDuration;
                    Vector2 bounceForce = _currentBounceDirection * _currentBounceStrength * decayFactor;

                    _physics.Update(bounceForce);
                    _physics._velocity *= 0.92f;

                }
                else
                {
                    _physics.Update(movementDirection);
                }





                _physics._velocity += _bounceImpulse;
                _bounceImpulse = Vector2.Zero;
                _position += _physics._velocity * dt;


                _dashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_dashCharges < _maxDashCharges)
                {
                    _dashRechargeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_dashRechargeTimer >= _dashRechargeTime)
                    {
                        _dashCharges++;
                        _dashRechargeTimer = 0f;
                    }
                }

                bool spacePressed = keyboardState.IsKeyDown(Keys.Space);
                if (_dashCharges > 0 && spacePressed && !_spacePreviouslyPressed && _canDash)
                {
                    Vector2 dashDirection = movementDirection;
                    if (dashDirection == Vector2.Zero)
                        dashDirection = _lastMovementDirection;

                    if (dashDirection != Vector2.Zero)
                    {
                        dashDirection.Normalize();
                        _physics._velocity += dashDirection * _dashStrength;
                        _dashCharges--;
                        _dashRechargeTimer = 0f;
                        _isDashing = true;
                        _dashTimer = 0f;
                        Console.WriteLine("DASH!");
                    }
                }

                // End dash after 0.2s
                if (_isDashing && _dashTimer > 0.2f)
                {
                    _isDashing = false;
                }


                _spacePreviouslyPressed = spacePressed;


                Vector2 newPosition = _physics.GetPosition();

                float halfWidth = _player_base.Width / 2f;
                float halfHeight = _player_base.Height / 2f;

                newPosition.X = MathHelper.Clamp(newPosition.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);

                _physics._positon = newPosition;
                _position = newPosition;
                _player_base.SetPosition(newPosition);

                // Vector2 newPosition = _physics.GetPosition();

                // float halfWidth = _player_base.Width / 2f;
                // float halfHeight = _player_base.Height / 2f;

                // newPosition.X = MathHelper.Clamp(newPosition.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
                // newPosition.Y = MathHelper.Clamp(newPosition.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);


                // // Apply the clamped position to physics and player
                // _physics._positon = newPosition;
                // _player_base.SetPosition(newPosition);
                // _position = newPosition;

                // Apply friction to slow down bounce velocity
                Velocity *= 0.90f;
                _physics._velocity *= 0.90f;


                // Set position based on physics after user input
                // _player_base.SetPosition(_physics.GetPosition());
                // _position = _player_base.GetPosition();
            }


            //TRAITS
            // Regen logic
            if (HasTrait(EvolutionTrait.Regeneration))
            {
                _regenTimer += dt;
                _lastDamageTime += dt;

                if (_lastDamageTime >= RegenCooldown && _regenTimer >= 1f && _health < _maxhealth)
                {
                    _health += 1;
                    _regenTimer = 0f;
                }
            }



            /// KOSI

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
        public void DrawPlayer(SpriteBatch _spriteBatch, GameTime _gameTime)
        {
            Color tint = _frenzyDuration > 0 ? Color.OrangeRed : Color.White;
            _player_base.Draw(_spriteBatch, tint);
            _player_eye.Draw(_spriteBatch, _gameTime);
            _player_mouth.Draw(_spriteBatch, tint);

            //  _player_base.Draw(_spriteBatch);
            //  _player_eye.Draw(_spriteBatch,_gameTime);
            //  _player_mouth.Draw(_spriteBatch);
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

        public void ApplyBounce(Vector2 direction, float strength, float duration = 0.3f)
        {
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                _currentBounceDirection = direction;
                _currentBounceStrength = strength;
                _bounceTimer = duration;
                _bounceDuration = duration;
                _inputLockTimer = duration * 0.3f;
            }
        }






        // ==============================================
        // ============== DMG =================
        // ==============================================
        public void TakeDamage(int damage)
        {
            _lastDamageTime = 0f;

            if (_hasShellArmor)
                damage = (int)(damage * 0.5f);

            _health -= damage;
            if (_health <= 0)
            {
                System.Console.WriteLine("Game over! U died!");
                _health = 0;
            }
        }


        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int barWidth = 40;
            int barHeight = 5;
            int offsetY = -50;

            float healthPercent = MathHelper.Clamp(_health / (float)_maxhealth, 0f, 1f);
            Vector2 barPosition = _position + new Vector2(-barWidth / 2, offsetY);

            // Background
            spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
            // Fill
            spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercent), barHeight), Color.Blue);

            if (_canDash)
            {
                for (int i = 0; i < _maxDashCharges; i++)
                {
                    Color color = i < _dashCharges ? Color.White : Color.Gray;
                    spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)(_position.X - 20 + i * 12), (int)_position.Y - 60, 10, 5), color);
                }
            }


        }
        public void SetPosition(Vector2 pos)
        {
            _position = pos;
            _physics._positon = pos;
            _player_base.SetPosition(pos);

            // Also update colliders to prevent first-frame desync
            _baseCollider.Position = pos;
            _mouthCollider.Position = _player_base._position_mouth;
            _mouthProximityCollider.Position = _player_base._position_mouth;
        }

        /// TRAITS
        public void TriggerFrenzy()
        {
            if (_hasFrenzyMode && !_isFrenzy)
            {
                _isFrenzy = true;
                // Start frenzy mode
                _frenzyTimer = 0f;
                _frenzyDuration = 5f; // Set duration for frenzy mode
                _dashStrength = 30f; // Increase dash strength during frenzy
            }
        }



        public void AddTrait(EvolutionTrait trait)
        {
            if (!ActiveTraits.Contains(trait))
                ActiveTraits.Add(trait);

            switch (trait)
            {
                case EvolutionTrait.BonusHealth:
                    _health += 50;
                    _maxhealth += 50;
                    break;

                case EvolutionTrait.Dash:
                    _canDash = true;
                    break;

                case EvolutionTrait.ExtraDash:
                    _dashCharges=2;
                    _maxDashCharges=2;
                    break;
                case EvolutionTrait.Regeneration:
                    // No immediate stat change — regen logic will be in UpdatePlayer
                    break;

                case EvolutionTrait.StunDash:
                    _hasStunDash = true;
                    break;

                case EvolutionTrait.FrenzyMode:
                    _hasFrenzyMode = true;
                    _frenzyDuration = 5f;
                    break;

                case EvolutionTrait.ShellArmor:
                    _hasShellArmor = true;
                    break;

                case EvolutionTrait.FeederMode:
                    _hasFeederMode = true;
                    break;

                case EvolutionTrait.TraitMemory:
                    // Trait memory logic likely handled outside player (in evolution system)
                    break;

                case EvolutionTrait.SlowTouch:
                    _hasSlowTouch = true;
                    break;

            }
        }

        public bool HasTrait(EvolutionTrait trait)
        {
            return ActiveTraits.Contains(trait);
        }
        
        public void LoadTraits(List<EvolutionTrait> traits)
        {
            foreach (var trait in traits)
            {
                AddTrait(trait); // your existing trait logic
            }
        }





    }

    
}
