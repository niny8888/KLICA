using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using Klica.Classes;
using Klica;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

public class SettingsScene : IScene
{private Game1 _game;
    private BitmapFont _font;
    private Texture2D _background;

    private int _selectedOption = 0;
    private string[] _options = { "Resolution", "Graphics", "Fullscreen", "Volume" };
    private int _resolutionIndex = 1;
    private string[] _resolutions = { "800x600", "1280x720", "1920x1080" };
    private int _graphicsIndex = 1;
    private string[] _graphicsLevels = { "Low", "Medium", "High" };
    private bool _isFullscreen = false;
    private MouseState _previousMouseState;
    private float _volume = 1.0f;

    public SettingsScene(Game1 game)
    {
        _game = game;
    }

    public void Initialize()
    {
        _previousMouseState = Mouse.GetState();
    }

    public void LoadContent(ContentManager content)
    {
        _font = content.Load<BitmapFont>("Arial");
        _background = content.Load<Texture2D>("bg_0000_bg3"); // Load background
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        // Navigate settings with UP/DOWN
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            _selectedOption = (_selectedOption - 1 + _options.Length) % _options.Length;
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            _selectedOption = (_selectedOption + 1) % _options.Length;
        }

        // Adjust settings with LEFT/RIGHT
        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Right))
        {
            switch (_selectedOption)
            {
                case 0: // Resolution
                    if (keyboardState.IsKeyDown(Keys.Right)) _resolutionIndex = (_resolutionIndex + 1) % _resolutions.Length;
                    if (keyboardState.IsKeyDown(Keys.Left)) _resolutionIndex = (_resolutionIndex - 1 + _resolutions.Length) % _resolutions.Length;
                    ApplyResolution();
                    break;

                case 1: // Graphics
                    if (keyboardState.IsKeyDown(Keys.Right)) _graphicsIndex = (_graphicsIndex + 1) % _graphicsLevels.Length;
                    if (keyboardState.IsKeyDown(Keys.Left)) _graphicsIndex = (_graphicsIndex - 1 + _graphicsLevels.Length) % _graphicsLevels.Length;
                    break;

                case 2: // Fullscreen
                    if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.Left))
                    {
                        _isFullscreen = !_isFullscreen;
                        _game.IsFullscreen = _isFullscreen;
                        _game.ApplyResolutionSettings();

                    }
                    break;

                case 3: // Volume
                    if (keyboardState.IsKeyDown(Keys.Right)) _volume = MathHelper.Clamp(_volume + 0.1f, 0f, 1f);
                    if (keyboardState.IsKeyDown(Keys.Left)) _volume = MathHelper.Clamp(_volume - 0.1f, 0f, 1f);
                    break;
            }
        }

        // Return to menu
        if (_previousMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
        {
            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
            SceneManager.Instance.LoadContent(_game.Content); // Ensure content is reloaded
        }

        _previousMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.Clear(Color.DarkGray);
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);

        Vector2 position = new Vector2(200, 150);
        for (int i = 0; i < _options.Length; i++)
        {
            Color color = (i == _selectedOption) ? Color.Yellow : Color.White;
            string value = i switch
            {
                0 => _resolutions[_resolutionIndex],
                1 => _graphicsLevels[_graphicsIndex],
                2 => _isFullscreen ? "On" : "Off",
                3 => $"{_volume * 100:F0}%",
                _ => ""
            };

            spriteBatch.DrawString(_font, $"{_options[i]}: {value}", position, color);
            position.Y += 50;
        }

        spriteBatch.DrawString(_font, "[Arrow Keys] to navigate, [Left/Right] to change, Right Mouse to return", new Vector2(200, 400), Color.White);
    }

    private void ApplyResolution()
    {
        string[] parts = _resolutions[_resolutionIndex].Split('x');
        Game1.ScreenWidth = int.Parse(parts[0]);
        Game1.ScreenHeight = int.Parse(parts[1]);
        _game.ApplyResolutionSettings();
    }

}
