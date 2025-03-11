using System.Numerics;
using MohawkGame2D;

namespace haedrich_owen_a3_game
{
    class Creature
    {
        public Vector2 position;
        public float scale = 1;
        public Texture2D viewedTexture = Graphics.LoadTexture("./unknown.png");
        public Texture2D shadowTexture = Graphics.LoadTexture("./unknown.png");
        public static Vector2 standardSize = new Vector2(200, 200);

        static Texture2D aMonViewed = Graphics.LoadTexture("./aMon.png");
        static Texture2D aMonShadow = Graphics.LoadTexture("./aMonShadow.png");
        static Texture2D bMonViewed = Graphics.LoadTexture("./bMon.png");
        static Texture2D bMonShadow = Graphics.LoadTexture("./bMonShadow.png");
        static Texture2D cMonViewed = Graphics.LoadTexture("./cMon.png");
        static Texture2D cMonShadow = Graphics.LoadTexture("./cMonShadow.png");
        static Texture2D dMonViewed = Graphics.LoadTexture("./dMon.png");
        static Texture2D dMonShadow = Graphics.LoadTexture("./dMonShadow.png");
        static Texture2D birdViewed = Graphics.LoadTexture("./bird.png");
        static Texture2D birdShadow = Graphics.LoadTexture("./birdShadow.png");

        Creature(Vector2 spawnPosition)
        {
            position = spawnPosition;
        }

        public static Creature aMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                position = spawnPosition,
                viewedTexture = aMonViewed,
                shadowTexture = aMonShadow,
                scale = 0.5f
            };
        }

        public static Creature bMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                position = spawnPosition,
                viewedTexture = bMonViewed,
                shadowTexture = bMonShadow
            };
        }

        public static Creature cMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                position = spawnPosition,
                viewedTexture = cMonViewed,
                shadowTexture = cMonShadow
            };
        }

        public static Creature dMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                position = spawnPosition,
                viewedTexture = dMonViewed,
                shadowTexture = dMonShadow
            };
        }

        public static Creature bird(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                position = spawnPosition,
                viewedTexture = birdViewed,
                shadowTexture = birdShadow
            };
        }
    }
}