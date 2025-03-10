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
    Vector2 playerRotation = new Vector2(0, 0); // Turn horizontally
    float playerLookAngle = 0; // Vertical look angle
    float birdPosition = 0; // The bird is a special creature that moves
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
        DrawEnvironment(mousePosition, viewfinderSize, playerRotation);
        DrawCreatures(mousePosition, viewfinderSize, playerRotation);
        DrawViewfinderOnMouse(mousePosition, viewfinderSize);
        playerRotation += RotateViewfinder(mousePosition, viewfinderSize);
    }

    public void DrawViewfinderOnMouse(Vector2 mousePosition, Vector2 viewfinderSize)
    {
        // Draw a viewfinder on the mouse position
        Color viewfinderColor = Color.White;
        float viewfinderThickness = 5;

        Draw.LineColor = Color.Black;
        Draw.FillColor = new Color(0, 0, 0, 0);
        Draw.LineSize = viewfinderThickness;

        // Draw the viewfinder
        Draw.Rectangle(mousePosition - viewfinderSize * 0.5f, viewfinderSize);
    }

    public Vector2 RotateViewfinder(Vector2 mousePosition, Vector2 viewfinderSize)
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

    public void DrawEnvironment(Vector2 mousePosition, Vector2 viewfinderSize, Vector2 playerRotation)
    {
    }

    public void DrawCreatures(Vector2 mousePosition, Vector2 viewfinderSize, Vector2 playerRotation)
    {
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Draw(creature.viewedTexture, creature.position + playerRotation);
        }
    }

    public void TakePicture() 
    {
    }
}

