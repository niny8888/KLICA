using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Klica.Classes
{
    public class AnimatedSprite : Sprite
    {
        private List<Sprite> _frames; // List of sprites for each animation frame
        private int _currentFrameIndex; // Index of the current frame
        private float _frameTime; // Time per frame in seconds
        private float _timeSinceLastFrame; // Accumulated time since last frame
        private bool _isPlaying; // Whether the animation is playing

        public AnimatedSprite(
            List<Sprite> frames,
            float frameTime = 0.1f // Default to 10 frames per second
        ) : base(
            frames[0].Texture,
            frames[0].Position,
            frames[0].SourceRectangle,
            frames[0].Scale,
            frames[0].Rotation,
            frames[0].Origin,
            frames[0].Tint
        )
        {
            _frames = frames;
            _currentFrameIndex = 0;
            _frameTime = frameTime;
            _timeSinceLastFrame = 0f;
            _isPlaying = true;

            // Set initial values based on the first frame
            UpdateFrameProperties();
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPlaying || _frames.Count <= 1)
                return;

            _timeSinceLastFrame += 0.5f;///(float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastFrame >= _frameTime)
            {
                _timeSinceLastFrame -= _frameTime; // Reset frame timer
                _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Count; // Loop through frames
                UpdateFrameProperties();
            }
        }

        private void UpdateFrameProperties()
        {
            // Update the base sprite's properties to match the current frame
            var currentFrame = _frames[_currentFrameIndex];
            _texture = currentFrame.Texture;
            _position = currentFrame.Position;
            _sourceRectangle = currentFrame.SourceRectangle;
            _scale = currentFrame.Scale;
            _rotation = currentFrame.Rotation;
            _origin = currentFrame.Origin;
            _tint = currentFrame.Tint;
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Stop()
        {
            _isPlaying = false;
            _currentFrameIndex = 0;
            UpdateFrameProperties();
        }

        public void SetFrameTime(float frameTime)
        {
            _frameTime = frameTime;
        }

        public int CurrentFrame => _currentFrameIndex;
    }
}
