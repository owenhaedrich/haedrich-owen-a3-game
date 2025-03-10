// Include the namespaces (code libraries) you need below.
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using haedrich_owen_a3_game;
using Raylib_cs;

// The namespace your code is in.
namespace MohawkGame2D;

public class Game
{
    Vector2 viewfinderSize = new Vector2(250, 200); // In-game camera viewfinder size
    Vector2 viewfinderPosition = new Vector2(0, 0); // In-game camera viewfinder screen position
    Vector2 playerView = new Vector2(0, 0); // Turn horizontally and change vertical look angle
    Creature bird = Creature.bird(new Vector2(0, 800)); // The bird is a special creature that moves
    float birdSpeed = 1.3f;
    Creature[] spawnedCreatures = new Creature[7]; // There are 7 stationary creatures

    public void Setup()
    {
        Window.SetSize(800, 600);

        // Spawn creatures
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            int pickCreature = Random.Integer(1, 4);
            if (pickCreature == 1)
            {
                spawnedCreatures[i] = Creature.aMon(Random.Vector2());
            }
            else if (pickCreature == 2)
            {
                spawnedCreatures[i] = Creature.bMon(Random.Vector2());
            }
            else if (pickCreature == 3)
            {
                spawnedCreatures[i] = Creature.cMon(Random.Vector2());
            }
            else if (pickCreature == 4)
            {
                spawnedCreatures[i] = Creature.dMon(Random.Vector2());
            }
        }
    }

    public void Update()
    {
        Window.ClearBackground(Color.OffWhite);
        Vector2 mousePosition = Input.GetMousePosition();
        DrawEnvironment(mousePosition, viewfinderSize, playerView);
        DrawCreatures(mousePosition, viewfinderSize, playerView);
        DrawViewfinder(mousePosition, viewfinderSize);
        playerView += RotateView(mousePosition, viewfinderSize);
        if (Input.IsMouseButtonPressed(MouseInput.Left))
        {
            TakePicture();
        }
    }

    public void DrawViewfinder(Vector2 mousePosition, Vector2 viewfinderSize)
    {
        // Draw a viewfinder on the mouse position
        Color viewfinderColor = Color.White;
        float viewfinderThickness = 5;

        Draw.LineColor = Color.Black;
        Draw.FillColor = new Color(0, 0, 0, 0);
        Draw.LineSize = viewfinderThickness;

        // Draw the viewfinder, held by the mouse on the right side 
        viewfinderPosition = mousePosition - new Vector2(viewfinderSize.X, 0);
        Draw.Rectangle(viewfinderPosition, viewfinderSize);
    }

    public Vector2 RotateView(Vector2 mousePosition, Vector2 viewfinderSize)
    {
        // Rotate the viewfinder if the mouse is 100 px from the left or right edge
        float rotationSpeed = 1.5f;
        Vector2 rotationChange = new Vector2(0, 0);

        if (mousePosition.X < 100)
        {
            rotationChange.X = rotationSpeed;
        }
        else if (mousePosition.X > Window.Width - 100)
        {
            rotationChange.X = -rotationSpeed;
        }

        // Lift the viewfinder if the mouse is 100 px from the top or bottom edge
        float liftSpeed = 0.5f;
        if (mousePosition.Y < 100)
        {
            rotationChange.Y = liftSpeed;
        }
        else if (mousePosition.Y > Window.Height - 100)
        {
            rotationChange.Y = -liftSpeed;
        }

        return rotationChange;
    }

    public void DrawEnvironment(Vector2 mousePosition, Vector2 viewfinderSize, Vector2 playerView)
    {
    }

    public void DrawCreatures(Vector2 mousePosition, Vector2 viewfinderSize, Vector2 playerView)
    {
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Draw(creature.viewedTexture, creature.position - creature.size/2 + playerView);
        }

        // Draw the bird
        bird.position += new Vector2(birdSpeed, 0);
        Graphics.Draw(bird.viewedTexture, bird.position);
    }

    public void TakePicture() 
    {
        Vector2 viewfinderWorldPosition = viewfinderPosition - playerView;

        Console.WriteLine("Taking picture at:" + viewfinderWorldPosition.ToString());
        // Check if the in-game camera viewfinder is on the bird
        if (bird.position.X > viewfinderWorldPosition.X && bird.position.X < viewfinderWorldPosition.X + viewfinderSize.X &&
            bird.position.Y > viewfinderWorldPosition.Y && bird.position.Y < viewfinderWorldPosition.Y + viewfinderSize.Y)
        {
            // Take a picture of the bird
            Console.WriteLine("Bird!");
        }

        // Check if the in-game camera viewfinder is on any of the stationary creatures
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            if (creature.position.X > viewfinderWorldPosition.X && creature.position.X < viewfinderWorldPosition.X + viewfinderSize.X &&
                creature.position.Y > viewfinderWorldPosition.Y && creature.position.Y < viewfinderWorldPosition.Y + viewfinderSize.Y)
            {
                // Take a picture of the creature
                Console.WriteLine("Creature!");
            }
        }
    }
}

