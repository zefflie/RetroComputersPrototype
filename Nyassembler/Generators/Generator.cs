using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public List<Command> Parsed;
    public GeneratorState State;

    public Generator(List<Command> parsed)
    {
        Parsed = parsed;
        State = new();
    }

    public GeneratorState Process()
    {
        // Кодогенерация и запоминание символов
        foreach (var command in Parsed)
        {
            if (command.Label != null)
            {
                GetSymbol(command.Label).Value = State.Position;
                State.LastLabel = command.Label;
            }

            if (command.Directive != null) ProcessDirective(command);
            if (command.Instruction != null) ProcessInstruction(command);
        }

        // Разрещение символов

        foreach (var symbol in State.Symbols.Values)
        {

            foreach (var link in symbol.Links)
            {
                var bytes = IntToBytes(symbol.Value, link.Size);
                for (int i = 0; i < link.Size; i++) State.Data[link.Address + i] = bytes[i];
            }
        }

        return State;
    }

    public void ProcessDirective(Command command)
    {
        switch (command.Directive)
        {
            case Directive.EQU: EncodeEQU(command); break;
            case Directive.DB: EncodeDB(command); break;
            case Directive.SKIP: EncodeSKIP(command); break;
            default: throw new NotImplementedException(command.Directive.ToString());
        }
    }

    public void ProcessInstruction(Command command)
    {
        switch (command.Instruction)
        {
            case Instruction.MOV: EncodeMOV(command); break;
            case Instruction.MVI: EncodeMVI(command); break;
            case Instruction.LXI: EncodeLXI(command); break;
            case Instruction.LDA: EncodeLDA(command); break;
            case Instruction.LHLD: EncodeLHLD(command); break;
            case Instruction.LDAX: EncodeLDAX(command); break;
            case Instruction.XCHG: EncodeXCHG(command); break;
            case Instruction.STA: EncodeSTA(command); break;
            case Instruction.SHLD: EncodeSHLD(command); break;
            case Instruction.STAX: EncodeSTAX(command); break;

            case Instruction.ADD: EncodeADD(command); break;
            case Instruction.ADI: EncodeADI(command); break;
            case Instruction.ADC: EncodeADC(command); break;
            case Instruction.ACI: EncodeACI(command); break;
            case Instruction.SUB: EncodeSUB(command); break;
            case Instruction.SUI: EncodeSUI(command); break;
            case Instruction.SBB: EncodeSBB(command); break;
            case Instruction.SBI: EncodeSBI(command); break;
            case Instruction.ANA: EncodeANA(command); break;
            case Instruction.ANI: EncodeANI(command); break;
            case Instruction.XRA: EncodeXRA(command); break;
            case Instruction.XRI: EncodeXRI(command); break;
            case Instruction.ORA: EncodeORA(command); break;
            case Instruction.ORI: EncodeORI(command); break;
            case Instruction.CMP: EncodeCMP(command); break;
            case Instruction.CPI: EncodeCPI(command); break;
            case Instruction.INR: EncodeINR(command); break;
            case Instruction.INX: EncodeINX(command); break;
            case Instruction.DCR: EncodeDCR(command); break;
            case Instruction.DCX: EncodeDCX(command); break;
            case Instruction.DAD: EncodeDAD(command); break;
            case Instruction.RLC: EncodeRLC(command); break;
            case Instruction.RRC: EncodeRRC(command); break;
            case Instruction.RAL: EncodeRAL(command); break;
            case Instruction.RAR: EncodeRAR(command); break;
            case Instruction.DAA: EncodeDAA(command); break;
            case Instruction.CMA: EncodeCMA(command); break;
            case Instruction.STC: EncodeSTC(command); break;
            case Instruction.CMC: EncodeCMC(command); break;

            case Instruction.PCHL: EncodePCHL(command); break;
            case Instruction.JNZ:
            case Instruction.JZ:
            case Instruction.JNC:
            case Instruction.JC:
            case Instruction.JPO:
            case Instruction.JPE:
            case Instruction.JP:
            case Instruction.JM: EncodeJcc(command); break;
            case Instruction.CNZ:
            case Instruction.CZ:
            case Instruction.CNC:
            case Instruction.CC:
            case Instruction.CPO:
            case Instruction.CPE:
            case Instruction.CP:
            case Instruction.CM: EncodeCcc(command); break;
            case Instruction.RNZ:
            case Instruction.RZ:
            case Instruction.RNC:
            case Instruction.RC:
            case Instruction.RPO:
            case Instruction.RPE:
            case Instruction.RP:
            case Instruction.RM: EncodeRcc(command); break;
            case Instruction.RST: EncodeRST(command); break;
            case Instruction.PUSH: EncodePUSH(command); break;
            case Instruction.POP: EncodePOP(command); break;
            case Instruction.XTHL: EncodeXTHL(command); break;
            case Instruction.SPHL: EncodeSPHL(command); break;
            case Instruction.IN: EncodeIN(command); break;
            case Instruction.OUT: EncodeOUT(command); break;
            case Instruction.EI: EncodeEI(command); break;
            case Instruction.DI: EncodeDI(command); break;
            case Instruction.HLT: EncodeHLT(command); break;
            case Instruction.NOP: EncodeNOP(command); break;
            default: throw new NotImplementedException(command.Instruction.ToString());
        }
    }

    public void Write(byte value)
    {
        State.Data.Add(value);
        State.Position++;
    }

    public void Write(int value, int size = -1)
    {
        var bytes = IntToBytes(value, size);
        State.Data.AddRange(bytes);
        State.Position += bytes.Count();
    }

    public void Write(IEnumerable<byte> values)
    {
        State.Data.AddRange(values);
        State.Position += values.Count();
    }

    public void AddLink(string label, int size)
    {
        GetSymbol(label).Links.Add(new(State.Position, size));
        Write(new byte[size]);
    }

    public List<byte> IntToBytes(int value, int size = -1)
    {
        List<byte> bytes = BitConverter.GetBytes(value)
            .Reverse().SkipWhile(b => b == 0).Reverse().ToList();

        if (size > -1) {
            if (bytes.Count > size) throw new("Too big literal");

            if (bytes.Count < size)
            {
                bytes.AddRange(new byte[size - bytes.Count]);
            }
        }

        return bytes;
    }

    public SymbolInfo GetSymbol(string label)
    {
        if (!State.Symbols.ContainsKey(label)) State.Symbols[label] = new();
        return State.Symbols[label];
    }
}

public class GeneratorState
{
    public List<byte> Data = [];

    public Dictionary<string, SymbolInfo> Symbols = [];

    public int Position;

    public string LastLabel = "";
}

public class SymbolInfo
{
    public int Value { get; set; }

    public List<Link> Links = [];
}

public class Link(int address, int size)
{
    public int Address = address;

    public int Size = size;
}