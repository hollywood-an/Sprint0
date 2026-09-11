using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Sprites;

public class AnimatedSprite : ISprite
{
    private readonly Texture2D texture;
    private readonly int row;
    private readonly int frameCount;
    private readonly int frameWidth;
    private readonly int frameHeight;
    private readonly double secondsPerFrame;
    private readonly float scale;
    private int currentFrame;
    private double elapsedSeconds;

    public AnimatedSprite(Texture2D texture, int row, int frameCount, int frameWidth, int frameHeight, double secondsPerFrame, float scale)
    {
        this.texture = texture;
        this.row = row;
        this.frameCount = frameCount;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.secondsPerFrame = secondsPerFrame;
        this.scale = scale;
    }

    public void Update(GameTime gameTime)
    {
        elapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        while (elapsedSeconds >= secondsPerFrame)
        {
            elapsedSeconds -= secondsPerFrame;
            currentFrame = (currentFrame + 1) % frameCount;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
        Vector2 roundedPosition = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
        spriteBatch.Draw(texture, roundedPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}
