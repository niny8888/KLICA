
using Klica.Classes;
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
    public GameTime gameTime;




    // Level, gameplay, and physics engine
    private Level _level;
    private GameplayRules _gameplayRules;
    private PhysicsEngine _physicsEngine;

    // Player and background
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
        SceneManager.Instance.AddScene(SceneManager.SceneType.MainMenu, new MenuScene());
        //SceneManager.Instance.AddScene(SceneManager.SceneType.Game, new GameScene());
        SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        SceneManager.Instance.GetCurrentScene().LoadContent(Content);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _background = Content.Load<Texture2D>("bg_0000_bg3");
        _spriteSheet = Content.Load<Texture2D>("SpriteInfo");
        
        
        
        _spriteManager = new SpriteManager(_spriteSheet);
        _gameplayRules = new GameplayRules(3600, 1);
        System.Console.WriteLine($"Current Directory: {System.IO.Directory.GetCurrentDirectory()}");
        var spriteDataLines = System.IO.File.ReadAllLines("Content/SpriteInfo.txt");
        SpriteFactory.Initialize(_spriteSheet, _spriteManager, spriteDataLines);
        

        var food = new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 50f);
        _player = new Player();
        _level = new Level(new Rectangle(0, 0, 1280, 720), _background,_gameplayRules,10);
        _physicsEngine = new PhysicsEngine(_level);
        
        
        
        _physicsEngine.AddFood(food);
        

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO
        _player.UpdatePlayer();
        int score = 0;
        _physicsEngine.Update(gameTime, _player._position, ref score);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
       GraphicsDevice.Clear(Color.CornflowerBlue);

        
        _spriteBatch.Begin();
        _level.DrawBackground(_spriteBatch);
        SceneManager.Instance.Draw(_spriteBatch);
        _physicsEngine.Draw(_spriteBatch);
        //_spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        //_spriteManager.DrawSpriteNamed(_spriteBatch,"spike_front");
        //_spriteManager.DrawSpriteNamed(_spriteBatch,"ustaD");
        //_spriteManager.DrawSprites(_spriteBatch);
        _player.DrawPlayer(_spriteBatch);
        //_physicsEngine.Draw(_spriteBatch);
        

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
