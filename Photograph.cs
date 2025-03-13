using System.Numerics;

namespace haedrich_owen_a3_game
{
    public class Photograph
    {
        public Creature[] capturedCreatures = new Creature[5];
        public Vector2 viewfinderPosition = new Vector2(0, 0);
        string title = "Untitled";
        
        public Photograph(Creature[] capturedCreatures, Vector2 viewfinderPosition, string title)
        {

            for (int i = 0; i < capturedCreatures.Length; i++)
            {
                if (capturedCreatures[i] != null)
                {
                    this.capturedCreatures[i] = new Creature(capturedCreatures[i]);
                }
            }
            this.viewfinderPosition = viewfinderPosition;
            this.title = title;
        }
    }
}
