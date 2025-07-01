using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HUD
{
    private SpriteFont _font;

    public HUD(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, Player player, List<Enemy> enemies)
    {
        // // Draw player health above the player
        // Vector2 playerHealthPosition = player._position - new Vector2(0, 30); // Slightly above the player
        // spriteBatch.DrawString(_font, $"{player._health}/100", playerHealthPosition, Color.White);

        // // Draw enemy health above each enemy
        // foreach (var enemy in enemies)
        // {
        //     Vector2 enemyHealthPosition = enemy._position - new Vector2(0, 30); // Slightly above each enemy
        //     spriteBatch.DrawString(_font, $"{enemy._health}/100", enemyHealthPosition, Color.Red);
        // }
    }
    public void DisplayScore(SpriteBatch spriteBatch, int gameScore) //int highhscore
    {
        Vector2 scorePosition = new Vector2(800, 20); // Top-left corner of the screen
        //Vector2 highScorePosition = new Vector2(20, 50); // Below the current score

        // Draw the current score and high score
        spriteBatch.DrawString(_font, $"Score: {gameScore}", scorePosition, Color.White);
        //spriteBatch.DrawString(_font, $"High Score: {highScore}", highScorePosition, Color.Yellow);
    }
    
}
