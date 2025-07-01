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
 
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Audio;

public class GameScene : IScene
{ //was OG scene - unused in the final game!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private Level _level;
    private GameplayRules _gameplayRules;
    private PhysicsEngine _physicsEngine;
    private Player _player;
    private CollisionManager _collisionManager;
    
/// NPC STVARI
    private List<Enemy> _enemies;
    private List<PeacefulEnemy> _enemies_peaceful;

     private int _enemyCount = 0;
    private int _enemySpawnRate = 4;
     private int _enemypeacefulCount = 0;
    private int _enemypeacefulSpawnRate = 1;

/// VISUAL
    private Texture2D _background;
    private SpriteManager _spriteManager;

    

    // za gumb
    private Rectangle _backButton;
    private Texture2D _buttonTexture;
    private SpriteFont _font;
    private MouseState _previousMouseState;


    //game state
    Game1 _game;
    public static int _gameScore;
    public bool _gameStateWin;
    public bool _gameStateLost;
    private Texture2D _winTexture;
    private Texture2D _loseTexture;
    private Random _random;
    private GameData _gameData;
    private string _saveFilePath;

    //Shader
    private Effect _waterFlowEffect;
    private Effect _perlinNoiseEffect;
    private HUD _hud;
    private Vector2 _shaderTime = Vector2.Zero;
    private Vector2 _shaderPerlinTime = Vector2.Zero;
    

    //trail
    private List<HalfCircleTrail> _trails = new List<HalfCircleTrail>();
    private Dictionary<Enemy, List<HalfCircleTrail>> _enemyTrails = new();
    private Dictionary<Enemy, float> _enemyTrailTimers = new();
    private Dictionary<PeacefulEnemy, List<HalfCircleTrail>> _enemypeacefulTrails = new();
    private Dictionary<PeacefulEnemy, float> _enemypeacefulTrailTimers = new();
    private Texture2D _halfCircleTexture;
    private float _trailTimer = 0f;

    //debug stuuf
    private Texture2D _debugTexture;
    private Texture2D _circleTexture;

    //soundeffecti
    SoundEffectInstance se_dmg;

    public GameScene(Game1 game)
    {
        _game = game; 
        _random = new Random();
        _enemies = new List<Enemy>();
        _enemies_peaceful = new List<PeacefulEnemy>();
        _collisionManager = new CollisionManager();
        _saveFilePath = GetSaveFilePath();
        // _gameData = LoadGameData(); // Load game data
        // _gameScore = _gameData.LastScore; 
    }

    public void Initialize()
    {
        _gameplayRules = new GameplayRules(3600, 3);
        
    }

// ==============================================
// ============== LOAD =================
// ==============================================


    public void LoadContent(ContentManager content)
    {   
        
        _backButton = new Rectangle(20, 20, 200, 50);
        System.Console.WriteLine("Loading game scene content");
        _background = content.Load<Texture2D>("menu_BG"); ///bg_0000_bg3
        var spriteSheet = content.Load<Texture2D>("SpritesPNG1_fixed"); //"SpriteInfo"
        _spriteManager = new SpriteManager(spriteSheet);
        _halfCircleTexture = TextureGenerator.CreateCircleRadiusLineTexture(_game.GraphicsDevice, 50); // Radius 50
        _spriteManager.SetDefaultSheet(content.Load<Texture2D>("SpritesPNG1_fixed"));
        //_spriteManager.SetIceSheet(content.Load<Texture2D>("SpritesPNG-ICE"));
        //_spriteManager.SetToxicSheet(content.Load<Texture2D>("SpritesPNG-Toxic"));
        _spriteManager.UseDefaultSheet();
        var spriteDataLines = System.IO.File.ReadAllLines("Assets/SpriteInfo.txt");
        SpriteFactory.Initialize(_spriteManager, spriteDataLines);
        se_dmg= content.Load<SoundEffect>("SE_eat_food").CreateInstance();

        _waterFlowEffect = content.Load<Effect>("WaterFlow");
        _waterFlowEffect.Parameters["DistortionStrength"].SetValue(0.005f);
        _waterFlowEffect.Parameters["Frequency"].SetValue(0.005f);

        _perlinNoiseEffect = content.Load<Effect>("PerlinNoise");
        _perlinNoiseEffect.Parameters["seed"].SetValue(714.434f);
        _perlinNoiseEffect.Parameters["lineValueLimit"].SetValue(0.005f); // Adjust for less intense lines
        _perlinNoiseEffect.Parameters["lineColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f)); // White lines
        _perlinNoiseEffect.Parameters["lineAlpha"].SetValue(0.5f); // Adjust for less intense lines

        _debugTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });
        _circleTexture = Collider.CreateCircleTexture(_game.GraphicsDevice, 50, Color.White);

