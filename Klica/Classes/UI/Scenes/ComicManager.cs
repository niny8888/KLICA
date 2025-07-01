using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public class ComicManager
    {//unused
        private Dictionary<string, ComicScene> comics;
        private ComicScene currentComic;

        public ComicManager()
        {
            comics = new Dictionary<string, ComicScene>();
        }

        public void AddComic(string name, ComicScene comic)
        {
            comics[name] = comic;
        }

        public void StartComic(string name)
        {
            if (comics.ContainsKey(name))
            {
                currentComic = comics[name];
            }
        }

        public bool IsComicFinished()
        {
            return currentComic != null && currentComic.IsFinished;
        }

        public void Update(GameTime gameTime)
        {
            currentComic?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentComic?.Draw(spriteBatch);
        }
    }
}
