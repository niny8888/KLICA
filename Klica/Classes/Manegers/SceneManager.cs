using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SceneManager
{
    private static SceneManager _instance;
    public static SceneManager Instance => _instance ??= new SceneManager();

    public enum SceneType { MainMenu, Game }
    private SceneType _currentScene;
    private Dictionary<SceneType, IScene> _scenes;

    private SceneManager()
    {
        _scenes = new Dictionary<SceneType, IScene>();
    }

    public void AddScene(SceneType sceneType, IScene scene)
    {
        _scenes[sceneType] = scene;
    }

    public void SetScene(SceneType sceneType)
    {
        _currentScene = sceneType;
        _scenes[_currentScene].Initialize();
    }

    public void Update(GameTime gameTime)
    {
        _scenes[_currentScene]?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _scenes[_currentScene]?.Draw(spriteBatch);
    }
    public IScene GetCurrentScene(){
        return _scenes[_currentScene];
    }
}
