using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeCPI(Command command)
    {
        if (command.Args.Count != 1) throw new("CPI accepts one argument");
        Write(0b11111110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"CPI operand #2 type {data.Type}, Literal|Label expected");
    }

    public void EncodeINX(Command command)
    {
        if (command.Args.Count != 1) throw new("INX accepts one argument");
        var rp = command.Args[0];

        if (rp.Type != ArgumentType.RegisterPair) throw new($"INX operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00000011 | rp.Value << 4));
    }
}
