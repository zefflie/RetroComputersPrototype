using Retro.Nyassembler.Enums;
using Retro.Nyassembler;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeSTA(Command command)
    {
        if (command.Args.Count != 1) throw new("STA accepts one operand");
        Write(0b00110010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"Invalid operand type {addr.Type} for STA");
    }
}
