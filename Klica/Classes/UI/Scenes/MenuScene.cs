using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using Klica.Classes;
using Klica;
using Microsoft.Xna.Framework.Audio;
using Klica.Classes.Objects_sprites;

public class MenuScene : IScene
{
    private enum MenuState
    {
        MainMenu,
        HowToPlay,
        Settings

    }

    private Texture2D _background;
    private Texture2D _background_HTP;
    private BitmapFont _font;
    private Rectangle _playButton;
    private Rectangle _howToPlayButton;
    private Rectangle _settingsButton;
    private MouseState _previousMouseState;
    private Rectangle _newGameButton;

    private MenuState _currentState;

    private Texture2D _buttonTexture;
    private bool isShowingIntro = false;
    private double introStartTime;
    private Texture2D introImage; // Load your intro image
    private bool isGameStarting = false;


    private Game1 _game;
    private bool _contentLoaded = false;

    private SoundEffectInstance sound_menu_click;
    private int Button_offset_width=Game1.ScreenWidth/2-100;

    //gumbi
    private Rectangle _srcNewGame = new Rectangle(60, 118, 493, 330);
    private Rectangle _srcMenu = new Rectangle(610, 118, 590, 330);
    private Rectangle _srcHowToPlay = new Rectangle(60, 475, 450, 300);
    private Rectangle _srcSettings = new Rectangle(675, 500, 450, 300);
    private Rectangle _srcBack = new Rectangle(610, 415, 590, 330);

    private Rectangle _game_copyright = new Rectangle(1392, 500, 210, 230);


    public MenuScene(Game1 game)
    {
        _game = game; 
    }

    public void Initialize()
    {
        SetButtonPosition();
        _previousMouseState = Mouse.GetState();
        _currentState = MenuState.MainMenu;
    }

    private void SetButtonPosition()
    {
        int spacing = -100;
        int startY = 100;

        // Calculate X so each button is centered based on its width
        _newGameButton = new Rectangle(
            (Game1.ScreenWidth / 2 - _srcNewGame.Width / 2)+30,
            startY,
            _srcNewGame.Width,
            _srcNewGame.Height);

        _playButton = new Rectangle(
            Game1.ScreenWidth / 2 - _srcMenu.Width / 2,
            _newGameButton.Bottom + spacing -40,
            _srcMenu.Width,
            _srcMenu.Height);

        _howToPlayButton = new Rectangle(
            Game1.ScreenWidth / 2 - _srcHowToPlay.Width / 2,
            _playButton.Bottom + spacing-60,
            _srcHowToPlay.Width,
            _srcHowToPlay.Height);

        _settingsButton = new Rectangle(
            Game1.ScreenWidth / 2 - _srcSettings.Width / 2,
            _howToPlayButton.Bottom + spacing +10,
            _srcSettings.Width,
            _srcSettings.Height);
    }





    public void LoadContent(ContentManager content)
    {
        
        try
        {
            //Console.WriteLine("Loading background...");
            _background = content.Load<Texture2D>("menu_BG");
            _background_HTP = content.Load<Texture2D>("howtoplay_BG");
            //Console.WriteLine("Background loaded successfully!");

            //Console.WriteLine("Loading font...");
            _font = content.Load<BitmapFont>("Arial");
            //Console.WriteLine("Font loaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading content: {ex.Message}");
        }

        sound_menu_click= content.Load<SoundEffect>("SE_menu").CreateInstance();
        _buttonTexture = content.Load<Texture2D>("menu_buttons");

        //_buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _font = content.Load<BitmapFont>("Arial");
        introImage = content.Load<Texture2D>("intro");

       
        // _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        // _buttonTexture.SetData(new Color[] { Color.White });
        _contentLoaded = true;
    }

   public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
        
