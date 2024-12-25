using System;
using System.Collections.Generic;
using Klica;
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;
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
    private List<Enemy> _enemies;
    private Texture2D _background;
    private SpriteManager _spriteManager;
    private Random _random;
    private int _enemyCount = 0;
    private int _enemySpawnRate = 1;

    private CollisionManager _collisionManager;

    // za gumb
    private Rectangle _backButton;
    private Texture2D _buttonTexture;
    private BitmapFont _font;
    private MouseState _previousMouseState;


    //game state
    Game1 _game;
    public static int _gameScore;
    public bool _gameStateWin;
    public bool _gameStateLost;

    public GameScene(Game1 game)
    {
        _game = game; 
        _random = new Random();
        _enemies = new List<Enemy>();
        _collisionManager = new CollisionManager();
    }

    public void Initialize()
    {
        _gameplayRules = new GameplayRules(3600, 3);
        
    }

    public void LoadContent(ContentManager content)
    {   
        
        _backButton = new Rectangle(20, 20, 200, 50);
        System.Console.WriteLine("Loading game scene content");
        _background = content.Load<Texture2D>("bg_0000_bg3");
        var spriteSheet = content.Load<Texture2D>("SpriteInfo");
        _spriteManager = new SpriteManager(spriteSheet);

        var spriteDataLines = System.IO.File.ReadAllLines("Content/SpriteInfo.txt");
        SpriteFactory.Initialize(spriteSheet, _spriteManager, spriteDataLines);

        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, _gameplayRules, 20);
        _physicsEngine = new PhysicsEngine(_level);

        var food = new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f);
        _physicsEngine.AddFood(food);

        _player = new Player(_physicsEngine);
        _collisionManager.AddCollider(_player.GetBaseCollider(), HandlePlayerBaseCollision);
        _collisionManager.AddCollider(_player.GetMouthCollider(), HandlePlayerMouthCollision);

        if (_enemyCount < _enemySpawnRate){
            _enemyCount++;
            SpawnEnemies(1);
        }

        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _font = content.Load<BitmapFont>("Arial");

       
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new Color[] { Color.White });
    }

    public void Update(GameTime gameTime)
    {
        System.Console.WriteLine("Updating game scene");
        _player.UpdatePlayer(gameTime);
        
        // Check collisions with food
        foreach (var food in _physicsEngine._foodItems)
        {
            food.Update(gameTime, _level.Bounds, _player._position, ref _gameScore);
        }
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime, _player, _physicsEngine, ref _gameScore);
            ConstrainToBounds(enemy);
        }
         // Handle input for the back button
        _collisionManager.Update();
        _physicsEngine.Update(gameTime, _player._player_mouth._position, ref _gameScore);
        _gameStateWin = _gameplayRules.CheckWinCondition(_gameScore);
        _gameStateLost = _gameplayRules.CheckLoseCondition(_gameScore);
        if (_gameStateWin){
            System.Console.WriteLine("You won!");
        }
        if(_gameStateLost){
            System.Console.WriteLine("You lost!");
        }
        HandleInput();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        System.Console.WriteLine("Drawing game scene");
        _level.DrawBackground(spriteBatch);
        _physicsEngine.Draw(spriteBatch);
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch, _game.GetGameTime());
        }
        //System.Console.WriteLine("GameTime: " + _game.GetGameTime());
        _player.DrawPlayer(spriteBatch, _game.GetGameTime());
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
    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Vector2 spawnPosition = new Vector2(
            //     _random.Next(_level.Bounds.Left, _level.Bounds.Right),
            //     _random.Next(_level.Bounds.Top, _level.Bounds.Bottom)
            // );

            Base baseSprite = new Base(1);
            Eyes eyes = new Eyes(1);
            Mouth mouth = new Mouth(1);

            Enemy enemy = new Enemy(baseSprite, eyes, mouth, _random.Next(30, 70));
            // enemy._position = spawnPosition;

            _collisionManager.AddCollider(enemy.GetBaseCollider(), collider =>
            {
                if (collider == _player.GetMouthCollider())
                {
                    Console.WriteLine("Enemy hit by player mouth.");
                }
            });

            _collisionManager.AddCollider(enemy.GetMouthCollider(), collider =>
            {
                if (collider == _player.GetBaseCollider())
                {
                    // Enemy's mouth collides with player base
                    _player.TakeDamage(10);
                }
            });

            _enemies.Add(enemy);
        }
    }
    private void HandlePlayerBaseCollision(Collider other)
    {
        if (other.Owner is Food food && !food.IsConsumed)
        {
            food.OnConsumed(ref _gameScore);
        }
    }


    private void HandlePlayerMouthCollision(Collider other)
    {
        if (other.Owner is Enemy enemy)
        {
            Console.WriteLine("Player's mouth hits enemy base.");
        }
    }


    private void ConstrainToBounds(Enemy enemy)
    {
        var bounds = _level.Bounds;

        if (enemy._position.X < bounds.Left)
            enemy._position = new Vector2(bounds.Left, enemy._position.Y);
        else if (enemy._position.X > bounds.Right)
            enemy._position = new Vector2(bounds.Right, enemy._position.Y);

        if (enemy._position.Y < bounds.Top)
            enemy._position = new Vector2(enemy._position.X, bounds.Top);
        else if (enemy._position.Y > bounds.Bottom)
            enemy._position = new Vector2(enemy._position.X, bounds.Bottom);
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
