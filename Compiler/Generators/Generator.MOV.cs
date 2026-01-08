using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeMOV(Command command)
    {
        if (command.Args.Count != 2) throw new("MOV accepts two arguments");
        var dst = command.Args[0];
        var src = command.Args[1];
        if (dst.Type != ArgumentType.Register) throw new($"MOV operand #1 type {dst.Type}, Register expected");
        if (src.Type != ArgumentType.Register) throw new($"MOV operand #2 type {dst.Type}, Register expected");

        if (dst.IsMemoryAccess && src.IsMemoryAccess) throw new($"MOV can accept only one memory-access operand");
        Write((byte)(0b01000000 | dst.Value << 3 | src.Value));
    }
}
