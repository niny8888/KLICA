using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Organizmi
{
    public enum PeacefulEnemyState
    {
        Idle,
        ChasingFood
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

        public void Update(GameTime gameTime, PhysicsEngine physicsEngine)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Console.WriteLine($"PeacefulEnemy speed: {_velocity.Length():0.000}");

            // Lock check
            if (_isStateLocked)
            {
                _stateLockTimer -= dt;
                if (_stateLockTimer <= 0)
                    _isStateLocked = false;
                else
                    return;
            }

            // Decide direction
            Vector2 movementDirection = Vector2.Zero;
            switch (_currentState)
            {
                case PeacefulEnemyState.Idle:
                    movementDirection = UpdateIdleState(physicsEngine._foodItems.ToArray());
                    break;
                case PeacefulEnemyState.ChasingFood:
                    movementDirection = UpdateChasingFoodState(physicsEngine._foodItems.ToArray());
                    break;
            }

            // Movement physics
            if (movementDirection != Vector2.Zero)
            {
                Vector2 steering = Seek(_position + movementDirection);
                _velocity += steering;
            }

            _velocity *= 0.95f; // friction
            if (_velocity.Length() > _speed)
                _velocity = Vector2.Normalize(_velocity) * _speed;
            _physics.Update(_velocity);
            UpdateOrganism(gameTime);
            _position = _organism_base.GetPosition(); // make sure it's synced

            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }


        private Vector2 UpdateIdleState(Food[] foods)
        {
            if (_random.NextDouble() < 0.02) // More frequent but subtle wandering
                _targetPosition = GetRandomTargetPosition();

            if (IsFoodInRange(foods, 30f)) // Less sensitive
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
                    LockState(PeacefulEnemyState.Idle, 2.0); // Pause before next move
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

            // Dampen steering to feel smoother
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
            DrawOrganism(spriteBatch, gameTime);
        }
    }
}