        if (_currentState == MenuState.MainMenu)
        {
            // Hover detection and button size adjustment
            _playButton = GetButtonWithHoverEffect(_playButton, mouseState.Position);
            _howToPlayButton = GetButtonWithHoverEffect(_howToPlayButton, mouseState.Position);
            _settingsButton = GetButtonWithHoverEffect(_settingsButton, mouseState.Position);
            _newGameButton = GetButtonWithHoverEffect(_newGameButton, mouseState.Position);

            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                // if (_playButton.Contains(mouseState.Position))
                // {
                //     sound_menu_click.Play();
                //     var level1Intro = (Level1IntroScene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1Intro);
                //     level1Intro.SetLevelId(1); // Set level ID for Level 1 Intro
                //     SceneManager.Instance.SetScene(SceneManager.SceneType.Level1Intro);

                //     // var level1 = (Level1_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1);
                //     // level1.LoadFromSave();
                //     // SceneManager.Instance.SetScene(SceneManager.SceneType.Level1);
                // }
                if (_playButton.Contains(mouseState.Position))
                {
                    sound_menu_click.Play();
                    var data = SaveManager.Load();

                    if (data.Traits != null && data.Traits.Count > 0)
                    {
                        _game.CurrentPlayer ??= new Player(null); // fallback if player not created yet
                        _game.CurrentPlayer.LoadTraits(data.Traits);
                    }



                    int nextLevel = data.LastCompletedLevel;

                    switch (nextLevel)
                    {
                        case 0:
                            // If no levels completed, start with Level 1 Intro
                            var level1Intro = (Level1IntroScene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1Intro);
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level1Intro);
                            break;
                        case 1:
                            var level1 = (Level1_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1);
                            level1.Initialize();
                            level1._isPaused = false;
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level1);
                            break;

                        case 2:
                            var level2Intro = (Level2IntroScene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level2Intro);
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level2Intro);
                            break;

