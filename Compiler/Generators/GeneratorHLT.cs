using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeHLT(Command command)
    {
        State.Data.Add(0b01110110);
        State.Position++;
    }
}
