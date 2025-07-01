using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Klica.Classes.Objects_sprites;

namespace Klica.Classes.Organizmi
{
    public class Enemy : OrganismBuilder
    {
        public enum AggressiveEnemyState
        {
            Idle,
            ChasingFood,
            ChasingPlayer,
            Dying,
            Locked
        }

        private AggressiveEnemyState _currentState;

        private Vector2 _velocity;
        private Vector2 _targetPosition;
        private float _speed;
        private Random _random;
        private double _stateLockTimer;
        private bool _isStateLocked;

        private Collider _baseCollider;
        private Collider _mouthCollider;
        public float Mass { get; private set; } = 3f;
        public float Restitution { get; private set; } = 0.6f;
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public double _damageCooldown = 0;
        private int _aggressionLevel;

        public bool _isDead = false;
        private bool _hasDroppedFood = false;
        private double _deathTimer = 1.0;
        private float _deathRotation = 0f;
        private double _chaseTimer = 0;
        private double _maxChaseTime = 5.0; // 5 seconds

        //slow
        private float _originalSpeed;
        private float _slowTimer = 0f;
        private bool _isSlowed = false;

        private bool _wasBounced = false;

        //SUS
        private float _suspicionTimer = 0f;
        private bool _playerSpottedOnce = false;
        private const float _suspicionCheckTime = 5f;
        private const float _suspicionRange = 200f;

        //Drunkards
        private float _drunkAngle;
        private float _drunkTurnRate = 0.2f;
        private float _drunkSpeed = 0.5f;



        public Enemy(Base baseSprite, Eyes eye, Mouth mouth, int aggressionLevel)
            : base(baseSprite, eye, mouth, null)
        {
            _aggressionLevel = aggressionLevel;
            _currentState = AggressiveEnemyState.Idle;
            _random = new Random();
            _position = new Vector2(_random.Next(100, 1700), _random.Next(100, 950));
            _speed = 0.8f;
            _targetPosition = _position;
            _health = 100;
            _originalSpeed = _speed;


            _organism_base.SetPosition(_position);
            _organism_mouth.SetPosition(_organism_base._position_mouth, 0, 0);
            _organism_eye.SetPosition(_organism_base._position_eyes);

            _baseCollider = new Collider(_position, baseSprite.Width / 2f, this);
            _mouthCollider = new Collider(baseSprite._position_mouth, 10f, this);
            _drunkAngle = (float)(_random.NextDouble() * MathHelper.TwoPi);

        }

        public void Update(GameTime gameTime, PhysicsEngine physicsEngine, Player player)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_isSlowed)
            {
                _slowTimer -= dt;
                if (_slowTimer <= 0f)
                {
                    _isSlowed = false;
                    _speed = _originalSpeed;
                    Console.WriteLine("Enemy speed restored.");
                }
            }

            if (_damageCooldown > 0) _damageCooldown -= dt;

            if (_health <= 0 && _currentState != AggressiveEnemyState.Dying)
            {
                _currentState = AggressiveEnemyState.Dying;
                _isDead = true;
                return;
            }

            if (_isDead)
            {
                _deathTimer -= dt;
                _deathRotation += (float)(Math.PI * dt * 4);
                _organism_base.SetRotation(_deathRotation);

                if (player.HasTrait(EvolutionTrait.FrenzyMode))
                {
                    player.TriggerFrenzy();
                }

                if (_deathTimer <= 0 && !_hasDroppedFood)
                {
                    int foodDropCount = player.HasTrait(EvolutionTrait.FeederMode) ? 4 : 2;

                    for (int i = 0; i < foodDropCount; i++)
                    {
                        Vector2 offset = new Vector2(_random.Next(-10, 10), _random.Next(-10, 10));
                        Vector2 dir = Vector2.Normalize(offset + new Vector2(1, 1));
                        physicsEngine.AddFood(new Food(_position + offset, dir, 1f));
                    }

                    _hasDroppedFood = true;
                }
                


                return;
            }

            if (_isStateLocked)
            {
                _stateLockTimer -= dt;
                if (_stateLockTimer > 0) return;
                _isStateLocked = false;
                
            }

            Vector2 movementDirection = Vector2.Zero;
            switch (_currentState)
            {
                case AggressiveEnemyState.Idle:
                    movementDirection = UpdateIdleState(player, physicsEngine._foodItems.ToArray(),dt);
                    break;
                case AggressiveEnemyState.ChasingFood:
                    movementDirection = UpdateChasingFoodState(physicsEngine._foodItems.ToArray());
                    break;
                case AggressiveEnemyState.ChasingPlayer:
                    _chaseTimer += dt;
                    if (_chaseTimer > _maxChaseTime)
                    {
                        _chaseTimer = 0;
                        _currentState = AggressiveEnemyState.Idle;
                    }
                    movementDirection = player._position - _position;
                    break;
                case AggressiveEnemyState.Locked:
                    _velocity = Vector2.Zero;
                    if (_stateLockTimer <= 0)
                    {
                        _currentState = AggressiveEnemyState.Idle;
                    }
                    break;
            } 

            if (movementDirection != Vector2.Zero)
            {
                Vector2 steering = Seek(_position + movementDirection);
                _velocity += steering;
            }

