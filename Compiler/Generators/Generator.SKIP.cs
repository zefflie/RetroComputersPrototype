using Retro.Nyassembler.Enums;
using Retro.Nyassembler;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeSKIP(Command command)
    {
        if (command.Args.Count != 1) throw new("SKIP accepts one argument");
        var arg = command.Args[0];
        if (arg.Type != ArgumentType.Literal) throw new("EQU argument must be literal");

        State.Data.AddRange(new byte[arg.Value]);
        State.Position += arg.Value;
    }
}
