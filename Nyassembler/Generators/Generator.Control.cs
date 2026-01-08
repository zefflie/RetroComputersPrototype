using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodePCHL(Command command)
    {
        if (command.Args.Count != 0) throw new("PCHL don't accept operands");
        Write(0b11101001);
    }

    public void EncodeJMP(Command command)
    {
        if (command.Args.Count != 1) throw new("JMP accepts one operand");
        Write(0b11000011);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"JMP operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeJcc(Command command)
    {
        if (command.Args.Count != 1) throw new("Jcc accepts one operand");
        int con = (int)(command.Instruction - Instruction.JNZ);
        Write(0b11000010 | con << 3);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"Jcc operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeCALL(Command command)
    {
        if (command.Args.Count != 1) throw new("CALL accepts one operand");
        Write(0b11001101);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"CALL operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeCcc(Command command)
    {
        if (command.Args.Count != 1) throw new("Ccc accepts one operand");
        int con = (int)(command.Instruction - Instruction.CNZ);
        Write(0b11000100 | con << 3);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"Ccc operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeRET(Command command)
    {
        if (command.Args.Count != 1) throw new("RET accepts one operand");
        Write(0b11001001);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"RET operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeRcc(Command command)
    {
        if (command.Args.Count != 1) throw new("Rcc accepts one operand");
        int con = (int)(command.Instruction - Instruction.RNZ);
        Write(0b11001000 | con << 3);

        var addr = command.Args[0];
        if (addr.Type == ArgumentType.Literal) Write(addr.Value, 2);
        else if (addr.Type == ArgumentType.Label) AddLink(addr.Label, 2);
        else throw new($"Rcc operand #1 type {addr.Type}, Literal expected");
    }

    public void EncodeRST(Command command)
    {
        if (command.Args.Count != 1) throw new("RST accepts one operans");
        var dst = command.Args[0];
        if (dst.Type != ArgumentType.Literal) throw new($"RST operand #1 type {dst.Type}, Literal expected");
        if (dst.Value >= 8) throw new($"RST operand #1 out of bound 0-7");
        Write(0b11000111 | dst.Value << 3);
    }

    public void EncodePUSH(Command command)
    {
        if (command.Args.Count != 1) throw new("PUSH accepts one operand");
        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"PUSH operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b11000101 | rp.Value << 4));
    }

    public void EncodePOP(Command command)
    {
        if (command.Args.Count != 1) throw new("POP accepts one operand");
        var rp = command.Args[0];
        if (rp.Type != ArgumentType.RegisterPair) throw new($"POP operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b11000001 | rp.Value << 4));
    }

    public void EncodeXTHL(Command command)
    {
        if (command.Args.Count != 0) throw new("XTHL don't accept operands");
        Write(0b11100011);
    }

    public void EncodeSPHL(Command command)
    {
        if (command.Args.Count != 0) throw new("SPHL don't accept operands");
        Write(0b11111001);
    }

    public void EncodeIN(Command command)
    {
        if (command.Args.Count != 0) throw new("IN don't accept operands");
        Write(0b11011011);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"IN operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeOUT(Command command)
    {
        if (command.Args.Count != 0) throw new("OUT don't accept operands");
        Write(0b11010011);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"OUT operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeEI(Command command)
    {
        if (command.Args.Count != 0) throw new("EI don't accept operands");
        Write(0b11111011);
    }

    public void EncodeDI(Command command)
    {
        if (command.Args.Count != 0) throw new("DI don't accept operands");
        Write(0b11110011);
    }

    public void EncodeHLT(Command command)
    {
        if (command.Args.Count != 0) throw new("HLT don't accept operands");
        Write(0b01110110);
    }

    public void EncodeNOP(Command command)
    {
        if (command.Args.Count != 0) throw new("NOP don't accept operands");
        Write(0b00000000);
    }
}
