using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica.Classes.Organizmi
{
    public enum EnemyState
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

        private Collider _baseCollider;
        private Collider _mouthCollider;

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

            // Initialize components in base class
            _organism_base.SetPosition(_position);
            _organism_mouth.SetPosition(_organism_base._position_mouth, 0, 0);
            _organism_eye.SetPosition(_organism_base._position_eyes);

            // Initialize colliders
            _baseCollider = new Collider(_position, baseSprite.Width / 2f, this);
            _mouthCollider = new Collider(baseSprite._position_mouth, 25f, this);
        }


        public void Update(GameTime gameTime, Player player, PhysicsEngine physicsEngine,ref int score)
        {
            Vector2 movementDirection = Vector2.Zero;
            switch (_currentState)
            {
                case EnemyState.Idle:
                    movementDirection= UpdateIdleState(player, physicsEngine._foodItems.ToArray());
                    break;
                case EnemyState.ChasingFood:
                    movementDirection=UpdateChasingFoodState(physicsEngine._foodItems.ToArray(), ref score);
                    break;
                case EnemyState.ChasingPlayer:
                    movementDirection=UpdateChasingPlayerState(player);
                    break;
                case EnemyState.Fleeing:
                    movementDirection=UpdateFleeingState(player);
                    break;
            }
            if(movementDirection != Vector2.Zero)
            {
                //System.Console.WriteLine("movement dir: " + movementDirection);
                UpdateOrganism(movementDirection, gameTime);
            }
            Console.WriteLine($"Enemy in state {_currentState}");
            // Update colliders
            
            _baseCollider.Position = _position;
            _mouthCollider.Position = _organism_base._position_mouth;
        }

        private Vector2 UpdateIdleState(Player player, Food[] foods)
        {
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

        private Vector2 UpdateChasingFoodState(Food[] foods, ref int score)
        {
            Food closestFood = GetClosestFood(foods);
            if (closestFood != null)
            {
                _targetPosition = closestFood.Position;

                if (Vector2.Distance(_position, closestFood.Position) < 10f)
                {
                    closestFood.OnConsumed(ref score);
                    _currentState = EnemyState.Idle;
                }
            }
            else
            {
                _currentState = EnemyState.Idle;
            }

            return _targetPosition - _position;
        }

        private Vector2 UpdateChasingPlayerState(Player player)
        {
            _targetPosition = player._position;

            if (Vector2.Distance(_position, player._position) < 20f)
            {
                player.TakeDamage(10);
                _currentState = EnemyState.Idle;
            }

            return _targetPosition - _position;
        }

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

        private Vector2 GetRandomTargetPosition()
        {
            return new Vector2(
                _random.Next(100, 800),
                _random.Next(100, 600)
            );
        }

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

        public Collider GetBaseCollider() => _baseCollider;

        public Collider GetMouthCollider() => _mouthCollider;

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawOrganism(spriteBatch, gameTime);
        }
    }
}
