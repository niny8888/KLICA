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
 

public class Level2IntroScene : IScene
{
    private Game1 _game;
    private Texture2D _introImage;
    private Rectangle _startButton;
    private SpriteFont _font;

    public Level2IntroScene(Game1 game)
    {
        _game = game;
    }

    public void Initialize() { }

    public void LoadContent(ContentManager content)
    {
        _introImage = content.Load<Texture2D>("food_chain2");
        _font = _game.Content.Load<SpriteFont>("Arial");
        _startButton = new Rectangle(860, 900, 200, 60);
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && _startButton.Contains(mouse.Position))
        {
            var level2 = (Level2_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level2);
            level2.Initialize();
            level2._isPaused = false;
            SceneManager.Instance.SetScene(SceneManager.SceneType.Level2);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try { spriteBatch.End(); } catch { }
        spriteBatch.Begin(); 
        spriteBatch.Draw(_introImage, new Rectangle(0, 0, 1920, 1080), Color.White);

        // Button background: White
        spriteBatch.Draw(TextureGenerator.Pixel, _startButton, Color.White);

        // Text: Blue
        var text = "Start Level";
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPos = new Vector2(
            _startButton.X + (_startButton.Width - textSize.X) / 2,
            _startButton.Y + (_startButton.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(_font, text, textPos, Color.SkyBlue);
    }

}
