using System;
using System.Linq;
using System.Numerics;
using haedrich_owen_a3_game;

namespace MohawkGame2D;

public class Game
{
    // State Management
    enum GameState
    {
        Menu,
        Playing,
        Gallery,
        End
    }

    GameState activeState = GameState.Menu;

    // Player View Control
    float rotationSpeed = 3.7f;
    float liftSpeed = 3.5f;
    int photosRemaining = 24;

    // Gallery View Control
    Vector2 photoOffset = new Vector2(0, 300);
    Vector2 playerOffset = new Vector2(0, 0);
    Vector2 photoFrameSize = new Vector2(275, 220);
    Vector2 galleryOffset = new Vector2(50, 100);
    Vector2 galleryTextOffset = new Vector2(390, 0);
    bool renaming = false;
    
    // Game Objects
    Rectangle viewfinder = new Rectangle(new Vector2(0, 0), new Vector2(250, 200));
    Creature bird = Creature.bird(new Vector2(0, -400));
    float birdSpeed = 1.3f;
    Creature[] spawnedCreatures = new Creature[11];
    Photograph[] photographs = new Photograph[24];
    const float mapLength = 3000;
    const float lookHeight = 1000;
    Vector2 playerView = new Vector2(-mapLength / 2, 0);
    Texture2D background = Graphics.LoadTexture("../../../forest.png");
    Vector2 backGroundOffset = new Vector2(-1500, -500);

    public void Setup()
    {
        Window.SetSize(800, 600); 

        // Spawn creatures
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            // Randomly pick a creature to spawn, except for the first 5 which are fixed to one of each species
            int pickCreature = Random.Integer(1, 5);
            if (i + 1 < 5) {
                pickCreature = i + 1;
            }
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
            case GameState.Menu:
                Menu();
                break;
            case GameState.End:
                Gallery();
                End();
                break;
        }

        if (Input.IsMouseButtonDown(MouseInput.Left) && activeState == GameState.Menu)
        {
            activeState = GameState.Playing;
        }

