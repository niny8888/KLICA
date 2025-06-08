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
using Microsoft.Xna.Framework.Audio;

public class Level2_Scene : IScene
{
    private Game1 _game;
    private Level _level;
    private Player _player;
    private HUD _hud;
    private PhysicsEngine _physicsEngine;
    private CollisionManager _collisionManager;

    private List<PeacefulEnemy> _peacefulEnemies { get; } = new();
    private List<Enemy> _aggressiveEnemies;

    private Texture2D _background, _halfCircleTexture, _buttonTexture, _winTexture, _loseTexture;
    private BitmapFont _font;
    private Rectangle _backButton;

    private int _foodGoal = 10;
    private int _peacefulEnemyCount = 3;
    private float _trailTimer = 0f;
    private List<HalfCircleTrail> _trails = new();
    private bool _gameStateWin = false;
    private bool _gameStateLost = false;
    public int _gameScore = 0;

    private Texture2D _circleTexture;
    private MouseState _previousMouseState;

    //options
    public bool _isPaused = false;
    private Rectangle _resumeButton, _settingsButton, _mainMenuButton, _exitButton;

    private double _autosaveTimer = 0;
    private Camera2D _camera;

    public Level2_Scene(Game1 game)
    {
        _game = game;
    }

    private void SetupSystems()
    {
        _collisionManager = new CollisionManager();
        _physicsEngine = new PhysicsEngine(_level);
        _player = _game.CurrentPlayer;

        _peacefulEnemies.Clear();
        _aggressiveEnemies = new List<Enemy>();

    }


    public void Initialize()
    {
        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, new GameplayRules(3600, 3), 20);
        SetupSystems();


        _physicsEngine.ClearFood();
        _peacefulEnemies.Clear();

        for (int i = 0; i < _peacefulEnemyCount; i++)
        {
            var peaceful = new PeacefulEnemy(new Base(2), new Eyes(2), new Mouth(2));
            _peacefulEnemies.Add(peaceful);
        }
        // Add 1 aggressive enemy
        _aggressiveEnemies = new List<Enemy>();
        _aggressiveEnemies.Add(new Enemy(new Base(1), new Eyes(1), new Mouth(1), aggressionLevel: 100));

        //_player._canDash = true;
        _player._canDash = true;
        _player._health = _player._maxhealth;

        _camera = new Camera2D(Game1.ScreenWidth, Game1.ScreenHeight, 1920, 1080);

