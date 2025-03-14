using System.Numerics;

namespace haedrich_owen_a3_game
{
    public class Rectangle(Vector2 position, Vector2 size)
    {
        public Vector2 position = position;
        public Vector2 size = size;
        public static Rectangle Zero = new Rectangle(Vector2.Zero, Vector2.Zero);
    }
}
