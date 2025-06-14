using System;
using System.Collections.Generic;
using Klica;
using System.Linq;
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

public class Level1IntroScene : IScene
{
    private Game1 _game;
    private Texture2D _introImage;
    private Rectangle _startButton;
    private BitmapFont _font;

    private int _levelNumber; // Controls which image to load and which level to start

    public Level1IntroScene(Game1 game)
    {
        _game = game;
    }
    public void SetLevelId(int levelId)
    {
        _levelNumber  = levelId;
        this.Update(gameTime: null); // Ensure the intro image is set based on the level ID
    }

    public void Initialize() { }

    public void LoadContent(ContentManager content)
    {
        // Map level 0 to level 1 intro
        int safeId = _levelNumber == 0 ? 1 : _levelNumber;

        if (safeId == 1 || safeId == 2)
        {
            string imageName = $"food_chain{safeId}";
            _introImage = content.Load<Texture2D>(imageName); // food_chain1 or food_chain2
        }
        else
        {
            Console.WriteLine($"Invalid intro level ID: {_levelNumber}");
            _introImage = new Texture2D(_game.GraphicsDevice, 1, 1);
            _introImage.SetData(new[] { Color.Black });
        }

        _font = content.Load<BitmapFont>("Arial");
        _startButton = new Rectangle(860, 900, 200, 60);
    }



    public void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && _startButton.Contains(mouse.Position))
        {
            switch (_levelNumber)
            {
                case 1:
                    var level1 = (Level1_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1);
                    level1.Initialize();
                    level1._isPaused = false;
                    SceneManager.Instance.SetScene(SceneManager.SceneType.Level1);
                    break;

                case 2:
                    var level2 = (Level2_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level2);
                    level2.Initialize();
                    level2._isPaused = false;
                    SceneManager.Instance.SetScene(SceneManager.SceneType.Level2);
                    break;

                // Add more cases as needed
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try { spriteBatch.End(); } catch { }
        spriteBatch.Begin();
        spriteBatch.Draw(_introImage, new Rectangle(0, 0, 1920, 1080), Color.White);
        spriteBatch.Draw(TextureGenerator.Pixel, _startButton, Color.Gray);
        spriteBatch.DrawString(_font, "Start Level", new Vector2(_startButton.X + 20, _startButton.Y + 15), Color.White);
        
    }
}
