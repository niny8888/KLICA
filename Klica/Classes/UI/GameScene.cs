using Klica;
using Klica.Classes;
using Klica.Classes.Objects_sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class GameScene : IScene
{
    private Level _level;
    private GameplayRules _gameplayRules;
    private PhysicsEngine _physicsEngine;
    private Player _player;
    private Texture2D _background;
    private SpriteManager _spriteManager;

    public void Initialize()
    {
        // Initialize gameplay systems
        _player = new Player();
        _gameplayRules = new GameplayRules(3600, 1);
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
    }

    public void Update(GameTime gameTime)
    {
        _player.UpdatePlayer();
        int score = 0;
        _physicsEngine.Update(gameTime, _player._position, ref score);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _level.DrawBackground(spriteBatch);
        _physicsEngine.Draw(spriteBatch);
        _player.DrawPlayer(spriteBatch);
    }
}
