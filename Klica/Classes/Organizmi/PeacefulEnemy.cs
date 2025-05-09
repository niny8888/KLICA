using System;
using System.Linq;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Organizmi
{
    public enum PeacefulEnemyState
    {
        Idle,
        ChasingFood,
        Fleeing,
        Dying
    }

    public class PeacefulEnemy : OrganismBuilder
    {
        private PeacefulEnemyState _currentState;
        private Vector2 _velocity;
        private Vector2 _targetPosition;
        private float _speed;
        private Random _random;
        private double _stateLockTimer;
        private bool _isStateLocked;

        private Collider _baseCollider;
        private Collider _mouthCollider;
        public float Mass { get; private set; } = 5f;
        public float Restitution { get; private set; } = 0.6f;
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        private bool _isDead = false;
        private bool _hasDroppedFood = false;

        private double _deathTimer = 1.0;
        private float _deathRotation = 0f;
        public double _damageCooldown = 0;


        public PeacefulEnemy(Base baseSprite, Eyes eye, Mouth mouth)
            : base(baseSprite, eye, mouth, null)
        {
            _currentState = PeacefulEnemyState.Idle;
            _random = new Random();
            _position = new Vector2(_random.Next(100, 1700), _random.Next(100, 950));
            _speed = 0.4f;
            _targetPosition = _position;
            _health = 100;

            _organism_base.SetPosition(_position);
            _organism_mouth.SetPosition(_organism_base._position_mouth, 0, 0);
            _organism_eye.SetPosition(_organism_base._position_eyes);

            _baseCollider = new Collider(_position, baseSprite.Width / 2f, this);
            _mouthCollider = new Collider(baseSprite._position_mouth, 10f, this);
        }

        public void Update(GameTime gameTime, List<PeacefulEnemy> peacefulEnemies, PhysicsEngine physicsEngine, Player player, List<Enemy> enemies)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_damageCooldown > 0)
                _damageCooldown -= dt;

            if (_isDead)
            {
                _deathTimer -= dt;
                _deathRotation += (float)(Math.PI * dt * 4);
                _organism_base.SetRotation(_deathRotation);

                if (_deathTimer <= 0 && !_hasDroppedFood)
                {
                    physicsEngine.AddFood(new Food(_position, new Vector2(0.5f, 0.5f), 1f));
                    physicsEngine.AddFood(new Food(_position + new Vector2(10, 10), new Vector2(-0.5f, -0.5f), 1f));
                    _hasDroppedFood = true;
                }
                return;
            }

            if (_health <= 0 && _currentState != PeacefulEnemyState.Dying)
            {
                _currentState = PeacefulEnemyState.Dying;
                _isDead = true;
                return;
            }

            if (_isStateLocked)
            {
                _stateLockTimer -= dt;
                if (_stateLockTimer <= 0)
                    _isStateLocked = false;
                else
                    return;
            }

            float fleeRadius = 200f;
            Vector2 fleeVector = Vector2.Zero;
            bool threatDetected = false;

            if (Vector2.Distance(_position, player._position) < fleeRadius)
            {
                fleeVector += (_position - player._position);
                threatDetected = true;
            }

            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    float dist = Vector2.Distance(_position, enemy._position);
                    if (dist < fleeRadius)
                    {
                        fleeVector += (_position - enemy._position);
                        threatDetected = true;
                    }
                }
            }

            if (threatDetected)
            {
                _currentState = PeacefulEnemyState.Fleeing;
                _targetPosition = _position + fleeVector;
            }
            else if (_currentState == PeacefulEnemyState.Fleeing)
            {
                _currentState = PeacefulEnemyState.Idle;
            }

            Vector2 separation = Vector2.Zero;
            int neighborCount = 0;
            foreach (var other in peacefulEnemies)
            {
                if (other == this) continue;

                float dist = Vector2.Distance(_position, other._position);
                if (dist < 50f && dist > 0)
                {
                    separation += (_position - other._position) / dist;
                    neighborCount++;
                }
            }

            if (neighborCount > 0)
            {
                separation /= neighborCount;
                separation.Normalize();
                separation *= _speed * 0.5f;
                _velocity += separation;
            }

            Vector2 movementDirection = Vector2.Zero;
            switch (_currentState)
            {
                case PeacefulEnemyState.Idle:
                    movementDirection = UpdateIdleState(physicsEngine._foodItems.ToArray());
                    break;
                case PeacefulEnemyState.ChasingFood:
                    movementDirection = UpdateChasingFoodState(physicsEngine._foodItems.ToArray());
                    break;
                case PeacefulEnemyState.Fleeing:
                    movementDirection = _targetPosition - _position;
                    break;
            }

            if (movementDirection != Vector2.Zero)
            {
                Vector2 steering = Seek(_position + movementDirection);
                _velocity += steering;
            }

            _velocity *= 0.95f;
            if (_velocity.Length() > _speed)
                _velocity = Vector2.Normalize(_velocity) * _speed;

            _physics.Update(_velocity);
            UpdateOrganism(gameTime);
            _position = _organism_base.GetPosition();
            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        private Vector2 UpdateIdleState(Food[] foods)
        {
            if (_random.NextDouble() < 0.02)
                _targetPosition = GetRandomTargetPosition();

            if (IsFoodInRange(foods, 30f))
                _currentState = PeacefulEnemyState.ChasingFood;

            return _targetPosition - _position;
        }

        private Vector2 UpdateChasingFoodState(Food[] foods)
        {
            Food closestFood = GetClosestFood(foods);
            if (closestFood != null)
            {
                _targetPosition = closestFood.Position;

                if (Vector2.Distance(_position, closestFood.Position) < 15f)
                {
                    closestFood.OnConsumedByAI();
                    LockState(PeacefulEnemyState.Idle, 2.0);
                }
            }
            else
            {
                _currentState = PeacefulEnemyState.Idle;
            }

            return _targetPosition - _position;
        }

        private Vector2 Seek(Vector2 target)
        {
            Vector2 desired = target - _position;
            if (desired != Vector2.Zero)
                desired.Normalize();

            desired *= _speed;
            return (desired - _velocity) * 0.5f;
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
            Food closestFood = null;
            float closestDist = float.MaxValue;

            foreach (var food in foods)
            {
                float dist = Vector2.Distance(_position, food.Position);
                if (dist < closestDist)
                {
                    closestFood = food;
                    closestDist = dist;
                }
            }
            return closestFood;
        }

        public void LockState(PeacefulEnemyState state, double duration)
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
            _velocity += direction * strength;
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
            spriteBatch.Draw(TextureGenerator.Pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercent), barHeight), Color.LimeGreen);
        }
        public Vector2 Position => _position;
        public int Health => _health;
        public void SetPosition(Vector2 position)
        {
            _position = position;
            _organism_base.SetPosition(position);
        }

        public void SetHealth(int health)
        {
            _health = health;
        }

    }
}
