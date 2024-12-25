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

        public Enemy(Base baseSprite, Eyes eye, Mouth mouth, int aggressionLevel)
            : base(baseSprite, eye, mouth)
        {
            this.aggressionLevel = aggressionLevel;
            this.currentState = EnemyState.Idle;
            this.random = new Random();
        }


        public void Update(GameTime gameTime, Player player, Food[] foods)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    MoveRandomly(gameTime);
                    CheckForFood(foods);
                    CheckForPlayer(player);
                    break;
                case EnemyState.ChasingFood:
                    ChaseFood(foods);
                    break;
                case EnemyState.ChasingPlayer:
                    ChasePlayer(player);
                    break;
                case EnemyState.Fleeing:
                    FleeFromPlayer(player);
                    break;
            }
        }

        private void MoveRandomly(GameTime gameTime)
        {
            // Random small movement in any direction
            Vector2 randomDirection = new Vector2((float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5));
            randomDirection.Normalize();
            _position += randomDirection * 2f; // Adjust speed as needed

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

        private void ChaseFood(Food[] foods)
        {
            foreach (var food in foods)
            {
                if (IsInRange(food.Position, 10f)) // Close enough to eat
                {
                    Eat(food);
                    currentState = EnemyState.Idle;
                    return;
                }
                else
                {
                    MoveTo(food.Position);
                }
            }
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

        private void ChasePlayer(Player player)
        {
            if (IsInRange(player._position, 20f)) // Close enough to attack
            {
                DealDamage(player);
            }
            else if (IsInRange(player._position, 200f)) // Still within chase range
            {
                MoveTo(player._position);
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }

        private void FleeFromPlayer(Player player)
        {
            Vector2 fleeDirection = (_position - player._position);
            fleeDirection.Normalize();
            _position += fleeDirection * 2f; // Move away

            fleeTimer++;
            if (fleeTimer > 100) // Flee for a limited time
            {
                currentState = EnemyState.Idle;
                fleeTimer = 0;
            }
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
