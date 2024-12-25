using System;
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
        private EnemyState currentState;
        private int aggressionLevel;
        private int fleeTimer;
        public Vector2 _position { get; internal set; }
        private Random random;
        private PhysicsEngine _physicsEngine;
        private Vector2 _currentIdleDirection;  // Current direction when in Idle state
        private float _idleDirectionChangeTimer;
        private float _idleDirectionChangeInterval = 10.0f; // Change direction every 1 second
        private float _idleWaitTime; // Time to wait before changing idle direction again
        private float _idleWaitThreshold = 2.0f; // Wait for 2 seconds before changing direction again
        
        public Enemy(Base baseSprite, Eyes eye, Mouth mouth, int aggressionLevel, PhysicsEngine physicsEngine)
            : base(baseSprite, eye, mouth,physicsEngine)
        {
            this.aggressionLevel = aggressionLevel;
            this.currentState = EnemyState.Idle;
            this.random = new Random();
            _position = new Vector2(random.Next(100, 800), random.Next(100, 600));
            _currentIdleDirection = Vector2.Zero;
            _idleWaitTime = 0f;
        
        }


        public void Update(GameTime gameTime, Player player, Food[] foods)
        {
            Vector2 movementDirection = Vector2.Zero;

            // Update state-specific behavior
            switch (currentState)
            {
                case EnemyState.Idle:
                    _idleDirectionChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _idleWaitTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Change direction only after a certain interval or if idle wait time exceeds threshold
                    if (_idleDirectionChangeTimer >= _idleDirectionChangeInterval || _idleWaitTime >= _idleWaitThreshold)
                    {
                        _currentIdleDirection = GetRandomMovementDirection();
                        _idleDirectionChangeTimer = 0f; // Reset the timer
                        _idleWaitTime = 0f; // Reset idle wait time
                    }

                    movementDirection = _currentIdleDirection;
                    CheckForFood(foods);
                    CheckForPlayer(player);
                    break;

                case EnemyState.ChasingFood:
                    movementDirection = GetFoodChaseDirection(foods);
                    break;

                case EnemyState.ChasingPlayer:
                    movementDirection = GetPlayerChaseDirection(player);
                    break;

                case EnemyState.Fleeing:
                    movementDirection = GetFleeDirection(player);
                    break;
            }
            System.Console.WriteLine("Enemy state: " + currentState);
            System.Console.WriteLine("Enemy direction: " + movementDirection);
            // Call the base method to update organism properties
            UpdateOrganism(movementDirection, gameTime);
        }

        private Vector2 GetRandomMovementDirection()
        {
            Vector2 randomDirection = new Vector2((float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5));
            randomDirection.Normalize();
            return randomDirection * 2f; // Adjust speed as needed
        }


        private void CheckForFood(Food[] foods)
        {
            foreach (var food in foods)
            {
                if (IsInRange(food.Position, 100f)) // Example range
                {
                    currentState = EnemyState.ChasingFood;
                    break;
                }
            }
        }

       private Vector2 GetFoodChaseDirection(Food[] foods)
        {
            foreach (var food in foods)
            {
                if (IsInRange(food.Position, 10f)) // Close enough to eat
                {
                    Eat(food);
                    currentState = EnemyState.Idle;
                    return Vector2.Zero;
                }
                else if (IsInRange(food.Position, 100f))
                {
                    return Vector2.Normalize(food.Position - _position) * 2f;
                }
            }
            currentState = EnemyState.Idle; // If no food is close, return to Idle
            return Vector2.Zero;
        }

        private void CheckForPlayer(Player player)
        {
            if (IsInRange(player._position, 150f)) // Example detection range
            {
                if (random.Next(100) < aggressionLevel)
                {
                    currentState = EnemyState.ChasingPlayer;
                }
                else
                {
                    currentState = EnemyState.Fleeing;
                }
            }
        }

        private Vector2 GetPlayerChaseDirection(Player player)
        {
            if (IsInRange(player._position, 20f)) // Close enough to attack
            {
                DealDamage(player);
                return Vector2.Zero;
            }
            else if (IsInRange(player._position, 200f)) // Still within chase range
            {
                return Vector2.Normalize(player._position - _position) * 2f;
            }
            else
            {
                currentState = EnemyState.Idle;
                return Vector2.Zero;
            }
        }

        private Vector2 GetFleeDirection(Player player)
        {
            Vector2 fleeDirection = (_position - player._position);
            fleeDirection.Normalize();

            fleeTimer++;
            if (fleeTimer > 100) // Flee for a limited time
            {
                currentState = EnemyState.Idle;
                fleeTimer = 0;
            }

            return fleeDirection * 2f;
        }

        private bool IsInRange(Vector2 targetPosition, float range)
        {
            return Vector2.Distance(_position, targetPosition) <= range;
        }

        private void MoveTo(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - _position;
            direction.Normalize();
            _position += direction * 2f; // Example speed
        }

        private void Eat(Food food)
        {
            //food.IsConsumed = true;
        }

        private void DealDamage(Player player)
        {
            player.TakeDamage(10);
        }
        public void Draw(SpriteBatch _spriteBatch, GameTime _gameTime)
        {
            DrawOrganism(_spriteBatch, _gameTime);
        }
    }
}
