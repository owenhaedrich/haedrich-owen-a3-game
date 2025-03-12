// Include the namespaces (code libraries) you need below.
using System;
using System.ComponentModel.Design;
using System.Numerics;
using haedrich_owen_a3_game;

// The namespace your code is in.
namespace MohawkGame2D;

public class Game
{
    // State Management
    enum GameState
    {
        Menu,
        Playing,
        Gallery,
        PhotoPreview
    }

    GameState activeState = GameState.Playing;

    // Player View Control
    float rotationSpeed = 3.5f;
    float liftSpeed = 1.5f;

    Vector2 viewfinderSize = new Vector2(250, 200); // In-game camera viewfinder size
    Vector2 viewfinderPosition = new Vector2(0, 0); // In-game camera viewfinder screen position
    Creature bird = Creature.bird(new Vector2(0, 800)); // The bird is a special creature that moves
    float birdSpeed = 1.3f;
    Creature[] spawnedCreatures = new Creature[11]; // There are 11 stationary creatures
    Photograph[] photographs = new Photograph[24]; // There are 24 shots available

    const float mapLength = 3000;
    Vector2 playerView = new Vector2(-mapLength / 2, 0); // Turn horizontally and change vertical look angle. Initialize to center of the map

    public void Setup()
    {
        Window.SetSize(800, 600);

        // Spawn creatures
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            int pickCreature = Random.Integer(1, 4);
            Vector2 spawnPosition = GetSpawnPosition();
            switch (pickCreature)
            {
                case 1:
                    spawnedCreatures[i] = Creature.aMon(spawnPosition);
                    break;
                case 2:
                    spawnedCreatures[i] = Creature.bMon(spawnPosition);
                    break;
                case 3:
                    spawnedCreatures[i] = Creature.cMon(spawnPosition);
                    break;
                case 4:
                    spawnedCreatures[i] = Creature.dMon(spawnPosition);
                    break;
            }
        }
    }

    public void Update()
    {
        switch (activeState)
        {
            case GameState.Playing:
                Play();
                break;
            case GameState.Gallery:
                Gallery();
                break;
            case GameState.PhotoPreview:
                PhotoPreview();
                break;
            case GameState.Menu:
                Menu();
                break;
        }
    }

    public void Play()
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

    public void Gallery()
    {

    }

    public void PhotoPreview()
    {

    }

    public void Menu()
    {

    }

    public Vector2 GetSpawnPosition()
    {
        // Ensure one creature is immediately visible then provide a full range of spawn positions
        if (spawnedCreatures[0] is null)
        {
            return Random.Vector2(mapLength / 2 - 300, mapLength / 2 - 700, 400, 600);
        }

        Vector2 spawnPosition = Random.Vector2(150, mapLength, 200, 900);
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            // Check distance to all instantiated creatures
            if (spawnedCreatures[i] is not null)
            {
                if (Vector2.Distance(spawnPosition, spawnedCreatures[i].position) < 200)
                {
                    return GetSpawnPosition();
                }
            }
        }
        return spawnPosition;
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

        Vector2 rotationChange = new Vector2(0, 0);

        if (mousePosition.X < 100 && Math.Abs(playerView.X) > 0)
        {
            rotationChange.X = rotationSpeed;
        }
        else if (mousePosition.X > Window.Width - 100 && Math.Abs(playerView.X) < mapLength)
        {
            rotationChange.X = -rotationSpeed;
        }

        // Lift the viewfinder if the mouse is 100 px from the top or bottom edge
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
        // Draw silhouette layer
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Scale = creature.scale;
            Graphics.Draw(creature.shadowTexture, creature.position + playerView);
        }

        Graphics.Scale = bird.scale;
        bird.position += new Vector2(birdSpeed, 0);
        Graphics.Draw(bird.shadowTexture, bird.position);

        // Draw viewed layer over the silhouette inside the viewfinder

        Vector2 viewfinderWorldPosition = viewfinderPosition + playerView - new Vector2(viewfinderSize.X, 0); // Viewfinder's world position (The viewfinder rotates with the player)

        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Scale = creature.scale;

            Vector2 creatureSize = Creature.standardSize * creature.scale;
            // Check if creature's bounds overlap with viewfinder in coordinates relative to the player view
            if (DoRectanglesOverlap(creature.position + playerView, creatureSize, viewfinderPosition, viewfinderSize))
            {
                // Using Vector4 to store the overlap rectangle
                Vector4 overlap = GetOverlap(creature.position + playerView, creatureSize, viewfinderPosition, viewfinderSize);
                Vector2 overlapOrigin = new Vector2(overlap.X, overlap.Y);
                Vector2 overlapSize = new Vector2(overlap.Z, overlap.W);
                // Draw the portion inside the viewfinder
                Graphics.DrawSubset(creature.viewedTexture, overlapOrigin, Vector2.Zero + (1 / creature.scale) * (overlapOrigin - (creature.position + playerView)), overlapSize / creature.scale);
            }
        }
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
    public bool DoRectanglesOverlap(Vector2 P1, Vector2 S1, Vector2 P2, Vector2 S2)
    {
        // Calculate the left, right, top, and bottom edges of both rectangles
        float left1 = P1.X;
        float right1 = P1.X + S1.X;
        float top1 = P1.Y;
        float bottom1 = P1.Y + S1.Y;

        float left2 = P2.X;
        float right2 = P2.X + S2.X;
        float top2 = P2.Y;
        float bottom2 = P2.Y + S2.Y;

        // Check if one rectangle is completely to the left or right of the other
        if (right1 <= left2 || right2 <= left1)
            return false;

        // Check if one rectangle is completely above or below the other
        if (bottom1 <= top2 || bottom2 <= top1)
            return false;

        // If none of the above conditions are true, rectangles overlap
        return true;
    }

    public Vector4 GetOverlap(Vector2 P1, Vector2 S1, Vector2 P2, Vector2 S2)
    {
        // Calculate the edges of both rectangles
        float left1 = P1.X;
        float right1 = P1.X + S1.X;
        float top1 = P1.Y;
        float bottom1 = P1.Y + S1.Y;

        float left2 = P2.X;
        float right2 = P2.X + S2.X;
        float top2 = P2.Y;
        float bottom2 = P2.Y + S2.Y;

        // Calculate the overlap boundaries
        float left = Math.Max(left1, left2);
        float right = Math.Min(right1, right2);
        float top = Math.Max(top1, top2);
        float bottom = Math.Min(bottom1, bottom2);

        float width = right - left;
        float height = bottom - top;

        // Check if there is actually an overlap
        if (width <= 0 || height <= 0)
        {
            return new Vector4(0, 0, 0, 0); // No overlap
        }

        return new Vector4(left, top, width, height);
    }

}