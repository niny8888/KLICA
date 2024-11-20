using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Klica
{
    public static class SpriteFactory
    {
        private static Texture2D _spriteSheet;
        public static void Initialize(Texture2D spriteSheet, SpriteManager spriteManager, IEnumerable<string> spriteDataLines)
        {
            _spriteSheet = spriteSheet;

            // Parse and add each sprite from the provided data lines
            foreach (var line in spriteDataLines)
            {
                AddSpriteFromDataLine(spriteManager, line);
            }
        }

        
        // public static void Initialize(Texture2D spriteSheet, SpriteManager spriteManager)
        // {
        //     _spriteSheet = spriteSheet;

        //     // Definicije spritov

        //     //ADD-ON
        //     //spriteManager.AddSprite("spikes_middle", new Vector2(500, 100), new Rectangle(0, 0, 570, 70), scale: 1f);

        //     //BODY
        //     //spriteManager.AddSprite("body_orange", new Vector2(400, 500), new Rectangle(0, 75, 300, 437), scale: 1f);
        //     //spriteManager.AddSprite("body_blue", new Vector2(400, 500), new Rectangle(300, 75, 280, 437), scale: 1f);
        //     //spriteManager.AddSprite("body_green", new Vector2(400, 500), new Rectangle(0, 520, 260, 445), scale: 1f);
        //     spriteManager.AddSprite("body_pink", new Vector2(400, 500), new Rectangle(265, 520, 360, 400), scale: 1f);
        // }
        private static void AddSpriteFromDataLine(SpriteManager spriteManager, string line)
        {
            var parts = line.Split(';');

            // Parse data parts
            string name = parts[0];
            int rotacijaD = int.Parse(parts[1]); /// rotacija?


            int x=int.Parse(parts[2]);
            int y = int.Parse(parts[3]);
            int width = int.Parse(parts[4]);
            int height = int.Parse(parts[5]);
            float pivotX = float.Parse(parts[8])/10; // 0.5 - odvzone, 0,5 visie
            float pivotY = float.Parse(parts[9])/10;

            // Position and other properties
            Vector2 position = new Vector2(400, 500); // Modify or make dynamic as needed
            Rectangle sourceRectangle = new Rectangle(x, y, width, height);
            float rotateAngle;
            float scale = 1f;  // Default scale, modify as needed
            if(rotacijaD==1){
                rotateAngle= -1.6f;
            }
            else{
                rotateAngle=0f;
            }
            Vector2 pivot;

            pivot = new Vector2(
                (int)Math.Ceiling(pivotX * width),
                (int)Math.Ceiling(pivotY * height)
            );
            
            spriteManager.AddSprite(name, position, sourceRectangle, scale: scale ,rotateAngle, pivot);
        }
    }
}