            if (!_wasBounced)
            {
                _velocity *= 0.95f;
                if (_velocity.Length() > _speed)
                    _velocity = Vector2.Normalize(_velocity) * _speed;
            }
            _wasBounced = false; // reset

            _physics.Update(_velocity);
            UpdateOrganism(gameTime);
            _position = _organism_base.GetPosition();
            
            float halfWidth = _organism_base.Width / 2f;
            float halfHeight = _organism_base.Height / 2f;

            Vector2 clampedPos = _position;
            bool hitWall = false;

            if (_position.X < halfWidth || _position.X > 1920 - halfWidth)
            {
                _drunkAngle = MathHelper.Pi - _drunkAngle;
                hitWall = true;
            }
            if (_position.Y < halfHeight || _position.Y > 1080 - halfHeight)
            {
                _drunkAngle = -_drunkAngle;
                hitWall = true;
            }
            if (hitWall)
                _drunkAngle += (float)(_random.NextDouble() - 0.5f) * 0.3f;

            clampedPos.X = MathHelper.Clamp(clampedPos.X, halfWidth, 1920 - halfWidth);
            clampedPos.Y = MathHelper.Clamp(clampedPos.Y, halfHeight, 1080 - halfHeight);

            _position = clampedPos;
            _physics._positon = clampedPos;
            _organism_base.SetPosition(clampedPos);

            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        private Vector2 UpdateIdleState(Player player, Food[] foods, float dt)
        {
            // Drunkard wandering
            _drunkAngle += (float)(_random.NextDouble() - 0.5f) * _drunkTurnRate;
            Vector2 direction = new Vector2((float)Math.Cos(_drunkAngle), (float)Math.Sin(_drunkAngle));
            _targetPosition = _position + direction * 50f;


            float distanceToPlayer = Vector2.Distance(_position, player._position);

            if (distanceToPlayer < _suspicionRange)
            {
                if (!_playerSpottedOnce)
                {
                    _playerSpottedOnce = true;
                    _suspicionTimer = _suspicionCheckTime;
                }
                else
                {
                    _suspicionTimer -= dt;
                    if (_suspicionTimer <= 0f)
                    {
                        _currentState = AggressiveEnemyState.ChasingPlayer;
                        _chaseTimer = 0;
                        _playerSpottedOnce = false;
                    }
                }
            }
            else
            {
                _playerSpottedOnce = false;
                _suspicionTimer = 0f;
            }

            if (IsFoodInRange(foods, 50f))
                _currentState = AggressiveEnemyState.ChasingFood;

            return direction * _drunkSpeed;

        }

        private Vector2 UpdateChasingFoodState(Food[] foods)
        {
            Food closest = GetClosestFood(foods);
            if (closest != null)
            {
                _targetPosition = closest.Position;
                if (Vector2.Distance(_position, closest.Position) < 15f)
                {
                    closest.OnConsumedByAI();
                    LockState(AggressiveEnemyState.Idle, 1.0);
                }
            }
            else
            {
                _currentState = AggressiveEnemyState.Idle;
            }

            return _targetPosition - _position;
        }

        private Vector2 Seek(Vector2 target)
        {
            Vector2 desired = target - _position;
            if (desired != Vector2.Zero) desired.Normalize();
            desired *= _speed;
            return (desired - _velocity) * 0.3f; // used to be 0.5f

        }

        private Vector2 GetRandomTargetPosition()
        {
            return new Vector2(_random.Next(100, 1800), _random.Next(100, 1000));
        }

        private bool IsFoodInRange(IEnumerable<Food> foods, float range)
        {
            foreach (var food in foods)
            {
                if (Vector2.Distance(_position, food.Position) <= range)
                    return true;
            }
            return false;
        }

        private Food GetClosestFood(IEnumerable<Food> foods)
        {
            Food closest = null;
            float dist = float.MaxValue;

            foreach (var food in foods)
            {
                float d = Vector2.Distance(_position, food.Position);
                if (d < dist)
                {
                    closest = food;
                    dist = d;
                }
            }

            return closest;
        }

        public void LockState(AggressiveEnemyState state, double duration)
        {
            _currentState = state;
            _stateLockTimer = duration;
            _isStateLocked = true;
        }

        public Collider GetBaseCollider() => _baseCollider;
        public Collider GetMouthCollider() => _mouthCollider;
        public float GetRotation() => _organism_base.GetRotation();

        public void ApplyBounce(Vector2 direction, float strength)
        {
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                _velocity += direction * strength;
                _wasBounced = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_isDead)
                DrawOrganism(spriteBatch, gameTime);
        }

        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            if (_isDead) return;

            int barWidth = 40;
            int barHeight = 5;
            int offsetY = -50;

            float healthPercent = MathHelper.Clamp(_health / 100f, 0f, 1f);
            Vector2 barPosition = _position + new Vector2(-barWidth / 2, offsetY);

            spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
            spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercent), barHeight), Color.Red);
        }
        public void ApplySlow(float duration)
        {
            if (!_isSlowed)
            {
                _isSlowed = true;
                _speed *= 0.4f;
                _slowTimer = duration;
            }
        }


        public Vector2 Position => _position;
        public int Health => _health;

        public void SetPosition(Vector2 pos)
        {
            _position = pos;
            _physics._positon = pos;
            _organism_base.SetPosition(pos);
            _baseCollider.Position = pos;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        public void SetHealth(int hp) => _health = hp;
    }
}