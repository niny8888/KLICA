using System;
using Klica;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes
{
    public class Food
    {
        private SpriteManager _spriteManager = SpriteManager.getInstance();

        public static Sprite _sprite;
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public Vector2 InitialVelocity { get; private set; } 
        public float Speed { get; private set; }
        public Vector2 Direction { get; private set; }
        public float CollisionRadius { get; private set; } = 10f;
        public bool IsConsumed { get; private set; } = false;

        public float Mass { get; set; } = 1f; // Default mass
        public float Restitution { get;  set; } = 0.8f; // Default restitution coefficient
        private const float MaxVelocity = 300f;
        private const float MinVelocity = 5f;
        private float _vibrationDuration = 0f;

        public Food(Vector2 position, Vector2 direction, float speed)
        {
            _sprite = _spriteManager.GetSprite("food").Clone();
            Position = position;
            Direction = direction;
            Direction.Normalize(); // Ensure direction is normalized
            Speed = speed;
            Velocity = Direction * Speed;
            InitialVelocity = Velocity;
        }

        public void Update(GameTime gameTime, Rectangle levelBounds, Vector2 playerMouthPosition, ref int score)
        {
            // Move the food
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply friction to slow down velocity slightly
            Velocity *= 0.95f;

            // Ensure velocity does not fall below a minimum threshold
            if (Velocity.Length() < InitialVelocity.Length())
            {
                Velocity = Vector2.Normalize(Velocity != Vector2.Zero ? Velocity : Direction) * InitialVelocity.Length();
            }

            // Cap the velocity to avoid excessive speed
            if (Velocity.Length() > MaxVelocity)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxVelocity;
            }

            // Handle boundary collisions
            if (Position.X - CollisionRadius < levelBounds.Left || Position.X + CollisionRadius > levelBounds.Right)
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse X velocity
                Direction = new Vector2(-Direction.X, Direction.Y); // Reverse X direction
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, levelBounds.Left + CollisionRadius, levelBounds.Right - CollisionRadius),
                    Position.Y
                );
            }

            if (Position.Y - CollisionRadius < levelBounds.Top || Position.Y + CollisionRadius > levelBounds.Bottom)
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse Y velocity
                Direction = new Vector2(Direction.X, -Direction.Y); // Reverse Y direction
                Position = new Vector2(
                    Position.X,
                    MathHelper.Clamp(Position.Y, levelBounds.Top + CollisionRadius, levelBounds.Bottom - CollisionRadius)
                );
            }

            // Check for proximity to the player's mouth
            if (!IsConsumed && Vector2.Distance(Position, playerMouthPosition) <= CollisionRadius)
            {
                Console.WriteLine("Food consumed");
                Console.WriteLine("Score: " + score);
                OnConsumed(ref score);
            }

            // Handle vibration (optional feature)
            if (_vibrationDuration > 0)
            {
                _vibrationDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_vibrationDuration <= 0)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f); // Stop vibration
                }
            }
        }


        public void HandleCollision(Food other)
        {
            // Calculate collision normal
            Vector2 normal = Vector2.Normalize(Position - other.Position);

            // Relative velocity
            Vector2 relativeVelocity = Velocity - other.Velocity;

            // Velocity along the normal
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // If objects are separating, skip collision
            if (velocityAlongNormal > 0) return;

            // Combined restitution coefficient
            float combinedRestitution = Restitution * other.Restitution;

            // Inverse masses
            float invMass1 = Mass > 0 ? 1 / Mass : 0;
            float invMass2 = other.Mass > 0 ? 1 / other.Mass : 0;

            // Impulse scalar
            float impulse = -(1 + combinedRestitution) * velocityAlongNormal;
            impulse /= invMass1 + invMass2;

            // Apply impulse
            Vector2 impulseVector = impulse * normal;
            Velocity += impulseVector * invMass1;
            other.Velocity -= impulseVector * invMass2;

            // Cap velocities
            if (Velocity.Length() > MaxVelocity)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxVelocity;
            }

            if (other.Velocity.Length() > MaxVelocity)
            {
                other.Velocity = Vector2.Normalize(other.Velocity) * MaxVelocity;
            }
        }

        public void OnConsumed(ref int score)
        {
            IsConsumed = true;
            score++;

            Console.WriteLine("Food consumed! Score incremented.");
            _vibrationDuration = 0.5f;
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }

        public bool WasConsumed()
        {
            return IsConsumed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsConsumed)
            {
                _sprite._position = Position;
                _sprite.Draw(spriteBatch);
            }
        }
    }
}
