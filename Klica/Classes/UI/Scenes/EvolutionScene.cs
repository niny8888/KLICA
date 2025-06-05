using System;
using Klica;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

public class EvolutionScene : IScene
{
    private Game1 _game;
    private BitmapFont _font;
    private Texture2D _background;
    private Texture2D _buttonTexture;
    private MouseState _previousMouseState;

    private List<(EvolutionTrait Trait, string Description)> _availableTraits;
    private Rectangle[] _traitButtons;
    private Rectangle _titleRect;

    private int _currentLevel;

    public EvolutionScene(Game1 game)
    {
        _game = game;
        _availableTraits = new();
    }

    public void Initialize()
    {
        _currentLevel = _game.CurrentLevel;
        LoadTraitsForLevel(_currentLevel);
    }

    public void LoadContent(ContentManager content)
    {
        _font = content.Load<BitmapFont>("Arial");
        _background = content.Load<Texture2D>("menu_BG");
        _buttonTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });

        _titleRect = new Rectangle(Game1.ScreenWidth / 2 - 200, 100, 400, 60);
    }

    public void Update(GameTime gameTime)
    {
        _currentLevel = _game.CurrentLevel;
        LoadTraitsForLevel(_currentLevel);
        MouseState mouse = Mouse.GetState();

        for (int i = 0; i < _availableTraits.Count; i++)
        {
            if (_traitButtons[i].Contains(mouse.Position) &&
                mouse.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                ApplyTrait(_availableTraits[i].Trait);

                switch (_currentLevel)
                {
                    case 1:
                        var level2 = (Level2_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level2);
                        level2.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level2);
                        break;
                    case 2:
                        var level3 = (Level3_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level3);
                        level3.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level3);
                        break;
                    case 3:
                        var level4 = (Level4_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level4);
                        level4.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level4);
                        break;
                    case 4:
                        var level5 = (Level5_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level5);
                        level5.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level5);
                        break;
                    case 5:
                        var level6 = (Level6_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level6);
                        level6.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level6);
                        break;
                    case 6:
                        var level7 = (Level7_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level7);
                        level7.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level7);
                        break;
                    case 7:
                        var level8 = (Level8_Scene)SceneManager.Instance.GetScene(SceneManager.SceneType.Level8);
                        level8.Initialize();
                        SceneManager.Instance.SetScene(SceneManager.SceneType.Level8);
                        break;
                }
                
                _game.CurrentLevel++;
                return;
            }
        }

        _previousMouseState = mouse;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try { spriteBatch.End(); } catch { }

        spriteBatch.Begin();
        spriteBatch.Draw(_background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);
        spriteBatch.End();

        spriteBatch.Begin();
        spriteBatch.DrawString(_font, "Choose Your Evolution", new Vector2(_titleRect.X, _titleRect.Y), Color.White);
        for (int i = 0; i < _availableTraits.Count; i++)
        {
            var trait = _availableTraits[i];
            spriteBatch.Draw(_buttonTexture, _traitButtons[i], Color.CadetBlue);
            spriteBatch.DrawString(_font, trait.Description, new Vector2(_traitButtons[i].X + 10, _traitButtons[i].Y + 20), Color.White);
        }

        
    }

    private void LoadTraitsForLevel(int level)
    {
        _availableTraits.Clear();

        switch (level)
        {
            case 1:
                _availableTraits.Add((EvolutionTrait.Dash, "Dash")); // Quick burst forward
                break;
            case 2:
                _availableTraits.Add((EvolutionTrait.ExtraDash, "+1 Dash"));
                _availableTraits.Add((EvolutionTrait.BonusHealth, "+50 Health"));
                break;
            case 3:
                _availableTraits.Add((EvolutionTrait.Regeneration, "Regeneration"));
                _availableTraits.Add((EvolutionTrait.StunDash, "Stun Dash"));
                break;
            case 4:
                _availableTraits.Add((EvolutionTrait.SlowTouch, "Skin Adaptation - Slows enemies on contact"));
                _availableTraits.Add((EvolutionTrait.FeederMode, "Feeder Mode - More food drops from kills"));
                break;
            case 5:
                _availableTraits.Add((EvolutionTrait.FrenzyMode, "Frenzy - Speed & damage boost on kill"));
                _availableTraits.Add((EvolutionTrait.ShellArmor, "Shell Armor - Reduce all damage"));
                break;
            case 6:
                _availableTraits.Add((EvolutionTrait.TraitMemory, "Trait Memory - Random traits you skipped"));
                break;
            case 7: //temp
                _availableTraits.Add((EvolutionTrait.SlowTouch, "Skin Adaptation - Slows enemies on contact"));
                _availableTraits.Add((EvolutionTrait.FeederMode, "Feeder Mode - More food drops from kills"));
                break;
        }

        _traitButtons = new Rectangle[_availableTraits.Count];

        for (int i = 0; i < _availableTraits.Count; i++)
        {
            int spacing = Game1.ScreenWidth / (_availableTraits.Count + 1);
            int x = spacing * (i + 1) - 100; // center 200px-wide buttons
            int y = Game1.ScreenHeight / 2;

            _traitButtons[i] = new Rectangle(x, y, 200, 60);
        }


    }

    private void ApplyTrait(EvolutionTrait trait)
    {   
        _game.CurrentPlayer?.AddTrait(trait);
    }
} 

public enum EvolutionTrait
{
    None,
    Dash,
    ExtraDash,
    BonusHealth,
    Regeneration,
    StunDash,
    FrenzyMode,
    ShellArmor,
    FeederMode,
    TraitMemory,
    SlowTouch
} 