        _level = new Level(new Rectangle(0, 0, 1920, 1080), _background, _gameplayRules, 20);
        _physicsEngine = new PhysicsEngine(_level);

        var food = new Food(new Vector2(500, 500), new Vector2(1, 0.5f), 1f);
        _physicsEngine.AddFood(food);

        _player = new Player(_physicsEngine);
        // _collisionManager.AddCollider(_player.GetBaseCollider(), HandlePlayerBaseCollision);
        // _collisionManager.AddCollider(_player.GetMouthCollider(), HandlePlayerMouthCollision);

        if (_enemyCount < _enemySpawnRate){
            _enemyCount++;
            SpawnEnemies(1);
        }
        // if (_enemypeacefulCount < _enemypeacefulSpawnRate){
        //     _enemypeacefulCount++;
        //     SpawnEnemies(1);
        // }

        // SpawnEnemies(_enemySpawnRate);
       
        AddColliders();

        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _font = _game.Content.Load<SpriteFont>("Arial");
        _hud = new HUD(_font);
       
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new Color[] { Color.White });
        _winTexture = content.Load<Texture2D>("win");
        _loseTexture = content.Load<Texture2D>("lose");

    }

    


// ==============================================
// ============== UPDATE =================
// ==============================================

    public void Update(GameTime gameTime)
    {
        if (_gameStateWin || _gameStateLost)
        {
            return;
        }
        System.Console.WriteLine("Updating game scene");

        _player.UpdatePlayer(gameTime, _level.Bounds);


        // Add trail behind the player every 3 seconds
         _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_trailTimer >= 0.5f)
        {
        _trailTimer = 0f;
        _trails.Add(new HalfCircleTrail(
            _player._position,
            10f,
            50f,
            2f,
            _player.GetRotation()
        ));
        }
        foreach (var trail in _trails)
        {
            trail.Update(gameTime);
        }
        _trails.RemoveAll(trail => trail.IsExpired);


        // Update trails
        foreach (var enemy in _enemies)
        {
            // Initialize trail timers for the enemy
            if (!_enemyTrailTimers.ContainsKey(enemy))
            {
                _enemyTrailTimers[enemy] = 0f;
            }

            _enemyTrailTimers[enemy] += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_enemyTrailTimers[enemy] >= 0.5f)
            {
                _enemyTrailTimers[enemy] = 0f;
                if (!_enemyTrails.ContainsKey(enemy))
                {
                    _enemyTrails[enemy] = new List<HalfCircleTrail>();
                }

                _enemyTrails[enemy].Add(new HalfCircleTrail(
                    enemy._position,
                    10f,
                    50f,
                    2f,
                    enemy.GetRotation()
                ));
            }

            // Update trails
            if (_enemyTrails.ContainsKey(enemy))
            {
                foreach (var trail in _enemyTrails[enemy])
                {
                    trail.Update(gameTime);
                }

                _enemyTrails[enemy].RemoveAll(trail => trail.IsExpired);
            }

            // Collision handling between player and enemy (base collision)
            if (Vector2.Distance(_player._position, enemy._position) <
                _player.GetBaseCollider().Radius + enemy.GetBaseCollider().Radius)
            {
                _collisionManager.HandleBaseBaseCollision(_player,enemy);
            }

            // Collision handling for player mouth with enemy
            if (Vector2.Distance(_player.GetMouthCollider().Position, enemy._position) <
                _player.GetMouthCollider().Radius + enemy.GetBaseCollider().Radius)
            {
               _collisionManager.HandlePlayerMouthWithEnemyBase(_player,enemy);
            }

            // Collision handling for enemy mouth with player
            if (Vector2.Distance(enemy.GetMouthCollider().Position, _player._position) <
                enemy.GetMouthCollider().Radius + _player.GetBaseCollider().Radius)
            {
                _collisionManager.HandleEnemyMouthWithPlayerBase(_player,enemy);
            }

            // Update enemy behavior
            enemy.Update(gameTime, _physicsEngine, _player);

            ConstrainToBounds(enemy);
        }



         // Handle input for the back button
        _collisionManager.Update();
        _physicsEngine.Update(gameTime, _player._player_mouth._position, ref _gameScore, _player,_enemies);
        _gameStateWin = _gameplayRules.CheckWinCondition(_gameScore);
       // _gameStateLost = _gameplayRules.CheckLoseCondition(_gameScore);
        _gameStateLost = _gameplayRules.CheckLoseConditionPlayer(_player._health);

        // if (_gameStateWin)
        // {
        //     Console.WriteLine("You won!");
        //     SaveGameData(); 
        // }

        // if (_gameStateLost)
        // {
        //     Console.WriteLine("You lost!");
        //     SaveGameData(); 
        // }
        
        
        _shaderTime.X += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _shaderTime.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

        _shaderPerlinTime.X += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _perlinNoiseEffect.Parameters["iTime"].SetValue(_shaderPerlinTime.X);

        _waterFlowEffect.Parameters["Time"].SetValue(_shaderTime);
        HandleInput();
    }
    

    
