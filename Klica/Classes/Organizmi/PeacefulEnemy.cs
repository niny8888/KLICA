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

        private Collider _baseCollider;
        private Collider _mouthCollider;
        public float Mass { get; private set; } = 3f;
        public float Restitution { get; private set; } = 0.6f;
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public PeacefulEnemy(Base baseSprite, Eyes eye, Mouth mouth)
            : base(baseSprite, eye, mouth, null) 
        {
            _currentState = PeacefulEnemyState.Idle;
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
            _baseCollider = new Collider(_position, baseSprite.Width / 2f, this);
            _mouthCollider = new Collider(baseSprite._position_mouth, 10f, this);
        }

        public void Update(GameTime gameTime, PhysicsEngine physicsEngine)
        {
            // Apply velocity (bouncing effect)
            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _velocity *= 0.95f; // Apply friction

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

            if (movementDirection != Vector2.Zero)
            {
                UpdateOrganism(movementDirection, gameTime);
            }

            // Update colliders
            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        private Vector2 UpdateIdleState(Food[] foods)
        {
            if (_random.NextDouble() < 0.01)
            {
                _targetPosition = GetRandomTargetPosition();
            }

            if (IsFoodInRange(foods, 100f))
            {
                _currentState = PeacefulEnemyState.ChasingFood;
            }

            return _targetPosition - _position;
        }

        private Vector2 UpdateChasingFoodState(Food[] foods)
        {
            Food closestFood = GetClosestFood(foods);
            if (closestFood != null)
            {
                _targetPosition = closestFood.Position;

                if (Vector2.Distance(_position, closestFood.Position) < 10f)
                {
                    closestFood.OnConsumedByAI();
                    _currentState = PeacefulEnemyState.Idle;
                }
            }
            else
            {
                _currentState = PeacefulEnemyState.Idle;
            }

            return _targetPosition - _position;
        }

        private Vector2 GetRandomTargetPosition()
        {
            return new Vector2(
                _random.Next(100, 800),
                _random.Next(100, 600)
            );
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

        public Collider GetBaseCollider() => _baseCollider;
        public Collider GetMouthCollider() => _mouthCollider;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawOrganism(spriteBatch, gameTime);
        }

        public float GetRotation()
        {
            return _organism_base.GetRotation();
        }

        public void ApplyBounce(Vector2 direction, float strength)
        {
            _velocity += direction * strength;
        }
    }
}
