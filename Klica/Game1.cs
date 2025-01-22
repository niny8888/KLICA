
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Klica;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static int ScreenWidth = 1920;
    public static int ScreenHeight= 1080;
    public SpriteManager _spriteManager;
    public GameTime gameTime;

    public SoundEffectInstance bg_sound;
    


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

        bg_sound = Content.Load<SoundEffect>("song-bg").CreateInstance();
        bg_sound.IsLooped = true;
        bg_sound.Play();
        
        SceneManager.Instance.AddScene(SceneManager.SceneType.MainMenu, new MenuScene(this));
        SceneManager.Instance.AddScene(SceneManager.SceneType.Game, new GameScene(this));

        // Set the initial scene
        SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        SceneManager.Instance.LoadContent(Content);
    }

    public GameTime GetGameTime()
    {
        return gameTime;
    }

    protected override void Update(GameTime gameTime)
    {
        this.gameTime = gameTime;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        SceneManager.Instance.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        SceneManager.Instance.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
