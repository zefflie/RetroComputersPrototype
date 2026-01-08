using Retro.Nyassembler.Enums;
using Retro.Nyassembler;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeHLT(Command command)
    {
        State.Data.Add(0b01110110);
        State.Position++;
    }
}