        RegisterEnemyColliders();
        _physicsEngine.AddFood(new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f));
    }




    public void LoadContent(ContentManager content)
    {
        _background = content.Load<Texture2D>("menu_BG");
        _halfCircleTexture = TextureGenerator.CreateCircleRadiusLineTexture(_game.GraphicsDevice, 50);


        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, new GameplayRules(3600, 3), 20);
        _physicsEngine = new PhysicsEngine(_level);

        _player = new Player(_physicsEngine);
        // Font & HUD
        _font = content.Load<BitmapFont>("Arial");
        _hud = new HUD(_font);

        // Textures
        _winTexture = content.Load<Texture2D>("win");
        _loseTexture = content.Load<Texture2D>("lose");

        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });

        // Collider circle texture (optional, for debug)
        _circleTexture = Collider.CreateCircleTexture(_game.GraphicsDevice, 50, Color.White);

        // Back button rectangle
        _backButton = new Rectangle(20, 20, 200, 50);

        // _peacefulEnemies.Clear();
        // for (int i = 0; i < _peacefulEnemyCount; i++)
        // {
        //     var peaceful = new PeacefulEnemy(new Base(1), new Eyes(1), new Mouth(1));
        //     _peacefulEnemies.Add(peaceful);
        // }

        // _physicsEngine.AddFood(new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f));

        // ... other asset loading ...
    }


    public void Update(GameTime gameTime)
    {
        _game.CurrentLevel = 2;
        if (_isPaused)
        {
            HandlePauseMenuInput();
            return;
        }
        _autosaveTimer += gameTime.ElapsedGameTime.TotalSeconds;
    
        if (_gameStateWin || _gameStateLost)
            return;

        _player.UpdatePlayer(gameTime, _level.Bounds);

        // Player trail
        _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_trailTimer >= 0.5f)
        {
            _trailTimer = 0f;
            _trails.Add(new HalfCircleTrail(_player._position, 10f, 50f, 2f, _player.GetRotation()));
        }

        _trails.ForEach(t => t.Update(gameTime));
        _trails.RemoveAll(t => t.IsExpired);

        // Peaceful enemies
        foreach (var peacefulenemy in _peacefulEnemies)
        {
            peacefulenemy.Update(gameTime, _peacefulEnemies, _physicsEngine, _player, null);
        }
        foreach (var enemy in _aggressiveEnemies)
        {
            enemy.Update(gameTime, _physicsEngine, _player);
            ConstrainToBounds(enemy);
        }

        _collisionManager.Update();
        _physicsEngine.Update(gameTime, _player._player_mouth._position, ref _gameScore, _player, _aggressiveEnemies);

        _gameStateWin = _gameScore >= _foodGoal;
        if (_gameStateWin)
        {
            SaveGameState();
            SceneManager.Instance.SetScene(SceneManager.SceneType.EvolutionScene);
        }
        _gameStateLost = _player._health <= 0;
        if (_gameStateLost)
        {
            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        }
        _camera.Follow(_player._position);

        HandleInput();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try { spriteBatch.End(); } catch { }
        spriteBatch.Begin();
        if (_isPaused)
        {
            DrawPauseMenu(spriteBatch);
            return;
        }
        try { spriteBatch.End(); } catch { }
        spriteBatch.Begin(transformMatrix: _camera.Transform);
        _level.DrawBackground(spriteBatch);
    
        _physicsEngine.Draw(spriteBatch);

        foreach (var trail in _trails)
            trail.Draw(spriteBatch, _halfCircleTexture);

        foreach (var peaceful in _peacefulEnemies)
        {
            peaceful.Draw(spriteBatch, _game.GetGameTime());
            peaceful.Draw(spriteBatch, _game.GetGameTime());
            peaceful.DrawHealthBar(spriteBatch);
        }
        foreach (var enemy in _aggressiveEnemies)
        {
            enemy.Draw(spriteBatch, _game.GetGameTime());
            enemy.DrawHealthBar(spriteBatch);
        }

        _player.DrawHealthBar(spriteBatch);
        _player.DrawPlayer(spriteBatch, _game.GetGameTime());

        //DrawButton(spriteBatch, "Back to Menu", _backButton);
        spriteBatch.End();
        spriteBatch.Begin();
        DrawCheckpointBar(spriteBatch, _gameScore, _foodGoal);



        // if (_gameStateWin || _gameStateLost)
        //     DrawGameOverOverlay(spriteBatch);
        spriteBatch.End();


    }

    private void HandleInput()
    {
        MouseState mouseState = Mouse.GetState();

        if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !_previousMouseState.Equals(Mouse.GetState()))
        {
            _isPaused = !_isPaused;
            if (_isPaused);
        }

        _previousMouseState = mouseState;
    }

    private void DrawButton(SpriteBatch spriteBatch, string text, Rectangle rect)
    {
        spriteBatch.Draw(_buttonTexture, rect, Color.White);

        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new(rect.X + (rect.Width - textSize.X) / 2, rect.Y + (rect.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }

    private void DrawCheckpointBar(SpriteBatch spriteBatch, int currentScore, int maxScore)
    {
        int boxSize = 30;
        int spacing = 5;
        int totalWidth = maxScore * boxSize + (maxScore - 1) * spacing;

        Vector2 position = new Vector2(
            (Game1.ScreenWidth - totalWidth) / 2,
            Game1.ScreenHeight - 100 // 60 px from bottom
        );

        for (int i = 0; i < maxScore; i++)
        {
            Rectangle box = new Rectangle(
                (int)(position.X + i * (boxSize + spacing)),
                (int)position.Y,
                boxSize,
                boxSize
            );

            Color color = i < currentScore ? Color.SkyBlue : Color.White;
            spriteBatch.Draw(TextureGenerator.Pixel, box, color);
        }
    }



    private void DrawGameOverOverlay(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin();
        spriteBatch.Draw(_buttonTexture, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.Black * 0.5f);

        Texture2D texture = _gameStateWin ? _winTexture : _loseTexture;
        Vector2 position = new Vector2(
            (Game1.ScreenWidth - texture.Width) / 2,
            (Game1.ScreenHeight - texture.Height) / 2
        );

        spriteBatch.Draw(texture, position, Color.White);
    }

    public void Reset()
    {
        _gameScore = 0;
        _gameStateWin = false;
        _gameStateLost = false;
        _trailTimer = 0f;
        _trails.Clear();

        _player = new Player(_physicsEngine);
        _peacefulEnemies.Clear();
        for (int i = 0; i < _peacefulEnemyCount; i++)
        {
            _peacefulEnemies.Add(new PeacefulEnemy(new Base(1), new Eyes(1), new Mouth(1)));
        }

        _physicsEngine = new PhysicsEngine(_level);
        _physicsEngine.AddFood(new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f));
    }
    public void SaveGameState()
    {
        var data = SaveManager.Load() ?? new GameData();

        data.LastCompletedLevel = 2; // This scene = Level 1
        data.Traits = _player.ActiveTraits; // Replace with however you store selected traits

        SaveManager.Save(data);
    }

    // public void LoadFromSave()
    // {
    //     var data = SaveManager.Load();
    //     if (data == null)
    //     {
    //         Initialize(); // fallback
    //         return;
    //     }

    //     // Setup level and systems without overwriting player/enemy states
    //     _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, new GameplayRules(3600, 3), 20);
    //     SetupSystems();

    //     _gameScore = data.Score;
    //     _player._health = data.PlayerHealth;
    //     // _player._position = data.PlayerPosition;
    //     _player.SetPosition(data.PlayerPosition);

    //     _physicsEngine.ClearFood();
    //     Random rand = new Random();
    //     foreach (var pos in data.FoodPositions)
    //     {
    //         float angle = (float)(rand.NextDouble() * Math.PI * 2);
    //         Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    //         float speed = rand.Next(20, 50);

    //         _physicsEngine.AddFood(new Food(pos, dir, speed));
    //     }

    //     _peacefulEnemies.Clear();
    //     for (int i = 0; i < data.EnemyPositions.Count; i++)
    //     {
    //         var enemy = new PeacefulEnemy(new Base(2), new Eyes(2), new Mouth(2));
    //         enemy.SetPosition(data.EnemyPositions[i]);
    //         enemy.SetHealth(data.EnemyHealths[i]);
    //         _peacefulEnemies.Add(enemy);
    //     }

    //     RegisterEnemyColliders();
    // }

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


    private void RegisterEnemyColliders()
    {
        foreach (var enemy in _peacefulEnemies)
        {
            _collisionManager.AddCollider(enemy.GetBaseCollider(), other =>
            {
                if (other.Owner == _player && other == _player.GetMouthCollider())
                {
                    if (enemy._damageCooldown <= 0)
                    {
                        Console.WriteLine("Player's mouth collided with enemy's base");
                        enemy.TakeDamage(20);
                        enemy.ApplyBounce(_player.GetMouthCollider().Position - enemy.GetBaseCollider().Position, 0.5f);
                        enemy._damageCooldown = 0.5;
                    }
                }
            });

            _collisionManager.AddCollider(_player.GetMouthCollider(), other =>
            {
                Console.WriteLine("Mouth collider touched: " + other.Owner?.GetType().Name);
            });
        }
        foreach (var enemy in _aggressiveEnemies)
        {
            // Player mouth hits enemy base (deal damage to enemy)
            _collisionManager.AddCollider(enemy.GetBaseCollider(), other =>
            {
                if (other.Owner is Player)
                {
                    if (enemy._damageCooldown <= 0)
                    {
                        Console.WriteLine("Player's mouth hit aggressive enemy!");
                        enemy.TakeDamage(20);
                        enemy.ApplyBounce(_player.GetMouthCollider().Position - enemy.GetBaseCollider().Position, 0.5f);
                        enemy._damageCooldown = 1.0; // 1 second cooldown
                    }
                }
            });

            // Enemy mouth hits player base (enemy attacks player)
            _collisionManager.AddCollider(enemy.GetMouthCollider(), collider =>
            {
                if (collider.Owner is Player)
                {
                    if (enemy._damageCooldown <= 0)
                    {
                        Console.WriteLine("Aggressive enemy bit the player!");
                        _player.TakeDamage(10);
                        Console.WriteLine("Player health: " + _player._health);
                        _player.ApplyBounce(_player._position - enemy._position, 7f, 0.3f);
                        enemy._damageCooldown = 1.0; // Prevent rapid hits
                    }
                }
            });
        }

    }
    private void DrawPauseMenu(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin();

        // Grey overlay
        spriteBatch.Draw(_buttonTexture, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.SkyBlue);

        int boxWidth = 300, boxHeight = 300;
        int boxX = (Game1.ScreenWidth - boxWidth) / 2;
        int boxY = (Game1.ScreenHeight - boxHeight) / 2;

        Rectangle box = new Rectangle(boxX, boxY, boxWidth, boxHeight);
        spriteBatch.Draw(_buttonTexture, box, Color.White);

        int buttonHeight = 50;
        int padding = 10;

        _resumeButton = new Rectangle(box.X + 25, box.Y + 25, box.Width - 50, buttonHeight);
        _settingsButton = new Rectangle(box.X + 25, _resumeButton.Bottom + padding, box.Width - 50, buttonHeight);
        _mainMenuButton = new Rectangle(box.X + 25, _settingsButton.Bottom + padding, box.Width - 50, buttonHeight);
        _exitButton = new Rectangle(box.X + 25, _mainMenuButton.Bottom + padding, box.Width - 50, buttonHeight);

        DrawButton(spriteBatch, "Resume", _resumeButton);
        DrawButton(spriteBatch, "Settings", _settingsButton);
        DrawButton(spriteBatch, "Main Menu", _mainMenuButton);
        DrawButton(spriteBatch, "Exit", _exitButton);

        
    }
    private void HandlePauseMenuInput()
    {
        MouseState mouseState = Mouse.GetState();

        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if (_resumeButton.Contains(mouseState.Position))
                _isPaused = false;
            else if (_settingsButton.Contains(mouseState.Position))
            {
                var settings = (SettingsScene)SceneManager.Instance.GetScene(SceneManager.SceneType.SettingsScene);
                settings.SetCaller(SceneManager.SceneType.Level2);
                SceneManager.Instance.SetScene(SceneManager.SceneType.SettingsScene);

                _isPaused = false;
            }
            else if (_mainMenuButton.Contains(mouseState.Position))
            {
                SaveGameState();
                SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
            }
            else if (_exitButton.Contains(mouseState.Position))
            {
                _game.Exit(); // close the game
            }
        }

        _previousMouseState = mouseState;
    }


}
