using System;
using Microsoft.Xna.Framework;

namespace Klica.Classes.Managers
{
    public class Transform
    {
        private static Random random = new Random();

        public static void RandomizePosition<T>(T obj, Rectangle bounds)
        {
            // Use reflection to find and update the Position property
            var positionProperty = typeof(T).GetProperty("Position");
            if (positionProperty != null && positionProperty.PropertyType == typeof(Vector2))
            {
                // Generate random coordinates within bounds
                float x = (float)random.Next(bounds.Left, bounds.Right);
                float y = (float)random.Next(bounds.Top, bounds.Bottom);

                // Update the Position property
                positionProperty.SetValue(obj, new Vector2(x, y));
            }
            else
            {
                throw new ArgumentException("The object does not have a Vector2 Position property.");
            }
        }
    }
}
