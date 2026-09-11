using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprint0.Commands;
using Sprint0.Controllers;
using Sprint0.Interfaces;
using Sprint0.Players;

namespace Sprint0;

public class Game1 : Game
{
    private const int ScreenWidth = 800;
    private const int ScreenHeight = 600;
    private const int PlayerPixelSize = 64;
    private static readonly Color GrassGreen = new Color(58, 110, 66);

    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private IPlayer player;
    private List<IController> controllers;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
        graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D spriteSheet;
        using (Stream stream = TitleContainer.OpenStream("Content/adventurer.png"))
        {
            spriteSheet = Texture2D.FromStream(GraphicsDevice, stream);
        }

        Rectangle screenBounds = GraphicsDevice.Viewport.Bounds;
        Vector2 screenCenter = new Vector2((ScreenWidth - PlayerPixelSize) / 2f, (ScreenHeight - PlayerPixelSize) / 2f);
        player = new Player(spriteSheet, screenCenter, screenBounds);

        controllers = new List<IController>
        {
            BuildKeyboardController(),
            new MouseController(this, player, new StopCommand(player))
        };
    }

    private IController BuildKeyboardController()
    {
        KeyboardController keyboard = new KeyboardController();

        ICommand moveUp = new MoveCommand(player, Direction.Up);
        ICommand moveDown = new MoveCommand(player, Direction.Down);
        ICommand moveLeft = new MoveCommand(player, Direction.Left);
        ICommand moveRight = new MoveCommand(player, Direction.Right);
        keyboard.RegisterHeldCommand(Keys.W, moveUp);
        keyboard.RegisterHeldCommand(Keys.S, moveDown);
        keyboard.RegisterHeldCommand(Keys.A, moveLeft);
        keyboard.RegisterHeldCommand(Keys.D, moveRight);

        ICommand quit = new QuitCommand(this);
        keyboard.RegisterPressedCommand(Keys.Space, new DashCommand(player));
        keyboard.RegisterPressedCommand(Keys.Q, quit);
        keyboard.RegisterPressedCommand(Keys.Escape, quit);

        return keyboard;
    }

    protected override void Update(GameTime gameTime)
    {
        foreach (IController controller in controllers)
        {
            controller.Update();
        }
        player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(GrassGreen);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
        player.Draw(spriteBatch);
        spriteBatch.End();
        base.Draw(gameTime);
    }
}