// ==============================================
// ============== DRAW  =================
// ==============================================

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
        
        //to nrdi ful prevc dobre enemyje:
        //_enemies.ForEach(enemy => enemy.Update(_game.GetGameTime(), _player, _physicsEngine));
        
         foreach (var trail in _trails)
        {
            trail.Draw(spriteBatch, _halfCircleTexture);
        }

        // Draw enemy trails
        foreach (var enemyTrailPair in _enemyTrails)
        {
            foreach (var trail in enemyTrailPair.Value)
            {
                trail.Draw(spriteBatch, _halfCircleTexture);
            }
        }

        // Draw other game elements
        _physicsEngine.Draw(spriteBatch);
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch, _game.GetGameTime());
        }
        _player.DrawPlayer(spriteBatch, _game.GetGameTime());



//////////////Collision DEBUG
        // Draw player colliders
        // Collider.DrawCollider(spriteBatch, _circleTexture, _player.GetBaseCollider(), Color.Green);
        // Collider.DrawCollider(spriteBatch, _circleTexture, _player.GetMouthCollider(), Color.Blue);


        // //Draw enemy colliders
        // foreach (var enemy in _enemies)
        // {
        //     Collider.DrawCollider(spriteBatch, _circleTexture, enemy.GetBaseCollider(), Color.Red);
        //     Collider.DrawCollider(spriteBatch, _circleTexture, enemy.GetMouthCollider(), Color.Yellow);
        // }

        

        spriteBatch.End();

        spriteBatch.Begin(effect: _perlinNoiseEffect, blendState: BlendState.AlphaBlend);
        spriteBatch.Draw(
            _background, 
            new Rectangle(0, 0, 1920, 1080),
            Color.White
        );
        
        spriteBatch.End();

        spriteBatch.Begin();
        DrawButton(spriteBatch, "Back to Menu", _backButton);
        _hud.Draw(spriteBatch, _player, _enemies);
        _hud.DisplayScore(spriteBatch, _gameScore);
        
        if (_gameStateWin || _gameStateLost)
        {
            DrawGameOverOverlay(spriteBatch);
        }

    }

// ==============================================
// ============== INPUT =================
// ==============================================

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

// ==============================================
// ============== NPC STUFF =================
// ==============================================
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
                    HandleBounce(_player, enemy);
                    enemy.TakeDamage(10);
                }
            });

            _collisionManager.AddCollider(enemy.GetMouthCollider(), collider =>
            {
                if (collider == _player.GetBaseCollider())
                {
                    HandleBounce(enemy, _player);
                    _player.TakeDamage(10);
                    if(_player._health <= 0)
                    {
                        _gameStateLost = true;
                    }
                }
            });

            _enemies.Add(enemy);
        }
    }
    // private void HandlePlayerBaseCollision(Collider other)
    // {
    //     if (other.Owner is Food food && !food.IsConsumed)
    //     {
    //         food.OnConsumed(ref _gameScore);
    //     }
    // }


    // private void HandlePlayerMouthCollision(Collider other)
    // {
    //     if (other.Owner is Enemy enemy)
    //     {
    //         Console.WriteLine("Player's mouth hits enemy base.");
    //     }
    // }

// ==============================================
// ============== LEVEL STUFF =================
// ==============================================

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

    
    


