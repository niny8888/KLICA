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
    private int _enemySpawnRate = 5;

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

    //Shader
    private Effect _waterFlowEffect;
    private Effect _perlinNoiseEffect;
    private HUD _hud;

    //trail
    private List<HalfCircleTrail> _trails = new List<HalfCircleTrail>();
    private Dictionary<Enemy, List<HalfCircleTrail>> _enemyTrails = new();

    private Texture2D _halfCircleTexture;
    private float _trailTimer = 0f;
    

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
        _halfCircleTexture = TextureGenerator.CreateCircleRadiusLineTexture(_game.GraphicsDevice, 50); // Radius 50

        var spriteDataLines = System.IO.File.ReadAllLines("Content/SpriteInfo.txt");
        SpriteFactory.Initialize(spriteSheet, _spriteManager, spriteDataLines);

        

        _waterFlowEffect = content.Load<Effect>("WaterFlow");
        _waterFlowEffect.Parameters["DistortionStrength"].SetValue(0.005f);
        _waterFlowEffect.Parameters["Frequency"].SetValue(0.005f);

        _perlinNoiseEffect = content.Load<Effect>("PerlinNoise");
        _perlinNoiseEffect.Parameters["seed"].SetValue(714.434f);
        _perlinNoiseEffect.Parameters["lineValueLimit"].SetValue(0.005f); // Adjust for less intense lines
        _perlinNoiseEffect.Parameters["lineColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f)); // White lines
        _perlinNoiseEffect.Parameters["lineAlpha"].SetValue(0.5f); // Adjust for less intense lines

        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, _gameplayRules, 20);
        _physicsEngine = new PhysicsEngine(_level);

        var food = new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f);
        _physicsEngine.AddFood(food);

        _player = new Player(_physicsEngine);
        _collisionManager.AddCollider(_player.GetBaseCollider(), HandlePlayerBaseCollision);
        _collisionManager.AddCollider(_player.GetMouthCollider(), HandlePlayerMouthCollision);

        // if (_enemyCount < _enemySpawnRate){
        //     _enemyCount++;
        //     SpawnEnemies(1);
        // }
        SpawnEnemies(_enemySpawnRate);

        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _font = content.Load<BitmapFont>("Arial");
        _hud = new HUD(_font);
       
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new Color[] { Color.White });
    }
private Vector2 _shaderTime = Vector2.Zero;
private Vector2 _shaderPerlinTime = Vector2.Zero;

    public void Update(GameTime gameTime)
    {
        System.Console.WriteLine("Updating game scene");
        _player.UpdatePlayer(gameTime);
        // Add trail behind the player every 3 seconds
        _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_trailTimer >= 0.5f)
        {
            _trailTimer = 0f;
            _trails.Add(new HalfCircleTrail(
                _player._position, // Position of the player
                10f,               // Initial radius
                50f,               // Maximum radius
                2f,                // Lifespan in seconds
                _player.GetRotation() // Player's current rotation
            ));
        }

        foreach (var trail in _trails)
        {
            trail.Update(gameTime);
        }
        _trails.RemoveAll(trail => trail.IsExpired);

        // Check collisions with food
        foreach (var food in _physicsEngine._foodItems)
        {
            food.Update(gameTime, _level.Bounds, _player._position, ref _gameScore);
        }
        foreach (var enemy in _enemies)
        {
            // Add a trail for each enemy every 0.5 seconds
            if (!_enemyTrails.ContainsKey(enemy))
            {
                _enemyTrails[enemy] = new List<HalfCircleTrail>();
            }

            // Add a trail to the current enemy
            _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_trailTimer >= 0.5f)
            {
                _trailTimer = 0f;
                _enemyTrails[enemy].Add(new HalfCircleTrail(
                    enemy._position,
                    10f,                    // Initial radius
                    50f,                    // Maximum radius
                    2f,                     // Lifespan in seconds
                    enemy.GetRotation()     // Enemy's rotation
                ));
            }

            // Update trails for the enemy
            foreach (var trail in _enemyTrails[enemy])
            {
                trail.Update(gameTime);
            }

            // Remove expired trails
            _enemyTrails[enemy].RemoveAll(trail => trail.IsExpired);

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
        
        
        _shaderTime.X += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _shaderTime.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

        _shaderPerlinTime.X += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _perlinNoiseEffect.Parameters["iTime"].SetValue(_shaderPerlinTime.X);

        _waterFlowEffect.Parameters["Time"].SetValue(_shaderTime);
        HandleInput();
    }
    

   public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        // Draw the background with the water flow effect
        spriteBatch.Begin(effect: _waterFlowEffect,  samplerState: SamplerState.LinearWrap);
        _level.DrawBackground(spriteBatch);
        spriteBatch.End();

        // Draw other game elements
        spriteBatch.Begin();
        _physicsEngine.Draw(spriteBatch);
        
        _enemies.ForEach(enemy => enemy.Update(_game.GetGameTime(), _player, _physicsEngine, ref _gameScore));
        _enemies.ForEach(enemy => enemy.Draw(spriteBatch, _game.GetGameTime()));
        foreach (var enemyTrailPair in _enemyTrails)
        {
            foreach (var trail in enemyTrailPair.Value)
            {
                trail.Draw(spriteBatch, _halfCircleTexture);
            }
        }

       
        

        _player.DrawPlayer(spriteBatch, _game.GetGameTime());

        foreach (var trail in _trails)
        {
            trail.Draw(spriteBatch, _halfCircleTexture);
        }
        foreach (var enemyTrailPair in _enemyTrails)
        {
            foreach (var trail in enemyTrailPair.Value)
            {
                trail.Draw(spriteBatch, _halfCircleTexture);
            }
        }

        spriteBatch.End();

        spriteBatch.Begin(effect: _perlinNoiseEffect, blendState: BlendState.AlphaBlend);
        spriteBatch.Draw(
            _background, // Use a dummy texture for full-screen effect
            new Rectangle(0, 0, 1920, 1080),
            Color.White
        );
        
        spriteBatch.End();

        spriteBatch.Begin();
        DrawButton(spriteBatch, "Back to Menu", _backButton);
        _hud.Draw(spriteBatch, _player, _enemies);
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
        spriteBatch.Draw(_buttonTexture, buttonRect, Color.White);
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, 2), Color.Black);
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y, 2, buttonRect.Height), Color.Black);
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X + buttonRect.Width - 2, buttonRect.Y, 2, buttonRect.Height), Color.Black);
        spriteBatch.Draw(_buttonTexture, new Rectangle(buttonRect.X, buttonRect.Y + buttonRect.Height - 2, buttonRect.Width, 2), Color.Black);

        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(buttonRect.X + (buttonRect.Width - textSize.X) / 2, buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }
}
