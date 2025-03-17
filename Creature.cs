using System.Numerics;
using MohawkGame2D;

namespace haedrich_owen_a3_game
{
    public class Creature
    {
        public int ID;
        public Vector2 position;
        public float scale = 1;
        public Texture2D viewedTexture;
        public Texture2D shadowTexture;

        public static Vector2 MaxSize = new Vector2(200, 200);
        public static Creature[] AllCreatures = new Creature[]
        {
            aMon(Vector2.Zero),
            bMon(Vector2.Zero),
            cMon(Vector2.Zero),
            dMon(Vector2.Zero),
            bird(Vector2.Zero)
        };

        static Texture2D aMonViewed = Graphics.LoadTexture("../../../../assets/aMon.png");
        static Texture2D aMonShadow = Graphics.LoadTexture("../../../../assets/aMonShadow.png");
        static Texture2D bMonViewed = Graphics.LoadTexture("../../../../assets/bMon.png");
        static Texture2D bMonShadow = Graphics.LoadTexture("../../../../assets/bMonShadow.png");
        static Texture2D cMonViewed = Graphics.LoadTexture("../../../../assets/cMon.png");
        static Texture2D cMonShadow = Graphics.LoadTexture("../../../../assets/cMonShadow.png");
        static Texture2D dMonViewed = Graphics.LoadTexture("../../../../assets/dMon.png");
        static Texture2D dMonShadow = Graphics.LoadTexture("../../../../assets/dMonShadow.png");
        static Texture2D birdViewed = Graphics.LoadTexture("../../../../assets/bird.png");
        static Texture2D birdShadow = Graphics.LoadTexture("../../../../assets/birdShadow.png");

        Creature(Vector2 spawnPosition)
        {
            position = spawnPosition;
        }

        public Creature(Creature creature)
        {
            this.ID = creature.ID;
            this.position = creature.position;
            this.scale = creature.scale;
            this.viewedTexture = creature.viewedTexture;
            this.shadowTexture = creature.shadowTexture;
        }

        public static Creature aMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                ID = 0,
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
                ID = 1,
                position = spawnPosition,
                viewedTexture = bMonViewed,
                shadowTexture = bMonShadow
            };
        }

        public static Creature cMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                ID = 2,
                position = spawnPosition,
                viewedTexture = cMonViewed,
                shadowTexture = cMonShadow
            };
        }

        public static Creature dMon(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                ID = 3,
                position = spawnPosition,
                viewedTexture = dMonViewed,
                shadowTexture = dMonShadow
            };
        }

        public static Creature bird(Vector2 spawnPosition)
        {
            return new Creature(spawnPosition)
            {
                ID = 4,
                position = spawnPosition,
                viewedTexture = birdViewed,
                shadowTexture = birdShadow
            };
        }
    }
}