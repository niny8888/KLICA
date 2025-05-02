using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public class MyLogoAnimation
{
    private List<Texture2D> frames; // List to hold all frames
    private int currentFrameIndex;   // Index of the current frame
    private double frameTimer;       // Timer to control frame switching
    private double frameDuration;    // How long each frame is displayed (in seconds)
    private bool isPlaying;          // Whether the animation is currently playing

    public MyLogoAnimation(double frameDuration = 0.1) // default frame duration of 0.1 seconds
    {
        frames = new List<Texture2D>();
        currentFrameIndex = 0;
        this.frameDuration = frameDuration;
        frameTimer = 0;
        isPlaying = true;
    }

    // Load frames into the animation
    public void LoadFrames(ContentManager content, string[] frameNames)
    {
        foreach (var frameName in frameNames)
        {
            frames.Add(content.Load<Texture2D>(frameName));
        }
    }

    // Update the animation based on elapsed time
    public void Update(GameTime gameTime)
    {
        if (!isPlaying) return;

        // Increment the timer by the time elapsed
        frameTimer += gameTime.ElapsedGameTime.TotalSeconds;

        // If the timer exceeds the frame duration, move to the next frame
        if (frameTimer >= frameDuration)
        {
            frameTimer = 0;
            currentFrameIndex++;
            if (currentFrameIndex >= frames.Count)
            {
                currentFrameIndex = 0; // Loop back to the first frame
            }
        }
    }

    // Draw the current frame to the screen
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
    {
        // Assuming 'frames' is a list/array of Texture2D objects
        Texture2D currentFrame = frames[currentFrameIndex];

        // Draw the current frame with scaling applied
        spriteBatch.Draw(currentFrame, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }



    // Start the animation
    public void Play()
    {
        isPlaying = true;
    }
    public Texture2D GetCurrentFrame()
    {
        return frames[currentFrameIndex];
    }


    // Pause the animation
    public void Pause()
    {
        isPlaying = false;
    }

    // Stop the animation and reset to the first frame
    public void Stop()
    {
        isPlaying = false;
        currentFrameIndex = 0;
    }
    public bool IsFinished()
    {
        return currentFrameIndex == frames.Count - 1 && frameTimer == 0;
    }

}
