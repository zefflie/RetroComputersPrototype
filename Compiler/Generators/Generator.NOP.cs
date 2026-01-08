using Retro.Nyassembler.Enums;
using Retro.Nyassembler;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeNOP(Command command)
    {
        State.Data.Add(0); State.Position++;
    }
}
