using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Organizmi
{
    public enum EnemyState //AI STATES
    {
        Idle,
        ChasingFood,
        ChasingPlayer,
        Fleeing
    }

    public class Enemy : OrganismBuilder
    {
        private EnemyState _currentState;
        private int _aggressionLevel;
        private int _fleeTimer;
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

        public Enemy(Base baseSprite, Eyes eye, Mouth mouth, int aggressionLevel)
    : base(baseSprite, eye, mouth, null) 
        {
            _aggressionLevel = aggressionLevel;
            _currentState = EnemyState.Idle;
            _random = new Random();
            _position = new Vector2(_random.Next(100, 800), _random.Next(100, 600));
            _speed = 2f;
            _targetPosition = _position;
            _health = 100;
            Mass = 3f;
            

            // Initialize components in base class
            _organism_base.SetPosition(_position);
            _organism_mouth.SetPosition(_organism_base._position_mouth, 0, 0);
            _organism_eye.SetPosition(_organism_base._position_eyes);

            // Initialize colliders
            _baseCollider = new Collider(_position, baseSprite.Width/2f, this);
            _mouthCollider = new Collider(baseSprite._position_mouth, 10f, this);
        
            _isStateLocked = false;
            _stateLockTimer = 0;
        }

// ==============================================
// ============== UPDATE  =================
// ==============================================
        public void Update(GameTime gameTime, Player player, PhysicsEngine physicsEngine)
        {
            // Handle state lock timer
             if (_isStateLocked)
            {
                _stateLockTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_stateLockTimer <= 0)
                {
                    _isStateLocked = false;
                }
                return; // Stop further state updates
            }
            // Apply velocity (bouncing effect)
            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply friction to slow down bounce velocity
            _velocity *= 0.95f;

            Vector2 movementDirection = Vector2.Zero;

            // Existing behavior logic
            switch (_currentState)
            {
                case EnemyState.Idle:
                    movementDirection = UpdateIdleState(player, physicsEngine._foodItems.ToArray());
                    break;
                case EnemyState.ChasingFood:
                    movementDirection = UpdateChasingFoodState(physicsEngine._foodItems.ToArray());
                    break;
                case EnemyState.ChasingPlayer:
                    movementDirection = UpdateChasingPlayerState(player);
                    break;
                case EnemyState.Fleeing:
                    movementDirection = UpdateFleeingState(player);
                    break;
            }

            if (movementDirection != Vector2.Zero)
            {
                UpdateOrganism(movementDirection, gameTime);
            }

            // Update colliders
            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        // ==============================================
        // ============== IDLE  =================
        // ==============================================

        private Vector2 UpdateIdleState(Player player, Food[] foods)
        {
            ///Wander(); ///to so pol tko bl na enmu kupcku
            if (_random.NextDouble() < 0.01)
            {
                _targetPosition = GetRandomTargetPosition();
            }

            if (IsPlayerInRange(player, 150f))
            {
                _currentState = _random.Next(100) < _aggressionLevel ? EnemyState.ChasingPlayer : EnemyState.Fleeing;
            }
            else if (IsFoodInRange(foods, 100f))
            {
                _currentState = EnemyState.ChasingFood;
            }

            return _targetPosition - _position;
        }

        // ==============================================
        // ============== CHASING FOOD =================
        // ==============================================
        private Vector2 UpdateChasingFoodState(Food[] foods)
        {
            Food closestFood = GetClosestFood(foods);
            if (closestFood != null)
            {
                _targetPosition = closestFood.Position;

                if (Vector2.Distance(_position, closestFood.Position) < 10f)
                {
                    closestFood.OnConsumedByAI();
                    _currentState = EnemyState.Idle;
                }
            }
            else
            {
                _currentState = EnemyState.Idle;
            }

            return _targetPosition - _position;
        }

        // ==============================================
        // ============== CHASING PLAYER  =================
        // ==============================================


        private Vector2 Seek(Vector2 target)
        {
            Vector2 desiredVelocity = target - _position;
            desiredVelocity.Normalize();
            desiredVelocity *= _speed;
            return desiredVelocity - _velocity; // Steering force
        }

        private Vector2 UpdateChasingPlayerState(Player player)
        {
            Vector2 steering = Seek(player._position);
            _velocity += steering;
            _velocity = Vector2.Clamp(_velocity, new Vector2(-_speed, -_speed), new Vector2(_speed, _speed));
            if (Vector2.Distance(_position, player._position) < 20f)
            {
                _currentState = EnemyState.Idle;
            }
            return _velocity;
        }

        // ==============================================
        // ============== FLEEING  =================
        // ==============================================
        private Vector2 UpdateFleeingState(Player player)
        {
            Vector2 directionAwayFromPlayer = _position - player._position;
            directionAwayFromPlayer.Normalize();
            _targetPosition = _position + directionAwayFromPlayer * 100f;

            _fleeTimer++;
            if (_fleeTimer > 100)
            {
                _currentState = EnemyState.Idle;
                _fleeTimer = 0;
            }

            return _targetPosition - _position;
        }

        // ==============================================
        // ============== TO TARGET  =================
        // ==============================================
        private void MoveTowardsTarget(GameTime gameTime)
        {
            Vector2 direction = _targetPosition - _position;
            if (direction.Length() > 1f)
            {
                direction.Normalize();
                _velocity = direction * _speed;
            }
            else
            {
                _velocity = Vector2.Zero; // Stop when close enough
            }

            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // ==============================================
        // ============== Wander =================
        // ==============================================
        private Vector2 Wander()
        {
            if (_random.NextDouble() < 0.02) // Change direction occasionally
            {
                float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);
                _targetPosition = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50f + _position;
            }
            return Seek(_targetPosition);
        }


// ==============================================
// ============== DRAW  =================
// ==============================================
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawOrganism(spriteBatch, gameTime);
        }

        
// ==============================================
// ============== RANGE CHECK  =================
// ==============================================
        private bool IsPlayerInRange(Player player, float range)
        {
            return Vector2.Distance(_position, player._position) <= range;
        }

        private bool IsFoodInRange(IEnumerable<Food> foods, float range)
        {
            foreach (var food in foods)
            {
                if (Vector2.Distance(_position, food.Position) <= range)
                {
                    return true;
                }
            }
            return false;
        }

        private Food GetClosestFood(IEnumerable<Food> foods)
        {
            Food closestFood = null;
            float closestDistance = float.MaxValue;

            foreach (var food in foods)
            {
                float distance = Vector2.Distance(_position, food.Position);
                if (distance < closestDistance)
                {
                    closestFood = food;
                    closestDistance = distance;
                }
            }

            return closestFood;
        }

// ==============================================
// ============== GETTERS  =================
// ==============================================

        public Collider GetBaseCollider() => _baseCollider;

        public Collider GetMouthCollider() => _mouthCollider;
        private Vector2 GetRandomTargetPosition()
        {
            return new Vector2(
                _random.Next(100, 800),
                _random.Next(100, 600)
            );
        }
        public float GetRotation()
        {
            return _organism_base.GetRotation();
        }


// ==============================================
// ============== FIZKA  =================
// ==============================================
        public void ApplyBounce(Vector2 direction, float strength)
        {
            _velocity += direction * strength;
        }

    
// ===========================================
// STATE LOCK SYSTEM
// ===========================================
        public void LockState(EnemyState state, double duration)
        {
            _currentState = state;
            _stateLockTimer = duration;
            _isStateLocked = true;
        }
    }
}