// ==============================================
// ============== GAME DATA =================
// ==============================================

    // private void SaveGameData()
    // ///problem da shranjuje sam ce zmagas alpa zgubis!!!!!!!
    // {
    //     _gameData.LastScore = _gameScore; 
    //     _gameData.SoundOn = true; 

    //     string jsonData = JsonSerializer.Serialize(_gameData);
    //     Directory.CreateDirectory(Path.GetDirectoryName(_saveFilePath));
    //     File.WriteAllText(_saveFilePath, jsonData);
    // }

    private string GetSaveFilePath()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(folder, "Klica", "SaveData.json");
        }

    // public GameData LoadGameData()
    // {
    //     string filePath = GetSaveFilePath();

    //     if (File.Exists(filePath))
    //     {
    //         string jsonData = File.ReadAllText(filePath);
    //         return JsonSerializer.Deserialize<GameData>(jsonData);
    //     }

    //     // Return default values if no save file exists
    //     return new GameData
    //     {
    //         LastScore = 0,
    //         SoundOn = true,
    //     };
    // }

// ==============================================
// ============== NEW GAME =================
// ==============================================

    // public void NewGame()
    // {
    //     _gameData.LastScore = 0; 
    
    //     _gameScore = 0;
    //     _gameStateWin = false;
    //     _gameStateLost = false;
        
    //     SaveGameData(); 
    //     // Reset player and enemies
    //     _player = new Player(_physicsEngine); 
    //     _enemies.Clear(); 
    //     SpawnEnemies(_enemySpawnRate); 

    //     _physicsEngine = new PhysicsEngine(_level); 
    //     Console.WriteLine("New game started. Data reset to default.");
    // }

// ==============================================
// ============== VISUAL =================
// ==============================================

    private void DrawGameOverOverlay(SpriteBatch spriteBatch)
    {
        // Draw a semi-transparent black overlay
        spriteBatch.End();
        spriteBatch.Begin();
        spriteBatch.Draw(
            _buttonTexture,
            new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight),
            Color.Black * 0.5f // Semi-transparent black
        );

        // Choose the appropriate image
        Texture2D gameOverTexture = _gameStateWin ? _winTexture : _loseTexture;

        // Center the image
        Vector2 position = new Vector2(
            (Game1.ScreenWidth - gameOverTexture.Width) / 2,
            (Game1.ScreenHeight - gameOverTexture.Height) / 2
        );

        spriteBatch.Draw(gameOverTexture, position, Color.White);
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


// ==============================================
// ============== FIZKA - simulacije =================
// ==============================================


    private void HandleBounce(dynamic entityA, dynamic entityB)
    {
        // Calculate collision normal
        Vector2 normal = Vector2.Normalize(entityB.GetBaseCollider().Position - entityA.GetMouthCollider().Position);

        // Relative velocity
        Vector2 relativeVelocity = entityA.Velocity - entityB.Velocity;

        // Velocity along the normal
        float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

        // If objects are separating, skip collision
        if (velocityAlongNormal > 0) return;

        // Combined restitution coefficient
        float combinedRestitution = entityA.Restitution * entityB.Restitution;

        // Inverse masses
        float invMassA = entityA.Mass > 0 ? 1 / entityA.Mass : 0;
        float invMassB = entityB.Mass > 0 ? 1 / entityB.Mass : 0;

        // Impulse scalar
        float impulse = -(1 + combinedRestitution) * velocityAlongNormal;
        impulse /= invMassA + invMassB;

        // Apply impulse
        Vector2 impulseVector = impulse * normal;
        entityA.Velocity += impulseVector * invMassA;
        entityB.Velocity -= impulseVector * invMassB;

        // Debug output
        Console.WriteLine($"Bounce! EntityA Velocity: {entityA.Velocity}, EntityB Velocity: {entityB.Velocity}");
    }

    public void AddColliders(){
         _collisionManager.AddCollider(_player.GetMouthCollider(), collider =>
        {
            if (collider.Owner is Enemy enemy)
            {
                se_dmg.Play();
                Console.WriteLine("Player mouth collided with enemy base!");
                HandleBounce(_player, enemy);
            }
        });

        _collisionManager.AddCollider(_player.GetBaseCollider(), collider =>
        {
            if (collider.Owner is Enemy enemy)
            {
                se_dmg.Play();
                Console.WriteLine("Player base collided with enemy mouth!");
                HandleBounce(enemy, _player);
            }
        });

        foreach (var enemy in _enemies)
        {
            _collisionManager.AddCollider(enemy.GetMouthCollider(), collider =>
            {
                if (collider.Owner is Player)
                {
                    se_dmg.Play();
                    Console.WriteLine("Enemy mouth collided with player base!");
                    HandleBounce(enemy, _player);
                }
            });

            _collisionManager.AddCollider(enemy.GetBaseCollider(), collider =>
            {
                if (collider.Owner is Player)
                {
                    se_dmg.Play();
                    Console.WriteLine("Enemy base collided with player mouth!");
                    HandleBounce(_player, enemy);
                }
            });
        }
    }




}
