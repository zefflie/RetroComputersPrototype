using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeNOP(Command command)
    {
        State.Data.Add(0); State.Position++;
    }
}
