
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Klica;
public enum GameState
{
    Intro,
    Menu,
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static int ScreenWidth = 1920;
    public static int ScreenHeight= 1080;
    public bool IsFullscreen = false;
    public SpriteManager _spriteManager;
    public GameTime gameTime;
    public int CurrentLevel { get; set; } = 1; // Start from Level 1
    public Player CurrentPlayer { get; set; }


    public SoundEffectInstance bg_sound;
    private GameState currentState;
    
    //intro
    private MyLogoAnimation animation;
    private double introDuration;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        ApplyResolutionSettings();

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

        bg_sound = Content.Load<SoundEffect>("song-bg").CreateInstance();
        bg_sound.IsLooped = true;
        bg_sound.Play();
        

        SceneManager.Instance.AddScene(SceneManager.SceneType.MainMenu, new MenuScene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Game, new GameScene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.SettingsScene, new SettingsScene(this, SceneManager.SceneType.MainMenu));
        
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level1, new Level1_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level2, new Level2_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level3, new Level3_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level4, new Level4_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level5, new Level5_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level6, new Level6_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level7, new Level7_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level8, new Level8_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level9, new Level9_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level10, new Level10_Scene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.EvolutionScene, new EvolutionScene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level1Intro, new Level1IntroScene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Level2Intro, new Level2IntroScene(this));

        // Set the initial scene
        SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        SceneManager.Instance.LoadContent(Content);

        TextureGenerator.Init(GraphicsDevice);

        

        //intro
        animation = new MyLogoAnimation(0.1);
        string[] frameNames = new string[]
        {
            "00001", "00002", "00003", "00004", "00005", // Add all your frame names here
            "00006", "00007", "00008", "00009", "00010",
            "00011", "00012", "00013", "00014", "00015",
            "00016", "00017", "00018", "00019", "00020",
            "00021", "00022", "00023", "00024", "00025",
            "00025", "00025", "00025", "00025", "00025"
        };
        animation.LoadFrames(Content, frameNames);
        currentState = GameState.Intro;
        
        
    }

    public GameTime GetGameTime()
    {
        return gameTime;
    }

    public void ApplyResolutionSettings()
    {
        _graphics.PreferredBackBufferWidth = ScreenWidth;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
        _graphics.IsFullScreen = IsFullscreen;
        _graphics.ApplyChanges();
    }
    protected override void Update(GameTime gameTime)
    {
        this.gameTime = gameTime;
        switch (currentState)
        {
            case GameState.Intro:
                animation.Update(gameTime);
                if (animation.IsFinished())
                {
                    currentState = GameState.Menu;
                }
                break;

            case GameState.Menu:
                // Handle menu logic here (e.g., button presses, menu interactions)
                break;
        }
        // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        // {
        //     ((Level1_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level1))?.SaveGameState();
        //     Exit();
        // }
            

        SceneManager.Instance.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        switch (currentState)
        {
            case GameState.Intro:
                int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Get the width and height of the current frame (the first frame, for example)
            Texture2D currentFrame = animation.GetCurrentFrame();
            int frameWidth = currentFrame.Width;
            int frameHeight = currentFrame.Height;

            // Calculate the scale based on screen dimensions and frame size
            Vector2 scale = new Vector2(screenWidth / (float)frameWidth, screenHeight / (float)frameHeight);
            
            // Draw the intro animation, scaled to fit the screen
            animation.Draw(_spriteBatch, new Vector2(0, 0), scale);
            break;

            case GameState.Menu:
                SceneManager.Instance.Draw(_spriteBatch);
                break;
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
