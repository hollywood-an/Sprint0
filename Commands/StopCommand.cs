using Sprint0.Interfaces;

namespace Sprint0.Commands;

public class StopCommand : ICommand
{
    private readonly IPlayer player;

    public StopCommand(IPlayer player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.StopMoving();
    }
}
