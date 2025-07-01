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

                if (distance >= radius - lineThickness && distance <= radius && dy >= 0) 
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
    public static Texture2D Pixel { get; private set; }

    public static void Init(GraphicsDevice device)
    {
        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }
        
    public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
        Color[] colorData = new Color[diameter * diameter];

        float center = radius;

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int index = y * diameter + x;
                float dx = x - center;
                float dy = y - center;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                colorData[index] = distance <= radius ? color : Color.Transparent;
            }
        }

        texture.SetData(colorData);
        return texture;
    }

}