                        case 3:
                            var level3 = (Level3_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level3);
                            level3.Initialize();
                            level3._isPaused = false;
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level3);
                            break;

                        case 4:
                            var level4 = (Level4_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level4);
                            level4._isPaused = false;
                            level4.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level4);
                            break;

                        case 5:
                            var level5 = (Level5_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level5);
                            level5._isPaused = false;
                            level5.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level5);
                            break;

                        case 6:
                            var level6 = (Level6_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level6);
                            level6._isPaused = false;
                            level6.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level6);
                            break;
                        case 7: 
                            var level7 = (Level7_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level7);
                            level7._isPaused = false;
                            level7.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level7);
                            break;
                        case 8:
                            var level8 = (Level8_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level8);
                            level8._isPaused = false;
                            level8.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level8);
                            break;
                        case 9:
                            var level9 = (Level9_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level9);
                            level9._isPaused = false;
                            level9.Initialize();
                            SceneManager.Instance.SetScene(SceneManager.SceneType.Level9);
                            break;
                        default:
                            // All levels completed – go to Main Menu or credits
                            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
                            break;
                    }

                }


                else if (_howToPlayButton.Contains(mouseState.Position))
                {
                    sound_menu_click.Play();
                    _currentState = MenuState.HowToPlay;
                }
                else if (_settingsButton.Contains(mouseState.Position))
                {
                    sound_menu_click.Play();
                    var settings = (SettingsScene)SceneManager.Instance.GetScene(SceneManager.SceneType.SettingsScene);
                    settings.SetCaller(SceneManager.SceneType.MainMenu);
                    SceneManager.Instance.SetScene(SceneManager.SceneType.SettingsScene);

                }
                else if (_newGameButton.Contains(mouseState.Position))
                {
                    sound_menu_click.Play();
                    var data = SaveManager.Load();
                    data.LastCompletedLevel = 0; // Reset to level 0
                    var level1Intro = (Level1IntroScene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1Intro);
                    SceneManager.Instance.SetScene(SceneManager.SceneType.Level1Intro);

                    // SaveManager.Reset(); // Clear old save
                    // var level1 = (Level1_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1);
                    // level1.Initialize();  
                    // level1._gameScore=0;
                    // level1.SaveGameState(); // Save after setup so positions are included
                    // SceneManager.Instance.SetScene(SceneManager.SceneType.Level1);
                }
            }
        }
        else if (mouseState.RightButton == ButtonState.Pressed)
        {
            _currentState = MenuState.MainMenu;
        }

        _previousMouseState = mouseState;
    }
    private Rectangle GetButtonWithHoverEffect(Rectangle buttonRect, Point mousePosition)
    {
        // Hover detection
        bool isHovered = buttonRect.Contains(mousePosition);

        // Scale factor for hover effect
        float scaleFactor = isHovered ? 1.1f : 1.0f; // 1.1 for scaling up, 1.0 for original size

        // Calculate the new width and height based on scale factor
        int newWidth = (int)(buttonRect.Width * scaleFactor);
        int newHeight = (int)(buttonRect.Height * scaleFactor);

        // Ensure the button stays centered
        int newX = buttonRect.X - (newWidth - buttonRect.Width) / 2;
        int newY = buttonRect.Y - (newHeight - buttonRect.Height) / 2;

        return new Rectangle(newX, newY, newWidth, newHeight);
    }





    public void Draw(SpriteBatch spriteBatch)
    {
        Button_offset_width=Game1.ScreenWidth/2-100;
        SetButtonPosition();
        if (!_contentLoaded)
        return; 
        
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
        // Get the screen dimensions
        var screenWidth = Game1.ScreenWidth;
        var screenHeight = Game1.ScreenHeight;

        // Draw the background scaled to fit the entire screen
        spriteBatch.Draw(_background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

        // Draw buttons
        DrawButton(spriteBatch, _newGameButton, _srcNewGame);
        DrawButton(spriteBatch, _playButton, _srcMenu);         // "Menu" = Play
        DrawButton(spriteBatch, _howToPlayButton, _srcHowToPlay);
        DrawButton(spriteBatch, _settingsButton, _srcSettings);
        
    }



    private void DrawHowToPlay(SpriteBatch spriteBatch)
    {
        var screenWidth = Game1.ScreenWidth;
        var screenHeight = Game1.ScreenHeight;

        // Draw the background scaled to fit the entire screen
        spriteBatch.Draw(_background_HTP, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

        spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.DrawString(_font, "How to Play Placeholder Text", new Vector2(Game1.ScreenWidth/2 -150, 450), Color.SkyBlue);
        spriteBatch.DrawString(_font, "Right-click to return", new Vector2(Game1.ScreenWidth/2 -150, 500), Color.SkyBlue);
    }

    private void DrawSettings(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.Clear(Color.DarkGray);
        spriteBatch.DrawString(_font, "Settings Placeholder", new Vector2(250, 250), Color.White);
        spriteBatch.DrawString(_font, "Right-click to return", new Vector2(250, 300), Color.White);
    }
    

    // private void DrawButton(SpriteBatch spriteBatch, string text, Rectangle buttonRect)
    // {
    //     spriteBatch.Draw(_buttonTexture, buttonRect, Color.White); // Button background (white)
        
    //     spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, 2), Color.Black); // Top border
    //     spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Left border
    //     spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X + buttonRect.Width - 2, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Right border
    //     spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y + buttonRect.Height - 2, buttonRect.Width, 2), Color.Black); // Bottom border

    //     Vector2 textSize = _font.MeasureString(text);
    //     Vector2 textPosition = new Vector2(buttonRect.X + (buttonRect.Width - textSize.X) / 2, buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);
    //     spriteBatch.DrawString(_font, text, textPosition, Color.Black); // Draw text
    // }
    private void DrawButton(SpriteBatch spriteBatch, Rectangle destRect, Rectangle srcRect)
    {
        spriteBatch.Draw(_buttonTexture, destRect, srcRect, Color.White);
    }
    private Rectangle GetScaledButtonRectangle(Rectangle original)
    {
        // Scale factor for hover effect
        float scaleFactor = 1.1f;

        // Calculate the new width and height
        int newWidth = (int)(original.Width * scaleFactor);
        int newHeight = (int)(original.Height * scaleFactor);

        // Center the button around its original position
        int newX = original.X - (newWidth - original.Width) / 2;
        int newY = original.Y - (newHeight - original.Height) / 2;

        return new Rectangle(newX, newY, newWidth, newHeight);
    }

    private Rectangle GetOriginalButtonRectangle(Rectangle original)
    {
        return original;
    }


}
