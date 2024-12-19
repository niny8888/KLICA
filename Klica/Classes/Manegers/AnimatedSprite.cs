using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Klica.Classes
{
    public class AnimatedSprite : Sprite
    {
        private List<Sprite> _frames; 
        private int _currentFrameIndex;
        private float _frameTime; 
        private float _timeSinceLastFrame;
        private bool _isPlaying; 
        private bool _isReversing;

        public AnimatedSprite(
            List<Sprite> frames,
            float frameTime = 0.1f 
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
            _isReversing = false;

            
            UpdateFrameProperties();
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPlaying || _frames == null || _frames.Count <= 1)
                 return;

            _timeSinceLastFrame += 0.01f;

            if (_timeSinceLastFrame >= _frameTime)
            {
                _timeSinceLastFrame -= _frameTime;

                
                if (_isReversing)
                {
                    _currentFrameIndex--;
                    if (_currentFrameIndex <= 0)
                    {
                        _currentFrameIndex = 0;
                        _isReversing = false; 
                    }
                }
                else
                {
                    _currentFrameIndex++;
                    if (_currentFrameIndex >= _frames.Count - 1)
                    {
                        _currentFrameIndex = _frames.Count - 1;
                        _isReversing = true;
                    }
                }

                UpdateFrameProperties();
            }
        }

        private void UpdateFrameProperties()
        {
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
