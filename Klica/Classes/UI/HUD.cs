using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class HUD
{
    private Texture2D healthBarTexture;
    private Texture2D hungerBarTexture;
    private Rectangle healthBarRectangle;
    private Rectangle hungerBarRectangle;
    private float decreaseRate = 0.1f;
    private float health = 100f;
    private float hunger = 100f;

    public HUD(Texture2D healthTexture, Texture2D hungerTexture, Rectangle healthRect, Rectangle hungerRect)
    {
        healthBarTexture = healthTexture;
        hungerBarTexture = hungerTexture;
        healthBarRectangle = healthRect;
        hungerBarRectangle = hungerRect;
    }

    public void Update(GameTime gameTime)
    {
        DecreaseBarsOverTime(gameTime);
    }

    private void DecreaseBarsOverTime(GameTime gameTime)
    {
        health -= decreaseRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
        hunger -= decreaseRate * (float)gameTime.ElapsedGameTime.TotalSeconds;

        health = MathHelper.Clamp(health, 0, 100);
        hunger = MathHelper.Clamp(hunger, 0, 100);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(healthBarTexture, new Rectangle(healthBarRectangle.X, healthBarRectangle.Y, (int)(healthBarRectangle.Width * (health / 100f)), healthBarRectangle.Height), Color.Red);
        spriteBatch.Draw(hungerBarTexture, new Rectangle(hungerBarRectangle.X, hungerBarRectangle.Y, (int)(hungerBarRectangle.Width * (hunger / 100f)), hungerBarRectangle.Height), Color.Green);
    }

    public void EatFood(float amount)
    {
        hunger += amount;
        hunger = MathHelper.Clamp(hunger, 0, 100);
    }

    public void Heal(float amount)
    {
        health += amount;
        health = MathHelper.Clamp(health, 0, 100);
    }
}