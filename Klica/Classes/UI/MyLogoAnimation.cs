using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public class MyLogoAnimation
{
    private List<Texture2D> frames; 
    private int currentFrameIndex;
    private double frameTimer;   
    private double frameDuration;
    private bool isPlaying;    

    public MyLogoAnimation(double frameDuration = 0.1) 
    {
        frames = new List<Texture2D>();
        currentFrameIndex = 0;
        this.frameDuration = frameDuration;
        frameTimer = 0;
        isPlaying = true;
    }

    
    public void LoadFrames(ContentManager content, string[] frameNames)
    {
        foreach (var frameName in frameNames)
        {
            frames.Add(content.Load<Texture2D>(frameName));
        }
    }

    
    public void Update(GameTime gameTime)
    {
        if (!isPlaying) return;

        frameTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (frameTimer >= frameDuration)
        {
            frameTimer = 0;
            currentFrameIndex++;
            if (currentFrameIndex >= frames.Count)
            {
                currentFrameIndex = 0; 
            }
        }
    }

    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
    {
        Texture2D currentFrame = frames[currentFrameIndex];
        spriteBatch.Draw(currentFrame, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }



    public void Play()
    {
        isPlaying = true;
    }
    public Texture2D GetCurrentFrame()
    {
        return frames[currentFrameIndex];
    }


    public void Pause()
    {
        isPlaying = false;
    }

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
