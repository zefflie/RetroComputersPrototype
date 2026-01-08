using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeNOP(Command command)
    {
        State.Data.Add(0); State.Position++;
    }

    public void EncodeHLT(Command command)
    {
        State.Data.Add(0b01110110);
        State.Position++;
    }

    public void EncodeJMP(Command command)
    {
        if (command.Args.Count != 1) throw new("JMP accepts one argument");
        Write(0b11000011);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"JMP operand #1 type {addr.Type}, Literal|Label expected");
    }

    public void EncodeJZ(Command command)
    {
        if (command.Args.Count != 1) throw new("JZ accepts one argument");
        Write(0b11001010);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"JZ operand #1 type {addr.Type}, Literal|Label expected");
    }
}
