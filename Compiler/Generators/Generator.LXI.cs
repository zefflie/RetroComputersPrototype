using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeLXI(Command command)
    {
        if (command.Args.Count != 2) throw new("LXI accepts two operands");
        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"LXI operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b1 | rp.Value << 4));

        var arg = command.Args[1];
        if (arg.Type == ArgumentType.Literal) Write(arg.Value, 2);
        else if (arg.Type == ArgumentType.Label) AddLink(arg.Label, 2);
        else throw new($"LXI operand #2 type {arg.Type}, LiteraLabel expected");
    }
}
