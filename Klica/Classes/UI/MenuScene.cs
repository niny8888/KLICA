using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

public class MenuScene : IScene
{
    private enum MenuState
    {
        MainMenu,
        HowToPlay,
        Settings
    }

    private Texture2D _background;
    private BitmapFont _font;
    private Rectangle _playButton;
    private Rectangle _howToPlayButton;
    private Rectangle _settingsButton;
    private MouseState _previousMouseState;

    private MenuState _currentState;

    private Texture2D _buttonTexture;

    private Game _game;

    public MenuScene(Game game)
    {
        _game = game; 
    }

    public void Initialize()
    {
        _background = null;
        _font = null;
        _playButton = new Rectangle(300, 200, 200, 50);
        _howToPlayButton = new Rectangle(300, 300, 200, 50);
        _settingsButton = new Rectangle(300, 400, 200, 50);
        _previousMouseState = Mouse.GetState();
        _currentState = MenuState.MainMenu;
    }

    public void LoadContent(ContentManager content)
    {
        
        _background = content.Load<Texture2D>("bg_0000_bg3");
        _font = content.Load<BitmapFont>("Arial");

       
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new Color[] { Color.White });
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        if (_currentState == MenuState.MainMenu)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                if (_playButton.Contains(mouseState.Position))
                {
                    SceneManager.Instance.SetScene(SceneManager.SceneType.Game);
                }
                else if (_howToPlayButton.Contains(mouseState.Position))
                {
                    _currentState = MenuState.HowToPlay;
                }
                else if (_settingsButton.Contains(mouseState.Position))
                {
                    _currentState = MenuState.Settings;
                }
            }
        }
        else if (mouseState.RightButton == ButtonState.Pressed)
        {
            _currentState = MenuState.MainMenu;
        }

        _previousMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        switch (_currentState)
        {
            case MenuState.MainMenu:
                DrawMainMenu(spriteBatch);
                break;

            case MenuState.HowToPlay:
                DrawHowToPlay(spriteBatch);
                break;

            case MenuState.Settings:
                DrawSettings(spriteBatch);
                break;
        }
    }

    private void DrawMainMenu(SpriteBatch spriteBatch)
    {

        spriteBatch.Draw(_background, Vector2.Zero, Color.White);

        DrawButton(spriteBatch, "Play", _playButton);
        DrawButton(spriteBatch, "How to Play", _howToPlayButton);
        DrawButton(spriteBatch, "Settings", _settingsButton);
    }

    private void DrawHowToPlay(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.DrawString(_font, "How to Play Placeholder Text", new Vector2(200, 250), Color.White);
        spriteBatch.DrawString(_font, "Right-click to return", new Vector2(200, 300), Color.White);
    }

    private void DrawSettings(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.Clear(Color.DarkGray);
        spriteBatch.DrawString(_font, "Settings Placeholder", new Vector2(250, 250), Color.White);
        spriteBatch.DrawString(_font, "Right-click to return", new Vector2(250, 300), Color.White);
    }

    private void DrawButton(SpriteBatch spriteBatch, string text, Rectangle buttonRect)
    {
        spriteBatch.Draw(_buttonTexture, buttonRect, Color.White); // Button background (white)
        
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, 2), Color.Black); // Top border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Left border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X + buttonRect.Width - 2, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Right border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y + buttonRect.Height - 2, buttonRect.Width, 2), Color.Black); // Bottom border

        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(buttonRect.X + (buttonRect.Width - textSize.X) / 2, buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, text, textPosition, Color.Black); // Draw text
    }
}
