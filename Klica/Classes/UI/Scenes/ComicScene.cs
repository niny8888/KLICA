using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica
{
    public class ComicScene : IScene
    {
        private List<Texture2D> panels;
        private int currentPanelIndex;
        private SpriteFont font;
        private Rectangle nextButtonRect;
        private MouseState previousMouseState;

        public bool IsFinished { get; private set; }

        public ComicScene(List<Texture2D> panelTextures, SpriteFont comicFont)
        {
            panels = panelTextures;
            font = comicFont;
            currentPanelIndex = 0;
            IsFinished = false;
            nextButtonRect = new Rectangle(600, 500, 150, 50); // Position of "Next" button
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            // Check for button click
            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                if (nextButtonRect.Contains(mouseState.Position))
                {
                    NextPanel();
                }
            }

            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (panels.Count == 0) return;

            // Draw current panel
            spriteBatch.Draw(panels[currentPanelIndex], new Rectangle(0, 0, 800, 600), Color.White);

            // Draw button if more panels exist
            if (currentPanelIndex < panels.Count - 1)
            {
                spriteBatch.Draw(panels[currentPanelIndex], nextButtonRect, Color.Gray);
                spriteBatch.DrawString(font, "Next", new Vector2(nextButtonRect.X + 50, nextButtonRect.Y + 15), Color.Black);
            }
        }

        private void NextPanel()
        {
            if (currentPanelIndex < panels.Count - 1)
            {
                currentPanelIndex++;
            }
            else
            {
                IsFinished = true; // Comic ends
            }
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void LoadContent(ContentManager content)
        {
            throw new System.NotImplementedException();
        }
    }
}
