using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

public class MenuScene : IScene
{
    private Texture2D _background;
    private BitmapFont _font;
    private Rectangle _playButton;
    private Rectangle _howToPlayButton;
    private Rectangle _settingsButton;
    private MouseState _previousMouseState;

    public void Initialize()
    {
        _background = null; 
        _font = null; 
        _playButton = new Rectangle(300, 200, 200, 50);
        _howToPlayButton = new Rectangle(300, 300, 200, 50);
        _settingsButton = new Rectangle(300, 400, 200, 50);
        _previousMouseState = Mouse.GetState();
    }
    public void LoadContent(ContentManager content){
        _background = content.Load<Texture2D>("bg_0000_bg3");
        _font = content.Load<BitmapFont>("Arial");
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (_playButton.Contains(mouseState.Position))
            {
                SceneManager.Instance.SetScene(SceneManager.SceneType.Game);
            }
            else if (_howToPlayButton.Contains(mouseState.Position))
            {
                // Open How to Play
            }
            else if (_settingsButton.Contains(mouseState.Position))
            {
                // Open Settings
            }
        }
        _previousMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {

        // Draw background
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);

        // Draw buttons
        spriteBatch.DrawString(_font, "Play", new Vector2(_playButton.X, _playButton.Y), Color.White);
        spriteBatch.DrawString(_font, "How to Play", new Vector2(_howToPlayButton.X, _howToPlayButton.Y), Color.White);
        spriteBatch.DrawString(_font, "Settings", new Vector2(_settingsButton.X, _settingsButton.Y), Color.White);

    }
}
