using Sprint0.Interfaces;

namespace Sprint0.Commands;

public class MoveCommand : ICommand
{
    private readonly IPlayer player;
    private readonly Direction direction;

    public MoveCommand(IPlayer player, Direction direction)
    {
        this.player = player;
        this.direction = direction;
    }

    public void Execute()
    {
        player.Move(direction);
    }
}
