
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static int ScreenWidth = 1920;
    public static int ScreenHeight= 1080;
    public SpriteManager _spriteManager;

    public Player _player;

    Texture2D _background;
    Texture2D _spriteSheet;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = ScreenWidth;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _background = Content.Load<Texture2D>("bg_0000_bg3");
        _spriteSheet = Content.Load<Texture2D>("SpriteInfo");
        //Player player = new Player(_spriteManager);

        _spriteManager = new SpriteManager(_spriteSheet);
        System.Console.WriteLine($"Current Directory: {System.IO.Directory.GetCurrentDirectory()}");

        var spriteDataLines = System.IO.File.ReadAllLines("Content/SpriteInfo.txt");
        _spriteManager = new SpriteManager(_spriteSheet);
        SpriteFactory.Initialize(_spriteSheet, _spriteManager, spriteDataLines);
        _player = new();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO
        _player.UpdatePlayer();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        
        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        _spriteManager.DrawSpriteNamed(_spriteBatch,"base_blue");
        //_spriteManager.DrawSpriteNamed(_spriteBatch,"ustaD");
        //_spriteManager.DrawSprites(_spriteBatch);
        //_spriteBatch.Draw(_player);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
