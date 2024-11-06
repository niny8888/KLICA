using Microsoft.Xna.Framework;

namespace KlicaGame.Classes.Managers
{
    public static class CollisionManager
    {
        public static bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }
    }
}
