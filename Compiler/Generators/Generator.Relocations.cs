using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeLDA(Command command)
    {
        if (command.Args.Count != 1) throw new("LDA accepts one operand");
        State.Data.Add(0b00111010); State.Position++;
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal)
        {
            var bytes = IntToBytes(addr.Value, 2);
            State.Data.AddRange(bytes);
        }
        else if (addr.Type == ArgumentType.Label)
        {
            GetSymbol(addr.Label).Links.Add(new(State.Position, 2));
            State.Data.AddRange([0, 0]);
        }
        else
        {
            throw new("Invalid argument type for MOV");
        }

        State.Position += 2;
    }

    public void EncodeLXI(Command command)
    {
        if (command.Args.Count != 2) throw new("LXI accepts two operands");
        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"LXI operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b1 | rp.Value << 4));

        var arg = command.Args[1];
        if (arg.Type == ArgumentType.Literal) Write(arg.Value, 2);
        else if (arg.Type == ArgumentType.Label) AddLink(arg.Label, 2);
        else throw new($"LXI operand #2 type {arg.Type}, LiteraLabel expected");
    }

    public void EncodeSTA(Command command)
    {
        if (command.Args.Count != 1) throw new("STA accepts one operand");
        Write(0b00110010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"Invalid operand type {addr.Type} for STA");
    }

    public void EncodeXCHG(Command command)
    {
        if (command.Args.Count != 0) throw new("XCHG don't accept arguments");
        Write(0b11101011);
    }
}
