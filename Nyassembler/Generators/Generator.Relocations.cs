using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeMOV(Command command)
    {
        if (command.Args.Count != 2) throw new("MOV accepts two operands");
        var dst = command.Args[0];
        var src = command.Args[1];
        if (dst.Type != ArgumentType.Register) throw new($"MOV operand #1 type {dst.Type}, Register expected");
        if (src.Type != ArgumentType.Register) throw new($"MOV operand #2 type {src.Type}, Register expected");

        if (dst.IsMemoryAccess && src.IsMemoryAccess) throw new($"MOV can accept only one memory-access operand");
        Write((byte)(0b01000000 | dst.Value << 3 | src.Value));
    }

    public void EncodeMVI(Command command)
    {
        if (command.Args.Count != 2) throw new("MVI accepts two operands");
        var dst = command.Args[0];
        if (dst.Type != ArgumentType.Register) throw new($"MVI operand #1 type {dst.Type}, Register expected");
        Write(0b01000110 | dst.Value << 3);

        var data = command.Args[1];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 2);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 2);
        else throw new($"MVI operand #2 type {data.Type}, Literal expected");
    }

    public void EncodeLXI(Command command)
    {
        if (command.Args.Count != 2) throw new("LXI accepts two operands");
        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"LXI operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00000001 | rp.Value << 4));

        var data = command.Args[1];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 2);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 2);
        else throw new($"LXI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeLDA(Command command)
    {
        if (command.Args.Count != 1) throw new("LDA accepts one operand");
        Write(0b00111010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"LDA operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeLHLD(Command command)
    {
        if (command.Args.Count != 1) throw new("LHLD accepts one operand");
        Write(0b00101010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"LHLD operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeLDAX(Command command)
    {
        if (command.Args.Count != 1) throw new("LDAX accepts one operand");

        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"LDAX operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00001010 | rp.Value << 4));

    }

    public void EncodeXCHG(Command command)
    {
        if (command.Args.Count != 0) throw new("XCHG don't accept operands");
        Write(0b11101011);
    }

    public void EncodeSTA(Command command)
    {
        if (command.Args.Count != 1) throw new("STA accepts one operand");
        Write(0b00110010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"STA operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeSHLD(Command command)
    {
        if (command.Args.Count != 1) throw new("SHLD accepts one operand");
        Write(0b00100010);
        var addr = command.Args[0];

        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"SHLD operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeSTAX(Command command)
    {
        if (command.Args.Count != 1) throw new("STAX accepts one operand");

        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"STAX operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00000010 | rp.Value << 4));

    }
}
