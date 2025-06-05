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

        //slow
        private float _originalSpeed;
        private float _slowTimer = 0f;
        private bool _isSlowed = false;

        /// DRUNKARD
        private float _drunkAngle;
        private float _drunkTurnRate = 0.2f; // How sharply direction changes
        private float _drunkSpeed = 0.3f;


        public PeacefulEnemy(Base baseSprite, Eyes eye, Mouth mouth)
            : base(baseSprite, eye, mouth, null)
        {
            _currentState = PeacefulEnemyState.Idle;
            _random = new Random();
            _position = new Vector2(_random.Next(100, 1700), _random.Next(100, 950));
            _speed = 0.4f;
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

        public void Update(GameTime gameTime, List<PeacefulEnemy> peacefulEnemies, PhysicsEngine physicsEngine, Player player, List<Enemy> enemies)
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
            
            
            if (_damageCooldown > 0)
                _damageCooldown -= dt;

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
            // Clamp position to screen bounds
            float halfWidth = _organism_base.Width / 2f;
            float halfHeight = _organism_base.Height / 2f;

            Vector2 clampedPos = _position;
            bool hitWall = false;

            if (_position.X < halfWidth || _position.X > 1920 - halfWidth)
            {
                _drunkAngle = MathHelper.Pi - _drunkAngle; // Reflect angle horizontally
                hitWall = true;
            }
            if (_position.Y < halfHeight || _position.Y > 1080 - halfHeight)
            {
                _drunkAngle = -_drunkAngle; // Reflect angle vertically
                hitWall = true;
            }

            if (hitWall)
            {
                _drunkAngle += (float)(_random.NextDouble() - 0.5f) * 0.3f; // Add jitter to avoid perfect bounce loops
            }

            // Clamp the final position
            clampedPos.X = MathHelper.Clamp(clampedPos.X, halfWidth, 1920 - halfWidth);
            clampedPos.Y = MathHelper.Clamp(clampedPos.Y, halfHeight, 1080 - halfHeight);

            _position = clampedPos;
            _physics._positon = clampedPos;
            _organism_base.SetPosition(clampedPos);

            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        private Vector2 UpdateIdleState(Food[] foods)
        {
            // Small random angle perturbation
            _drunkAngle += (float)(_random.NextDouble() - 0.5f) * _drunkTurnRate;

            // Get a new direction vector
            Vector2 direction = new Vector2((float)Math.Cos(_drunkAngle), (float)Math.Sin(_drunkAngle));

            // Move forward slightly
            _targetPosition = _position + direction * 50f;

            // Switch to chase if food is near
            if (IsFoodInRange(foods, 30f))
                _currentState = PeacefulEnemyState.ChasingFood;

            return direction * _drunkSpeed;
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
            return new Vector2(_random.Next(10, 1920), _random.Next(10, 1080));
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
        public void SetPosition(Vector2 pos)
        {
            _position = pos;
            _physics._positon = pos;

            _organism_base.SetPosition(pos);
            _baseCollider.Position = pos;
            _mouthCollider.Position = _organism_base._position_mouth;
        }
        public void ApplySlow(float duration)
        {
            if (!_isSlowed)
            {
                _isSlowed = true;
                _speed *= 0.4f; // slow to 40% of normal speed
                _slowTimer = duration;
                Console.WriteLine("Enemy slowed!");
            }
        }


        public void SetHealth(int health)
        {
            _health = health;
        }

    }
}
