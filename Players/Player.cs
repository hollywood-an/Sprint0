using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;
using Sprint0.Sprites;

namespace Sprint0.Players;

public class Player : IPlayer
{
    private const int FrameSize = 16;
    private const float Scale = 4f;
    private const int WalkFrameCount = 4;
    private const int IdleFrameCount = 2;
    private const double WalkSecondsPerFrame = 0.12;
    private const double IdleSecondsPerFrame = 0.45;
    private const float WalkSpeed = 140f;
    private const float DashMultiplier = 2.6f;
    private const double DashDuration = 0.22;
    private const double DashCooldown = 0.8;

    private readonly Dictionary<Direction, ISprite> walkSprites;
    private readonly Dictionary<Direction, ISprite> idleSprites;
    private readonly Rectangle movementBounds;
    private Vector2 position;
    private Vector2 pendingDirection;
    private Vector2? walkTarget;
    private Direction facing;
    private ISprite activeSprite;
    private double dashSecondsRemaining;
    private double dashCooldownRemaining;

    public Player(Texture2D spriteSheet, Vector2 startPosition, Rectangle movementBounds)
    {
        this.movementBounds = movementBounds;
        position = startPosition;
        facing = Direction.Down;
        walkSprites = BuildSprites(spriteSheet, 0, WalkFrameCount, WalkSecondsPerFrame);
        idleSprites = BuildSprites(spriteSheet, 4, IdleFrameCount, IdleSecondsPerFrame);
        activeSprite = idleSprites[facing];
    }

    private static Dictionary<Direction, ISprite> BuildSprites(Texture2D spriteSheet, int firstRow, int frameCount, double secondsPerFrame)
    {
        return new Dictionary<Direction, ISprite>
        {
            [Direction.Down] = new AnimatedSprite(spriteSheet, firstRow, frameCount, FrameSize, FrameSize, secondsPerFrame, Scale),
            [Direction.Up] = new AnimatedSprite(spriteSheet, firstRow + 1, frameCount, FrameSize, FrameSize, secondsPerFrame, Scale),
            [Direction.Left] = new AnimatedSprite(spriteSheet, firstRow + 2, frameCount, FrameSize, FrameSize, secondsPerFrame, Scale),
            [Direction.Right] = new AnimatedSprite(spriteSheet, firstRow + 3, frameCount, FrameSize, FrameSize, secondsPerFrame, Scale)
        };
    }

    public void Move(Direction direction)
    {
        pendingDirection += DirectionVector(direction);
        walkTarget = null;
    }

    public void MoveTo(Vector2 target)
    {
        Vector2 topLeft = target - new Vector2(FrameSize * Scale / 2f);
        walkTarget = ClampToBounds(topLeft);
    }

    public void StopMoving()
    {
        walkTarget = null;
        pendingDirection = Vector2.Zero;
    }

    public void Dash()
    {
        if (dashCooldownRemaining <= 0)
        {
            dashSecondsRemaining = DashDuration;
            dashCooldownRemaining = DashCooldown;
        }
    }

    public void Update(GameTime gameTime)
    {
        float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float currentSpeed = WalkSpeed * (dashSecondsRemaining > 0 ? DashMultiplier : 1f);
        dashSecondsRemaining = Math.Max(0, dashSecondsRemaining - deltaSeconds);
        dashCooldownRemaining = Math.Max(0, dashCooldownRemaining - deltaSeconds);

        Vector2 velocity = Vector2.Zero;
        if (pendingDirection != Vector2.Zero)
        {
            velocity = Vector2.Normalize(pendingDirection) * currentSpeed;
        }
        else if (walkTarget.HasValue)
        {
            Vector2 delta = walkTarget.Value - position;
            if (delta.Length() <= currentSpeed * deltaSeconds)
            {
                position = walkTarget.Value;
                walkTarget = null;
            }
            else
            {
                velocity = Vector2.Normalize(delta) * currentSpeed;
            }
        }
        else if (dashSecondsRemaining > 0)
        {
            velocity = DirectionVector(facing) * currentSpeed;
        }

        position = ClampToBounds(position + velocity * deltaSeconds);

        if (velocity != Vector2.Zero)
        {
            facing = DominantDirection(velocity);
        }

        activeSprite = (velocity != Vector2.Zero ? walkSprites : idleSprites)[facing];
        activeSprite.Update(gameTime);
        pendingDirection = Vector2.Zero;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        activeSprite.Draw(spriteBatch, position);
    }

    private Vector2 ClampToBounds(Vector2 value)
    {
        float scaledSize = FrameSize * Scale;
        float clampedX = Math.Clamp(value.X, movementBounds.Left, movementBounds.Right - scaledSize);
        float clampedY = Math.Clamp(value.Y, movementBounds.Top, movementBounds.Bottom - scaledSize);
        return new Vector2(clampedX, clampedY);
    }

    private static Direction DominantDirection(Vector2 velocity)
    {
        if (Math.Abs(velocity.X) >= Math.Abs(velocity.Y))
        {
            return velocity.X < 0 ? Direction.Left : Direction.Right;
        }
        return velocity.Y < 0 ? Direction.Up : Direction.Down;
    }

    private static Vector2 DirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return new Vector2(0, -1);
            case Direction.Down: return new Vector2(0, 1);
            case Direction.Left: return new Vector2(-1, 0);
            default: return new Vector2(1, 0);
        }
    }
}
