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
        public Vector2 Position { get;  set; }
        public Vector2 Velocity { get; set; }
        public Vector2 InitialVelocity { get; private set; } 
        public float Speed { get; private set; }
        public Vector2 Direction { get; private set; }
        public float CollisionRadius { get; private set; } = 10f;
        public bool IsConsumed { get; private set; } = false;

        public float Mass { get; set; } = 1f; // Default mass
        public float Restitution { get;  set; } = 0.8f; 
        private const float MaxVelocity = 300f;
        private const float MinVelocity = 5f;
        private float _vibrationDuration = 0f;

        public Food(Vector2 position, Vector2 direction, float speed)
        {
            _sprite = _spriteManager.GetSprite("food").Clone();
            Position = position;
            Direction = direction;
            Direction.Normalize();
            Speed = speed;
            Velocity = Direction * Speed;
            InitialVelocity = Velocity;
            Mass = 1f;
        }

        public void Update(GameTime gameTime, Rectangle levelBounds, Vector2 playerMouthPosition, ref int score)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Velocity.Length() > InitialVelocity.Length())
            {
                Velocity *= 0.95f;
            }
            // // Ensure velocity does not fall below a minimum threshold
            // if (Velocity.Length() < InitialVelocity.Length())
            // {
            //     Velocity = Vector2.Normalize(Velocity != Vector2.Zero ? Velocity : Direction) * InitialVelocity.Length();
            // }

            // Capam velocity ==> avoid excessive speed
            if (Velocity.Length() > MaxVelocity)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxVelocity;
            }

            // da je u okvirjih
            if (Position.X - CollisionRadius < levelBounds.Left || Position.X + CollisionRadius > levelBounds.Right)
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y); 
                Direction = new Vector2(-Direction.X, Direction.Y); 
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, levelBounds.Left + CollisionRadius, levelBounds.Right - CollisionRadius),
                    Position.Y
                );
            }

            if (Position.Y - CollisionRadius < levelBounds.Top || Position.Y + CollisionRadius > levelBounds.Bottom)
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y); 
                Direction = new Vector2(Direction.X, -Direction.Y); 
                Position = new Vector2(
                    Position.X,
                    MathHelper.Clamp(Position.Y, levelBounds.Top + CollisionRadius, levelBounds.Bottom - CollisionRadius)
                );
            }

            //a je bliz od playerja ustom
            if (!IsConsumed && Vector2.Distance(Position, playerMouthPosition) <= CollisionRadius)
            {
                Console.WriteLine("Food consumed");
                Console.WriteLine("Score: " + score);
                OnConsumed(ref score);
            }

            //vibration ce je game pad povezan
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
            Vector2 normal = Vector2.Normalize(Position - other.Position);
            Vector2 relativeVelocity = Velocity - other.Velocity;
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            //ce seperirata
            if (velocityAlongNormal > 0) return;
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
            _vibrationDuration = 0.5f;
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }

	public void OnConsumedByAI()
        {
            IsConsumed = true;

            Console.WriteLine("Food consumed by AI.");
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