        if (Input.IsKeyboardKeyPressed(KeyboardInput.Tab))
        {
            switch (activeState)
            {
                case GameState.Gallery:
                    activeState = GameState.Playing;
                    break;
                case GameState.Playing:
                    activeState = GameState.Gallery;
                    // Focus on most recent photo
                    int photoCount = 0;
                    foreach (Photograph photograph in photographs)
                    {
                        if (photograph != null)
                        {
                            photoCount++;
                        }
                    }
                    playerOffset = (1 - photoCount) * photoOffset;
                    playerOffset.X = 0;
                    break;
            }
        }
    }

    public void Menu()
    {
        Window.ClearBackground(Color.OffWhite);
        Text.Size = 50;
        Text.Draw("Wildlife Photographer", Window.Width/ 2 - 300, Window.Height / 2 - 100);
        Text.Size = 30;
        Text.Draw("Try to Capture Them All!", Window.Width / 2 - 200, Window.Height / 2);
        Text.Draw("Click to start", Window.Width / 2 - 150, Window.Height / 2 + 100);
    }

    public void Play()
    {
        Window.ClearBackground(Color.OffWhite);
        Vector2 mousePosition = Input.GetMousePosition();
        photosRemaining = 0;
        foreach (Photograph photograph in photographs)
        { 
            if (photograph is null) { photosRemaining++; } 
        }

        if (photosRemaining > 0)
        {
            DrawEnvironment(mousePosition, playerView);
            DrawCreatures(mousePosition, playerView);
            DrawViewfinder(mousePosition, viewfinder.size);
        }
        else
        {
            activeState = GameState.End;
        }
        playerView += RotateView(mousePosition);
        if (Input.IsMouseButtonPressed(MouseInput.Left))
        {
            TakePicture();
        }
    }

    public void Gallery()
    {
        Window.ClearBackground(Color.OffWhite);
        Vector2 photoPosition = galleryOffset + playerOffset;
        
        for (int i = 0; i < photographs.Length; i++)
        {
            Text.Color = Color.Black;
            Text.Size = 30;
            if (photographs[i] != null)
            {
                bool clickedNearTitle = Vector2.Distance(Input.GetMousePosition(), photoPosition + galleryOffset + galleryTextOffset) < 150;
                // Try to rename the photograph
                if (Input.IsMouseButtonPressed(MouseInput.Left) && clickedNearTitle)
                {
                    photographs[i].rename = true;
                    renaming = true;
                    if (photographs[i].title == "Untitled")
                    {
                        photographs[i].title = "";
                    }
                }
                if (photographs[i].rename)
                {
                    Text.Color = Color.Red;
                    if (Input.IsKeyboardKeyPressed(KeyboardInput.Enter) || Input.IsKeyboardKeyPressed(KeyboardInput.Tab) || (Input.IsMouseButtonDown(MouseInput.Left) && !clickedNearTitle))
                    {
                        photographs[i].rename = false;
                        renaming = false;
                        Text.Color = Color.Black;
                        if (photographs[i].title == "")
                        {
                            photographs[i].title = "Untitled";
                        }
                    }
                    else if (Input.IsKeyboardKeyPressed(KeyboardInput.Backspace))
                    {
                        if (photographs[i].title.Length > 0)
                        {
                            string newTitle = photographs[i].title.Remove(photographs[i].title.Length - 1);
                            photographs[i].title = newTitle;
                        }
                    }
                    else
                    {
                        char newChar = Input.GetCharsPressed();
                        if (newChar != 0)
                        {
                            photographs[i].title += newChar;
                        }
                    }
                }
                // Draw the photo and display the title
                DisplayPhotograph(photographs[i], photoPosition, 1.3f);
                Text.Draw("\"" + photographs[i].title + "\"", photoPosition + galleryOffset + galleryTextOffset);

                // Move to the next photo position
                photoPosition += photoOffset;
            }
        }

        playerOffset += RotateView(Input.GetMousePosition(), false);
    }

    public void End()
    {
        Creature[] creaturesCaught = new Creature[Creature.AllCreatures.Length];
        foreach (Photograph photograph in photographs)
        {
            foreach (Creature creature in photograph.capturedCreatures)
            {
                if (creature is null)
                    continue;
                else
                {
                    creaturesCaught[creature.ID] = creature;
                }
            }
        }

        int creaturesMissed = 0;
        foreach (Creature creature in creaturesCaught)
        {
            if (creature is null)
            {
                creaturesMissed++;
            }
        }

        if (creaturesMissed <= 0)
        {
            // Do not ClearBackground here, the game will show the winning gallery
            // Draw a rectanglar frame for the win text
            Draw.FillColor = Color.Yellow;
            Draw.LineSize = 0;
            Draw.Rectangle(Window.Width/2 - 75, 75, 200, 70);
;           Text.Draw("You Win!", Window.Width/2 - 50, 100);
        }
        else
        {
            Window.ClearBackground(Color.OffWhite);
            Text.Draw($"You Missed {creaturesMissed} Creatures!", Window.Width / 2 - 150, Window.Height / 2 - 50);
            Text.Draw("You Lose!", Window.Width / 2 - 150, Window.Height / 2 - 150);
        }

        // Press Tab to return to the menu indicator in the top right
        Text.Size = 20;
        Text.Draw("Press Tab to Play Again", Window.Width - 300, 20);

        if (Input.IsKeyboardKeyPressed(KeyboardInput.Tab))
        {
            activeState = GameState.Menu;
            Setup();
            playerView = new Vector2(-mapLength / 2, 0);
            photographs = new Photograph[24];
        }
    }

    public void DisplayPhotograph(Photograph photograph, Vector2 photoPosition, float scale = 1)
    {
        Graphics.Scale = 1 * scale; ;
        Vector2 backgroundSubsetOrigin = photograph.viewfinderPosition - backGroundOffset;
        Graphics.DrawSubset(background, photoPosition, backgroundSubsetOrigin, photoFrameSize);

        foreach (Creature creature in photograph.capturedCreatures)
        {
            if (creature == null)
                continue;

            Rectangle overlap = GetOverlap(
                new Rectangle(creature.position, Creature.MaxSize * creature.scale),
                new Rectangle(photograph.viewfinderPosition, photoFrameSize)
            );

            Vector2 viewfinderToOverlap = overlap.position - photograph.viewfinderPosition;
            Vector2 drawPosition = photoPosition + viewfinderToOverlap * scale;

            Graphics.Scale = creature.scale * scale;
            Vector2 textureSubsetOrigin = (overlap.position - creature.position) / creature.scale;
            Graphics.DrawSubset(creature.viewedTexture, drawPosition, textureSubsetOrigin, overlap.size / creature.scale);
        }

        Draw.LineSize = 25;
        Draw.FillColor = Color.Clear;
        Draw.Rectangle(photoPosition, photoFrameSize * scale);
    }

    public Vector2 GetSpawnPosition()
    {
        // The first creature will always be visible
        if (spawnedCreatures[0] is null)
        {
            float minX = mapLength / 2;
            float maxX = mapLength / 2 + 400;
            float minY = 0f;
            float maxY = 400f;
            return Random.Vector2(minX, maxX, minY, maxY);
        }

        Vector2 spawnPosition = Random.Vector2(150, mapLength, -300, 900);
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            if (spawnedCreatures[i] is not null &&
                Vector2.Distance(spawnPosition, spawnedCreatures[i].position) < 200)
            {
                return GetSpawnPosition();
            }
        }
        return spawnPosition;
    }

    public void DrawViewfinder(Vector2 mousePosition, Vector2 viewfinderSize)
    {
        viewfinder.position = mousePosition - new Vector2(viewfinder.size.X, 0);
        Draw.LineColor = Color.Black;
        Draw.FillColor = new Color(0, 0, 0, 0);
        Draw.LineSize = 5;
        Draw.Rectangle(viewfinder.position, viewfinder.size);
        // Draw remaining photos above the viewfinder
        Vector2 textOffset = new Vector2(35, -25);
        Text.Color = Color.White;
        Text.Size = 20;
        Text.Draw($"Photos Remaining: {photosRemaining}", viewfinder.position + textOffset);
    }

    public Vector2 RotateView(Vector2 mousePosition, bool isPlayer = true)
    {
        Vector2 rotationChange = Vector2.Zero;

        if (mousePosition.X < 100 && (-playerView.X > 0 || !isPlayer))
        {
            rotationChange.X = rotationSpeed;
        }
        else if (mousePosition.X > Window.Width - 100 && (-playerView.X < mapLength || !isPlayer))
        {
            rotationChange.X = -rotationSpeed;
        }

        if (mousePosition.Y < 100 && (playerView.Y < lookHeight / 2 || !isPlayer))
        {
            rotationChange.Y = liftSpeed;
        }
        else if (mousePosition.Y > Window.Height - 100 && (playerView.Y > -lookHeight / 2 || !isPlayer))
        {
            rotationChange.Y = -liftSpeed;
        }

        return rotationChange;
    }

    public void DrawEnvironment(Vector2 mousePosition, Vector2 playerView)
    {
        Graphics.Scale = 1;
        Graphics.Draw(background, playerView + backGroundOffset);
    }

    public void DrawCreatures(Vector2 mousePosition, Vector2 playerView)
    {
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Scale = creature.scale;
            Graphics.Draw(creature.shadowTexture, creature.position + playerView);
        }

        Graphics.Scale = bird.scale;
        if (bird.position.X < 0)
        {
            bird.position.X = mapLength;
        }

        bird.position += new Vector2(-birdSpeed, 0);
        Graphics.Draw(bird.shadowTexture, bird.position + playerView);

        Vector2 viewfinderWorldPosition = viewfinder.position + playerView - new Vector2(viewfinder.size.X, 0);

        // Check and draw spawned creatures in viewfinder
        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            Graphics.Scale = creature.scale;
            Vector2 creatureSize = Creature.MaxSize * creature.scale;

            if (DoRectanglesOverlap(
                new Rectangle(creature.position + playerView, creatureSize),
                new Rectangle(viewfinder.position, viewfinder.size)))
            {
                Rectangle overlap = GetOverlap(
                    new Rectangle(creature.position + playerView, creatureSize),
                    new Rectangle(viewfinder.position, viewfinder.size)
                );
                Graphics.DrawSubset(creature.viewedTexture, overlap.position,
                    Vector2.Zero + (1 / creature.scale) * (overlap.position - (creature.position + playerView)),
                    overlap.size / creature.scale);
            }
        }

        // Check and draw bird in viewfinder
        Graphics.Scale = bird.scale;
        Vector2 birdSize = Creature.MaxSize * bird.scale;
        Rectangle birdScreenRect = new Rectangle(bird.position + playerView, birdSize);
        if (DoRectanglesOverlap(birdScreenRect, new Rectangle(viewfinder.position, viewfinder.size)))
        {
            Rectangle overlap = GetOverlap(
                birdScreenRect,
                new Rectangle(viewfinder.position, viewfinder.size)
            );
            Vector2 subsetOrigin = (overlap.position - (bird.position + playerView)) / bird.scale;
            Graphics.DrawSubset(bird.viewedTexture, overlap.position, subsetOrigin, overlap.size / bird.scale);
        }
    }
    public void TakePicture()
    {
        Vector2 viewfinderWorldPosition = viewfinder.position - playerView;
        Creature[] capturedCreatures = new Creature[5];
        // Check if the in-game camera viewfinder is on the bird
        if (DoRectanglesOverlap(
            new Rectangle(bird.position, Creature.MaxSize * bird.scale),
            new Rectangle(viewfinderWorldPosition, viewfinder.size)))
        {
            for (int j = 0; j < capturedCreatures.Length; j++)
            {
                if (capturedCreatures[j] is null)
                {
                    capturedCreatures[j] = bird;
                    break;
                }
            }
        }

        for (int i = 0; i < spawnedCreatures.Length; i++)
        {
            Creature creature = spawnedCreatures[i];
            if (DoRectanglesOverlap(
                new Rectangle(creature.position, Creature.MaxSize * creature.scale),
                new Rectangle(viewfinderWorldPosition, viewfinder.size)))
            {
                for (int j = 0; j < capturedCreatures.Length; j++)
                {
                    if (capturedCreatures[j] is null)
                    {
                        capturedCreatures[j] = creature;
                        break;
                    }
                }
            }
        }

        // Draw flash
        Draw.FillColor = Color.White;
        Draw.Rectangle(viewfinder.position, viewfinder.size);

        // Save the picture to the first null slot in the photographs array
        for (int i = 0; i < photographs.Length; i++)
        {
            if (photographs[i] is null)
            {
                photographs[i] = new Photograph(capturedCreatures, viewfinderWorldPosition);
                return;
            }
        }
    }

    public bool DoRectanglesOverlap(Rectangle rect1, Rectangle rect2)
    {
        Vector2 P1 = rect1.position;
        Vector2 S1 = rect1.size;
        Vector2 P2 = rect2.position;
        Vector2 S2 = rect2.size;

        if (P1.X + S1.X <= P2.X || P2.X + S2.X <= P1.X)
            return false;
        if (P1.Y + S1.Y <= P2.Y || P2.Y + S2.Y <= P1.Y)
            return false;
        return true;
    }

    public Rectangle GetOverlap(Rectangle rect1, Rectangle rect2)
    {
        Vector2 P1 = rect1.position;
        Vector2 S1 = rect1.size;
        Vector2 P2 = rect2.position;
        Vector2 S2 = rect2.size;

        float left = Math.Max(P1.X, P2.X);
        float right = Math.Min(P1.X + S1.X, P2.X + S2.X);
        float top = Math.Max(P1.Y, P2.Y);
        float bottom = Math.Min(P1.Y + S1.Y, P2.Y + S2.Y);

        float width = right - left;
        float height = bottom - top;

        if (width <= 0 || height <= 0)
            return Rectangle.Zero;

        return new Rectangle(new Vector2(left, top), new Vector2(width, height));
    }
}