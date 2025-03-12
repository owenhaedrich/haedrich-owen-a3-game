using System;
using System.Numerics;
using MohawkGame2D;

namespace haedrich_owen_a3_game
{
    public class Photograph
    {
        Creature[] capturedCreatures = new Creature[5];
        public Vector2 viewfinderPosition = new Vector2(0, 0);
        string title = "Untitled";
        
        public Photograph(Creature[] capturedCreatures, Vector2 viewfinderPosition, string title)
        {
            this.capturedCreatures = capturedCreatures;
            this.viewfinderPosition = viewfinderPosition;
            this.title = title;
        }
    }
}
