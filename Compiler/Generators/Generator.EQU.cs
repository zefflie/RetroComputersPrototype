using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeEQU(Command command)
    {
        if (command.Args.Count != 1) throw new("EQU accepts one argument");
        var arg = command.Args[0];
        if (arg.Type != ArgumentType.Literal) throw new("EQU argument must be literal");
        State.Symbols[State.LastLabel].Value = arg.Value;
    }
}
