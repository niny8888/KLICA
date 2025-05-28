using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class SceneManager
{
    public enum SceneType { MainMenu, Level1Intro, Level1, Level2, Level3, Level4, Level5, Level6, Game , SettingsScene, EvolutionScene}

    private readonly Dictionary<SceneType, IScene> _scenes = new();
    private IScene _currentScene;

    public static SceneManager Instance { get; } = new();

    private SceneManager() { }

    public void AddScene(SceneType sceneType, IScene scene)
    {
        if (!_scenes.ContainsKey(sceneType))
        {
            _scenes.Add(sceneType, scene);
            Console.WriteLine($"Scene added: {sceneType}");
        }
    }
    

    public void SetScene(SceneType sceneType)
    {
        if (_scenes.TryGetValue(sceneType, out var scene))
        {
            _currentScene = scene;
            //_currentScene.Initialize();
        }
    }
    public IScene GetScene(SceneType sceneType)
    {
        if (_scenes.TryGetValue(sceneType, out var scene))
        {
            return scene;
        }
        throw new InvalidOperationException($"Scene of type {sceneType} not found.");
    }

    public void LoadContent(ContentManager content)
    {
        foreach (var scene in _scenes.Values)
        {
            scene.LoadContent(content);
        }
    }

    public void Update(GameTime gameTime)
    {
        _currentScene?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Console.WriteLine("Drawing scene: " + _currentScene);

        _currentScene?.Draw(spriteBatch);
    }
}
