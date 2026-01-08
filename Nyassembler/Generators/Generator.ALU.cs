using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeADD(Command command)
    {
        if (command.Args.Count != 1) throw new("ADD accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"ADD operand #1 type {src.Type}, Register expected");
        Write(0b10000000 | src.Value);
    }

    public void EncodeADI(Command command)
    {
        if (command.Args.Count != 1) throw new("ADI accepts one operand");
        Write(0b11000110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"ADI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeADC(Command command)
    {
        if (command.Args.Count != 1) throw new("ADC accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"ADC operand #1 type {src.Type}, Register expected");
        Write(0b10001000 | src.Value);
    }

    public void EncodeACI(Command command)
    {
        if (command.Args.Count != 1) throw new("ACI accepts one operand");
        Write(0b11001110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"ACI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeSUB(Command command)
    {
        if (command.Args.Count != 1) throw new("SUB accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"SUB operand #1 type {src.Type}, Register expected");
        Write(0b10010000 | src.Value);
    }

    public void EncodeSUI(Command command)
    {
        if (command.Args.Count != 1) throw new("SUI accepts one operand");
        Write(0b11010110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"SUI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeSBB(Command command)
    {
        if (command.Args.Count != 1) throw new("SBB accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"SBB operand #1 type {src.Type}, Register expected");
        Write(0b10011000 | src.Value);
    }

    public void EncodeSBI(Command command)
    {
        if (command.Args.Count != 1) throw new("SBI accepts one operand");
        Write(0b11011110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"SBI operand #1 type {data.Type}, Literal expected");
    }
    public void EncodeANA(Command command)
    {
        if (command.Args.Count != 1) throw new("ANA accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"ANA operand #1 type {src.Type}, Register expected");
        Write(0b10100000 | src.Value);
    }

    public void EncodeANI(Command command)
    {
        if (command.Args.Count != 1) throw new("ANI accepts one operand");
        Write(0b11100110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"ANI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeXRA(Command command)
    {
        if (command.Args.Count != 1) throw new("XRA accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"XRA operand #1 type {src.Type}, Register expected");
        Write(0b10101000 | src.Value);
    }

    public void EncodeXRI(Command command)
    {
        if (command.Args.Count != 1) throw new("XRI accepts one operand");
        Write(0b11101110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"XRI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeORA(Command command)
    {
        if (command.Args.Count != 1) throw new("ORA accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"ORA operand #1 type {src.Type}, Register expected");
        Write(0b10110000 | src.Value);
    }

    public void EncodeORI(Command command)
    {
        if (command.Args.Count != 1) throw new("ORI accepts one operand");
        Write(0b11110110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"ORI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeCMP(Command command)
    {
        if (command.Args.Count != 1) throw new("CMP accepts one operand");

        var src = command.Args[0];
        if (src.Type != ArgumentType.Register) throw new($"CMP operand #1 type {src.Type}, Register expected");
        Write(0b10111000 | src.Value);
    }

    public void EncodeCPI(Command command)
    {
        if (command.Args.Count != 1) throw new("CPI accepts one operand");
        Write(0b11111110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"CPI operand #1 type {data.Type}, Literal expected");
    }

    public void EncodeINR(Command command)
    {
        if (command.Args.Count != 1) throw new("INR accepts one operand");

        var dst = command.Args[0];
        if (dst.Type != ArgumentType.Register) throw new($"INR operand #1 type {dst.Type}, Register expected");
        Write(0b00000100 | dst.Value << 3);
    }

    public void EncodeINX(Command command)
    {
        if (command.Args.Count != 1) throw new("INX accepts one operand");
        var rp = command.Args[0];

        if (rp.Type != ArgumentType.RegisterPair) throw new($"INX operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00000011 | rp.Value << 4));
    }

    public void EncodeDCR(Command command)
    {
        if (command.Args.Count != 1) throw new("DCR accepts one operands");

        var dst = command.Args[0];
        if (dst.Type != ArgumentType.Register) throw new($"DCR operand #1 type {dst.Type}, Register expected");
        Write(0b00000101 | dst.Value << 3);
    }

    public void EncodeDCX(Command command)
    {
        if (command.Args.Count != 1) throw new("DCX accepts one operands");
        var rp = command.Args[0];

        if (rp.Type != ArgumentType.RegisterPair) throw new($"DCX operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00001011 | rp.Value << 4));
    }

    public void EncodeDAD(Command command)
    {
        if (command.Args.Count != 1) throw new("DAD accepts one operands");
        var rp = command.Args[0];

        if (rp.Type != ArgumentType.RegisterPair) throw new($"DAD operand #1 type {rp.Type}, RegisterPair expected");
        Write((byte)(0b00001001 | rp.Value << 4));
    }

    public void EncodeRLC(Command command)
    {
        if (command.Args.Count != 0) throw new("RLC don't accept operands");
        Write(0b00000111);
    }

    public void EncodeRRC(Command command)
    {
        if (command.Args.Count != 0) throw new("RRC don't accept operands");
        Write(0b00001111);
    }

    public void EncodeRAL(Command command)
    {
        if (command.Args.Count != 0) throw new("RAL don't accept operands");
        Write(0b00010111);
    }

    public void EncodeRAR(Command command)
    {
        if (command.Args.Count != 0) throw new("RAR don't accept operands");
        Write(0b00011111);
    }

    public void EncodeDAA(Command command)
    {
        if (command.Args.Count != 0) throw new("DAA don't accept operands");
        Write(0b00100111);
    }

    public void EncodeCMA(Command command)
    {
        if (command.Args.Count != 0) throw new("CMA don't accept operands");
        Write(0b00101111);
    }

    public void EncodeSTC(Command command)
    {
        if (command.Args.Count != 0) throw new("STC don't accept operands");
        Write(0b00110111);
    }

    public void EncodeCMC(Command command)
    {
        if (command.Args.Count != 0) throw new("CMC don't accept operands");
        Write(0b00111111);
    }
}
