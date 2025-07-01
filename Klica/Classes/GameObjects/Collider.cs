using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Collider
{
    public enum ColliderType { Sphere, Rectangle }
    public ColliderType Type { get; }
    public Vector2 Position { get; set; } 
    public float Radius { get; set;}
    public Rectangle Bounds { get; } 
    public object Owner { get; set;} 

    
    public Collider(Vector2 position, float radius, object owner) // Krogla
    {
        Type = ColliderType.Sphere;
        Position = position;
        Radius = radius;
        Owner = owner;
    }

    
    public Collider(Rectangle bounds, object owner) //rectangle
    {
        Type = ColliderType.Rectangle;
        Bounds = bounds;
        Owner = owner;
    }

    public bool Intersects(Collider other)
    {
        if (Type == ColliderType.Sphere && other.Type == ColliderType.Sphere)
        {
            return Vector2.Distance(Position, other.Position) <= Radius + other.Radius;
        }
        else if (Type == ColliderType.Rectangle && other.Type == ColliderType.Rectangle)
        {
            return Bounds.Intersects(other.Bounds);
        }
        else if (Type == ColliderType.Sphere && other.Type == ColliderType.Rectangle)
        {
            return IntersectsRectangleCircle(other.Bounds, Position, Radius);
        }
        else if (Type == ColliderType.Rectangle && other.Type == ColliderType.Sphere)
        {
            return IntersectsRectangleCircle(Bounds, other.Position, other.Radius);
        }

        return false;
    }

    private bool IntersectsRectangleCircle(Rectangle rect, Vector2 circleCenter, float circleRadius)
    {
        float nearestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        float nearestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);

        float deltaX = circleCenter.X - nearestX;
        float deltaY = circleCenter.Y - nearestY;

        return (deltaX * deltaX + deltaY * deltaY) <= (circleRadius * circleRadius);
    }
    public static void DrawCollider(SpriteBatch spriteBatch, Texture2D circleTexture, Collider collider, Color color)
    {
        if (collider.Type == Collider.ColliderType.Sphere)
        {
            var circleBounds = new Rectangle(
                (int)(collider.Position.X - collider.Radius),
                (int)(collider.Position.Y - collider.Radius),
                (int)(collider.Radius * 2),
                (int)(collider.Radius * 2)
            );
            spriteBatch.Draw(circleTexture, circleBounds, color * 0.5f); //za debug
        }
        else if (collider.Type == Collider.ColliderType.Rectangle)
        {
            spriteBatch.Draw(circleTexture, collider.Bounds, color * 0.5f); //za debug
        }
    }

    public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);

        Color[] colorData = new Color[diameter * diameter];
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                Vector2 position = new Vector2(x, y);
                if (Vector2.Distance(position, center) <= radius)
                {
                    colorData[y * diameter + x] = color;
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
