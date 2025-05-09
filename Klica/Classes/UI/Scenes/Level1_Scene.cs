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

public class Level1_Scene : IScene
{
    private Game1 _game;
    private Level _level;
    private Player _player;
    private HUD _hud;
    private PhysicsEngine _physicsEngine;
    private CollisionManager _collisionManager;

    private List<PeacefulEnemy> _peacefulEnemies { get; }= new();
    private Texture2D _background, _halfCircleTexture, _buttonTexture, _winTexture, _loseTexture;
    private BitmapFont _font;
    private Rectangle _backButton;

    private int _foodGoal = 10;
    private int _peacefulEnemyCount = 3;
    private float _trailTimer = 0f;
    private List<HalfCircleTrail> _trails = new();
    private bool _gameStateWin = false;
    private bool _gameStateLost = false;
    private int _gameScore = 0;

    private Texture2D _circleTexture;
    private MouseState _previousMouseState;


    private double _autosaveTimer = 0;

    public Level1_Scene(Game1 game)
    {
        _game = game;
    }

    public void Initialize()
    {
        // Create _level first!
        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, new GameplayRules(3600, 3), 20);

        _collisionManager = new CollisionManager();
        _physicsEngine = new PhysicsEngine(_level);
        _player = new Player(_physicsEngine);
        _peacefulEnemies.Clear();

        // Spawn enemies first
        for (int i = 0; i < _peacefulEnemyCount; i++)
        {
            var peaceful = new PeacefulEnemy(new Base(2), new Eyes(2), new Mouth(2));
            _peacefulEnemies.Add(peaceful);
        }

        // Then register collisions
        foreach (var enemy in _peacefulEnemies)
        {
            var collider = enemy.GetBaseCollider();
            // _collisionManager.AddCollider(_player.GetMouthCollider(), other => {
            // foreach (var enemy in _peacefulEnemies)
            //     {
            //         if (other == enemy.GetBaseCollider())
            //         {
            //             Console.WriteLine("Player's mouth collided with enemy's base");
            //             enemy.TakeDamage(50);
            //             enemy.ApplyBounce(_player.GetMouthCollider().Position - enemy.GetBaseCollider().Position, 0.5f);
            //         }
            //     }
            // });
            _collisionManager.AddCollider(enemy.GetBaseCollider(), other =>
            {
                if (other.Owner == _player && other == _player.GetMouthCollider())
                    {
                        if (enemy._damageCooldown <= 0)
                        {
                            Console.WriteLine("Player's mouth collided with enemy's base");
                            enemy.TakeDamage(20);
                            enemy.ApplyBounce(_player.GetMouthCollider().Position - enemy.GetBaseCollider().Position, 0.5f);
                            enemy._damageCooldown = 0.5; // 0.5 seconds between hits
                        }
                    }

            });

            _collisionManager.AddCollider(_player.GetMouthCollider(), other =>
            {
                Console.WriteLine("Mouth collider touched: " + other.Owner?.GetType().Name);
            });


            
        }



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
        _autosaveTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_autosaveTimer >= 5)
        {
            SaveGameState();
            _autosaveTimer = 0;
        }
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
        _collisionManager.Update();

        _physicsEngine.Update(gameTime, _player._player_mouth._position, ref _gameScore, _player, null);


        _gameStateWin = _gameScore >= _foodGoal;
        _gameStateLost = _player._health <= 0;

        HandleInput();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try { spriteBatch.End(); } catch { }

        spriteBatch.Begin();
        _level.DrawBackground(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin();
        _physicsEngine.Draw(spriteBatch);

        foreach (var trail in _trails)
            trail.Draw(spriteBatch, _halfCircleTexture);

        foreach (var enemy in _peacefulEnemies){
            enemy.Draw(spriteBatch, _game.GetGameTime());
            enemy.Draw(spriteBatch, _game.GetGameTime());
            enemy.DrawHealthBar(spriteBatch);
        }
            

        _player.DrawHealthBar(spriteBatch);
        _player.DrawPlayer(spriteBatch, _game.GetGameTime());

        DrawButton(spriteBatch, "Back to Menu", _backButton);
        _hud.Draw(spriteBatch, _player, new List<Enemy>());
        _hud.DisplayScore(spriteBatch, _gameScore);

        if (_gameStateWin || _gameStateLost)
            DrawGameOverOverlay(spriteBatch);

        
    }

    private void HandleInput()
    {
        MouseState mouseState = Mouse.GetState();

        if (mouseState.LeftButton == ButtonState.Pressed &&
            _previousMouseState.LeftButton == ButtonState.Released &&
            _backButton.Contains(mouseState.Position))
        {
            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            SceneManager.Instance.SetScene(SceneManager.SceneType.MainMenu);
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
        var data = new GameData
        {
            Score = _gameScore,
            PlayerHealth = _player._health,
            PlayerPosition = _player._position,
            FoodPositions = _physicsEngine.GetAllFoodPositions(),
            EnemyPositions = _peacefulEnemies.Select(e => e.Position).ToList(),
            EnemyHealths = _peacefulEnemies.Select(e => e.Health).ToList()
        };
        SaveManager.Save(data);
    }
    public void LoadFromSave()
    {
        var data = SaveManager.Load();
        if (data == null)
        {
            Initialize(); // fallback
            return;
        }

        Initialize(); // optional: clear before loading state
        _gameScore = data.Score;
        _player._health = data.PlayerHealth;
        _player._position = data.PlayerPosition;
        
        _physicsEngine.ClearFood();
        foreach (var pos in data.FoodPositions)
            _physicsEngine.AddFood(new Food(pos, Vector2.One, 1f));

        _peacefulEnemies.Clear();
        for (int i = 0; i < data.EnemyPositions.Count; i++)
        {
            var enemy = new PeacefulEnemy(new Base(2), new Eyes(2), new Mouth(2));
            enemy.SetPosition(data.EnemyPositions[i]);
            enemy.SetHealth(data.EnemyHealths[i]);
            _peacefulEnemies.Add(enemy);
        }
    }

}
