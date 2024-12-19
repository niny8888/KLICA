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
        public float Speed { get; private set; }
        public Vector2 Direction { get; private set; }
        public float CollisionRadius { get; private set; } = 20f;
        public bool IsConsumed { get; private set; } = false;
        private float _vibrationDuration = 0f; 

        

        
        public Food( Vector2 position, Vector2 direction, float speed)
        {
            _sprite = _spriteManager.GetSprite("food");
            Position = position;
            Direction = direction;
            Speed = speed;
            _sprite= _spriteManager.GetSprite("food");
        }

        public void Update(GameTime gameTime, Rectangle levelBounds, Vector2 playerMouthPosition, ref int score)
        {
            // Move food
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle bouncing off boundaries
            if (Position.X < levelBounds.Left || Position.X > levelBounds.Right)
            {
                Direction = new Vector2(-Direction.X, Direction.Y);
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, levelBounds.Left, levelBounds.Right),
                    Position.Y
                );
            }

            if (Position.Y < levelBounds.Top || Position.Y > levelBounds.Bottom)
            {
                Direction = new Vector2(Direction.X, -Direction.Y);
                Position = new Vector2(
                    Position.X,
                    MathHelper.Clamp(Position.Y, levelBounds.Top, levelBounds.Bottom)
                );
            }

            // Check for proximity to player's mouth
            if (!IsConsumed && Vector2.Distance(Position, playerMouthPosition) <= CollisionRadius)
            {
                OnConsumed(ref score);
            }

            // Handle vibration
            if (_vibrationDuration > 0)
            {
                _vibrationDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_vibrationDuration <= 0)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f); // Stop vibration
                }
            }
        }

        
        public void OnConsumed(ref int score)
        {
            IsConsumed = true;
            score++;
            _vibrationDuration = 0.5f;
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
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
