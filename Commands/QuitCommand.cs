using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Commands;

public class QuitCommand : ICommand
{
    private readonly Game game;

    public QuitCommand(Game game)
    {
        this.game = game;
    }

    public void Execute()
    {
        game.Exit();
    }
}
