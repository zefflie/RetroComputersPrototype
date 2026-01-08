using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeXCHG(Command command)
    {
        if (command.Args.Count != 0) throw new("XCHG don't accept arguments");
        Write(0b11101011);
    }
}
