using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sprint0.Commands;
using Sprint0.Interfaces;

namespace Sprint0.Controllers;

public class MouseController : IController
{
    private readonly Game game;
    private readonly IPlayer player;
    private readonly ICommand rightClickCommand;
    private MouseState previousState;

    public MouseController(Game game, IPlayer player, ICommand rightClickCommand)
    {
        this.game = game;
        this.player = player;
        this.rightClickCommand = rightClickCommand;
    }

    public void Update()
    {
        MouseState currentState = Mouse.GetState();
        if (game.IsActive)
        {
            if (WasClicked(currentState.LeftButton, previousState.LeftButton) && InsideWindow(currentState))
            {
                new WalkToPointCommand(player, new Vector2(currentState.X, currentState.Y)).Execute();
            }
            if (WasClicked(currentState.RightButton, previousState.RightButton))
            {
                rightClickCommand.Execute();
            }
        }
        previousState = currentState;
    }

    private static bool WasClicked(ButtonState current, ButtonState previous)
    {
        return current == ButtonState.Pressed && previous == ButtonState.Released;
    }

    private bool InsideWindow(MouseState state)
    {
        return game.GraphicsDevice.Viewport.Bounds.Contains(state.X, state.Y);
    }
}
