using System;
using Microsoft.Xna.Framework;

namespace Klica.Classes.Managers
{
    public class Transform
    {
        private Random random = new Random();

        public void RandomizePosition<T>(T obj, Rectangle bounds)
        {
            var positionProperty = typeof(T).GetProperty("Position");
            if (positionProperty != null && positionProperty.PropertyType == typeof(Vector2))
            {
                float x = (float)random.Next(bounds.Left, bounds.Right);
                float y = (float)random.Next(bounds.Top, bounds.Bottom);

                positionProperty.SetValue(obj, new Vector2(x, y));
            }
            else
            {
                throw new ArgumentException("The object does not have a Vector2 Position property.");
            }
        }
    }
}
