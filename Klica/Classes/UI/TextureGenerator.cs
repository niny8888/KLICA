using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class TextureGenerator
{
    public static Texture2D CreateCircleRadiusLineTexture(GraphicsDevice graphicsDevice, int radius, int lineThickness = 1)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
        Color[] colorData = new Color[diameter * diameter];

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int dx = x - radius;
                int dy = y - radius;

                float distance = MathF.Sqrt(dx * dx + dy * dy);

                if (distance >= radius - lineThickness && distance <= radius && dy >= 0) // Half-circle condition (top half only)
                {
                    colorData[y * diameter + x] = Color.White;
                }
                else
                {
                    colorData[y * diameter + x] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);
        return texture;
    }
}
