using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HalfCircleTrail
{
    public Vector2 Position { get; set; }
    public float InitialRadius { get; set; }
    public float MaxRadius { get; set; }
    public float GrowthRate { get; set; }
    public float Opacity { get; set; }
    public float Lifespan { get; set; }
    private float _elapsedTime;

    public float Rotation { get; set; }

    public HalfCircleTrail(Vector2 position, float initialRadius, float maxRadius, float lifespan, float rotation)
    {
        Position = position;
        InitialRadius = initialRadius;
        MaxRadius = maxRadius;
        GrowthRate = (maxRadius - initialRadius) / lifespan;
        Opacity = 1f;
        Lifespan = lifespan;
        _elapsedTime = 0f;
        Rotation = rotation;
    }

    public bool IsExpired => _elapsedTime >= Lifespan;

    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float progress = _elapsedTime / Lifespan;

        if (_elapsedTime < Lifespan)
        {
            float currentRadius = MathHelper.Lerp(InitialRadius, MaxRadius, progress);
            Opacity = MathHelper.Lerp(1f, 0f, progress);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D circleTexture)
    {
        if (_elapsedTime < Lifespan)
        {
            float scale = MathHelper.Lerp(InitialRadius, MaxRadius, _elapsedTime / Lifespan) / circleTexture.Width;
            spriteBatch.Draw(
                circleTexture,
                Position,
                null,
                Color.White * Opacity,
                Rotation, 
                new Vector2(circleTexture.Width / 2, circleTexture.Height), 
                new Vector2(scale, scale / 2), 
                SpriteEffects.None,
                0f
            );
        }
    }
}
