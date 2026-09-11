using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Commands;

public class WalkToPointCommand : ICommand
{
    private readonly IPlayer player;
    private readonly Vector2 target;

    public WalkToPointCommand(IPlayer player, Vector2 target)
    {
        this.player = player;
        this.target = target;
    }

    public void Execute()
    {
        player.MoveTo(target);
    }
}
