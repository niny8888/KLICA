using Klica;
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

public class GameScene : IScene
{
    private Level _level;
    private GameplayRules _gameplayRules;
    private PhysicsEngine _physicsEngine;
    private Player _player;
    private Texture2D _background;
    private SpriteManager _spriteManager;

    // za gumb
    private Rectangle _backButton;
    private Texture2D _buttonTexture;
    private BitmapFont _font;
    private MouseState _previousMouseState;
    Game1 _game;
    public static int _gameScore;
    public bool _gameStateWin;
    public bool _gameStateLost;

    public GameScene(Game1 game)
    {
        _game = game; 
    }
    public void Initialize()
    {
        _player = new Player(_physicsEngine);
        _gameplayRules = new GameplayRules(3600, 1);
        _backButton = new Rectangle(20, 20, 200, 50);
    }

    public void LoadContent(ContentManager content)
    {
        _background = content.Load<Texture2D>("bg_0000_bg3");
        var spriteSheet = content.Load<Texture2D>("SpriteInfo");
        _spriteManager = new SpriteManager(spriteSheet);

        var spriteDataLines = System.IO.File.ReadAllLines("Content/SpriteInfo.txt");
        SpriteFactory.Initialize(spriteSheet, _spriteManager, spriteDataLines);

        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, _gameplayRules, 10);
        _physicsEngine = new PhysicsEngine(_level);
        var food = new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 50f);
        _physicsEngine.AddFood(food);


       _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _font = content.Load<BitmapFont>("Arial");

       
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new Color[] { Color.White });
    }

    public void Update(GameTime gameTime)
{
    _player.UpdatePlayer(gameTime);
    
    // Check collisions with food
    foreach (var food in _physicsEngine._foodItems) // Assuming a collection of food objects in PhysicsEngine
    {
        if (!food.IsConsumed && Vector2.Distance(_player._position, food.Position) <= food.CollisionRadius)
        {
            food.OnConsumed(ref _gameScore);
        }
    }

    HandleInput(); // Handle input for the back button
    _physicsEngine.Update(gameTime, _player._position, ref _gameScore);
    _gameStateWin = _gameplayRules.CheckWinCondition(_gameScore);
    _gameStateLost = _gameplayRules.CheckLoseCondition(_gameScore);
}

    public void Draw(SpriteBatch spriteBatch)
    {
        _level.DrawBackground(spriteBatch);
        _physicsEngine.Draw(spriteBatch);
        _player.DrawPlayer(spriteBatch);
        DrawButton(spriteBatch, "Back to Menu", _backButton);
    
    }

    private void HandleInput()
    {
        MouseState mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (_backButton.Contains(mouseState.Position))
            {
                SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
                SceneManager.Instance.LoadContent(_game.Content);
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        }

        _previousMouseState = mouseState;
    }

    private void DrawButton(SpriteBatch spriteBatch, string text, Rectangle buttonRect)
    {
        spriteBatch.Draw(_buttonTexture, buttonRect, Color.Gray);

        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, 2), Color.Black); // Top border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Left border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X + buttonRect.Width - 2, buttonRect.Y, 2, buttonRect.Height), Color.Black); // Right border
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y + buttonRect.Height - 2, buttonRect.Width, 2), Color.Black); // Bottom border

        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(buttonRect.X + (buttonRect.Width - textSize.X) / 2, buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }
}
