using Sprint0.Interfaces;

namespace Sprint0.Commands;

public class DashCommand : ICommand
{
    private readonly IPlayer player;

    public DashCommand(IPlayer player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.Dash();
    }
}
