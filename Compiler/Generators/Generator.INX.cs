using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeINX(Command command)
    {
        if (command.Args.Count != 1) throw new("JZ accepts one argument");
        var rp = command.Args[0];

        if (rp.Type != ArgumentType.RegisterPair) throw new($"LXI operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00000011 | rp.Value << 4));
    }
}
