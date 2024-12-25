using Microsoft.Xna.Framework;

public class Collider
{
    public enum ColliderType { Sphere, Rectangle }
    public ColliderType Type { get; }
    public Vector2 Position { get; set; } // Position for spheres or the center of rectangles
    public float Radius { get; } // For sphere colliders
    public Rectangle Bounds { get; } // For rectangle colliders
    public object Owner { get; } // Reference to the object this collider belongs to

    // Constructor for sphere collider
    public Collider(Vector2 position, float radius, object owner)
    {
        Type = ColliderType.Sphere;
        Position = position;
        Radius = radius;
        Owner = owner;
    }

    // Constructor for rectangle collider
    public Collider(Rectangle bounds, object owner)
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
}
