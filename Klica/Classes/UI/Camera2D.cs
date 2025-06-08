using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Klica;

namespace Klica.Classes
{
    public class Camera2D
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; private set; }
        public float Zoom { get; set; } = 2f;
        public float Rotation { get; set; } = 0f;

        private int _viewportWidth;
        private int _viewportHeight;
        
        private int _levelWidth;
        private int _levelHeight;


        public Camera2D(int viewportWidth, int viewportHeight, int levelWidth, int levelHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _levelWidth = levelWidth;
            _levelHeight = levelHeight;
        }


        public void Follow(Vector2 targetPosition)
        {
            //System.Console.WriteLine($"Camera2D.Follow: Target Position: {targetPosition}, Viewport: {_viewportWidth}x{_viewportHeight}, Zoom: {Zoom}");
            
            float halfViewportWidth = _viewportWidth / (2f * Zoom);
            float halfViewportHeight = _viewportHeight / (2f * Zoom);
            //System.Console.WriteLine($"Camera2D.Follow: Half Viewport: {halfViewportWidth}x{halfViewportHeight}, Level Size: {_levelWidth}x{_levelHeight}");

            float clampedX = MathHelper.Clamp(
                targetPosition.X,
                halfViewportWidth,
                _levelWidth - halfViewportWidth
            );

            float clampedY = MathHelper.Clamp(
                targetPosition.Y,
                halfViewportHeight,
                _levelHeight - halfViewportHeight
            );

            Position = new Vector2(clampedX, clampedY);

            var translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
            var offset = Matrix.CreateTranslation(_viewportWidth / 2f, _viewportHeight / 2f, 0f);
            var zoomMatrix = Matrix.CreateScale(Zoom);
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);

            Transform = translation * rotationMatrix * zoomMatrix * offset;
        }

    }

}
