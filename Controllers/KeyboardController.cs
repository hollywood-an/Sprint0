using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Sprint0.Interfaces;

namespace Sprint0.Controllers;

public class KeyboardController : IController
{
    private readonly Dictionary<Keys, ICommand> heldCommands = new Dictionary<Keys, ICommand>();
    private readonly Dictionary<Keys, ICommand> pressedCommands = new Dictionary<Keys, ICommand>();
    private KeyboardState previousState;

    public void RegisterHeldCommand(Keys key, ICommand command)
    {
        heldCommands[key] = command;
    }

    public void RegisterPressedCommand(Keys key, ICommand command)
    {
        pressedCommands[key] = command;
    }

    public void Update()
    {
        KeyboardState currentState = Keyboard.GetState();
        foreach (KeyValuePair<Keys, ICommand> binding in heldCommands)
        {
            if (currentState.IsKeyDown(binding.Key))
            {
                binding.Value.Execute();
            }
        }
        foreach (KeyValuePair<Keys, ICommand> binding in pressedCommands)
        {
            if (currentState.IsKeyDown(binding.Key) && previousState.IsKeyUp(binding.Key))
            {
                binding.Value.Execute();
            }
        }
        previousState = currentState;
    }
}
