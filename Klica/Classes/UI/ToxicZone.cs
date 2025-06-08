using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;


namespace Klica.Classes.Environment
{
    public class ToxicZone
    {
        private Texture2D _texture;
        private Vector2 _position;
        private float _radius;
        private List<ToxicParticle> _particles = new();
        private Random _random = new();
        private float _damagePerSecond = 20f;
        private float _damageBuffer = 0f;


        public ToxicZone(Texture2D texture, Vector2 position, float radius)
        {
            _texture = texture;
            _position = position;
            _radius = radius;

            for (int i = 0; i < 20; i++)
                _particles.Add(GenerateParticle());
        }

        private ToxicParticle GenerateParticle()
        {
            return new ToxicParticle
            {
                Position = _position + new Vector2(_random.NextFloat(-_radius, _radius), _random.NextFloat(-_radius, _radius)),
                Velocity = new Vector2(_random.NextFloat(-10, 10), _random.NextFloat(-10, 10)),
                Life = _random.NextFloat(1f, 3f)
            };
        }

        public void Update(GameTime gameTime, Player player)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Console.WriteLine($"ToxicZone Update: dt={dt}, playerPos={player._position}, zonePos={_position}, radius={_radius}");

            if (Vector2.Distance(player._position, _position) < _radius)
            {
                float damageThisFrame = _damagePerSecond * dt;
                _damageBuffer += damageThisFrame;
                //Console.WriteLine($"Player in toxic zone! Accumulated Damage Buffer: {_damageBuffer}");

                if (_damageBuffer >= 1f)
                {
                    int wholeDamage = (int)_damageBuffer;
                    player.TakeDamage(wholeDamage);
                    //Console.WriteLine($"--> Damage applied: {wholeDamage}, HP now: {player._health}");
                    _damageBuffer -= wholeDamage;
                }
            }

            // Update particles
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var p = _particles[i];
                p.Update(dt);
                if (p.Life <= 0)
                    _particles[i] = GenerateParticle(); // regenerate
            }
        }


        public void Draw(SpriteBatch sb, Texture2D particleTex)
        {
            sb.Draw(_texture, _position, null, Color.LimeGreen * 0.4f, 0f,
                    new Vector2(_texture.Width, _texture.Height) / 2f,
                    _radius * 2 / _texture.Width, SpriteEffects.None, 0f);

            foreach (var p in _particles)
                sb.Draw(particleTex, p.Position, null, Color.Green * 0.2f, 0f,
                        new Vector2(particleTex.Width, particleTex.Height) / 2f,
                        0.6f, SpriteEffects.None, 0f);
        }
    }

    public class ToxicParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Life;

        public void Update(float dt)
        {
            Position += Velocity * dt;
            Life -= dt;
        }
    }

    public static class RandExtensions
    {
        public static float NextFloat(this Random r, float min, float max)
        {
            return (float)(r.NextDouble() * (max - min) + min);
        }
    }
}
