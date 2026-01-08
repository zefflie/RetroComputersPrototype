using System.Security.Cryptography;

namespace Retro.Compiler;

public class Generator
{
    public List<Command> Parsed;
    public ObjectFile File;
    public int Position;
    public string LastLabel = "";

    public Generator(List<Command> parsed)
    {
        Parsed = parsed;
        File = new();
        Position = 0;
    }

    public ObjectFile Process()
    {
        // Кодогенерация и запоминание символов
        foreach (var command in Parsed)
        {
            if (command.Label != null)
            {
                GetSymbol(command.Label).Value = Position;
                LastLabel = command.Label;
            }

            if (command.Directive != null) ProcessDirective(command);

            if (command.Instruction != null) ProcessInstruction(command);
        }

        // Разрещение символов

        foreach (var symbol in File.Symbols.Values)
        {

            foreach (var link in symbol.Links)
            {
                var bytes = IntToBytes(symbol.Value, link.Size);

                for (int i = 0; i < link.Size; i++) File.Data[link.Address + i] = bytes[i];
            }
        }

        return File;
    }

    public void ProcessDirective(Command command)
    {
        switch (command.Directive)
        {
            case Directive.EQU:
                {
                    if (command.Args.Count != 1) throw new("EQU accepts one argument");
                    var arg = command.Args[0];
                    if (arg.Type != ArgumentType.Literal) throw new("EQU argument must be literal");
                    File.Symbols[LastLabel].Value = arg.Value;
                    break;
                }

            case Directive.DB:
                foreach (var arg in command.Args)
                {
                    switch (arg.Type)
                    {
                        case ArgumentType.Literal:
                            {
                                var data = IntToBytes(arg.Value);

                                File.Data.AddRange(data);
                                Position += data.Count;
                                break;
                            }

                        case ArgumentType.String:
                            File.Data.AddRange(arg.Values);
                            Position += arg.Values.Count;
                            break;

                        case ArgumentType.Label:
                            GetSymbol(arg.Label).Links.Add(new(Position, 2));
                            File.Data.AddRange([0, 0]);
                            Position += 2;
                            break;

                        default: throw new("Invalid argument type for DB");
                    }
                }

                break;

            case Directive.SKIP:
                {
                    if (command.Args.Count != 1) throw new("SKIP accepts one argument");
                    var arg = command.Args[0];
                    if (arg.Type != ArgumentType.Literal) throw new("EQU argument must be literal");

                    File.Data.AddRange(new byte[arg.Value]);
                    Position += arg.Value;
                    break;
                }
        }
    }

    public void ProcessInstruction(Command command)
    {
        switch (command.Instruction)
        {
            case Instruction.NOP: File.Data.Add(0); Position++; break;

            case Instruction.MOV:
                {
                    if (command.Args.Count != 2) throw new("MOV accepts two arguments");
                    var dst = command.Args[0];
                    var src = command.Args[1];

                    if (dst.Type == ArgumentType.Register && src.Type == ArgumentType.Register)
                    {
                        File.Data.Add((byte)(0b01000000 | dst.Value << 3 | src.Value));
                        Position++;
                    }
                    else if (dst.Type == ArgumentType.Register && src.IsMemoryAccess) 
                    {
                        File.Data.Add((byte)(0b01000110 | dst.Value << 3));
                        Position++;

                        if (src.Type == ArgumentType.Literal)
                        {
                            var bytes = IntToBytes(src.Value, 2);
                            File.Data.AddRange(bytes);
                        }
                        else if (src.Type == ArgumentType.Label)
                        {
                            GetSymbol(src.Label).Links.Add(new(Position, 2));
                            File.Data.AddRange([0, 0]);
                        }
                        else
                        {
                            throw new("Invalid source type for MOV");
                        }

                        Position += 2;
                    }
                    else if (dst.IsMemoryAccess && src.Type == ArgumentType.Register)
                    {
                        File.Data.Add((byte)(0b01110000 | src.Value));
                        Position++;

                        if (dst.Type == ArgumentType.Literal)
                        {
                            var bytes = IntToBytes(dst.Value, 2);
                            File.Data.AddRange(bytes);
                        }
                        else if (dst.Type == ArgumentType.Label)
                        {
                            GetSymbol(dst.Label).Links.Add(new(Position, 2));
                            File.Data.AddRange([0, 0]);
                        }
                        else
                        {
                            throw new("Invalid source type for MOV");
                        }

                        Position += 2;
                    }
                    else
                    {
                        throw new("Invalid argument types for MOV");
                    }

                    break;
                }

            case Instruction.LDA:
                {
                    if (command.Args.Count != 1) throw new("LDA accepts one operand");
                    File.Data.Add(0b00111010); Position++;
                    var addr = command.Args[0];

                    if (addr.Type == ArgumentType.Literal)
                    {
                        var bytes = IntToBytes(addr.Value, 2);
                        File.Data.AddRange(bytes);
                    }
                    else if (addr.Type == ArgumentType.Label) {
                        GetSymbol(addr.Label).Links.Add(new(Position, 2));
                        File.Data.AddRange([0, 0]);
                    }
                    else
                    {
                        throw new("Invalid argument type for MOV");
                    }

                    Position += 2;

                    break;
                }

            case Instruction.STA:
                {
                    if (command.Args.Count != 1) throw new("STA accepts one operand");
                    File.Data.Add(0b00110010); Position++;
                    var addr = command.Args[0];

                    if (addr.Type == ArgumentType.Literal)
                    {
                        var bytes = IntToBytes(addr.Value, 2);
                        File.Data.AddRange(bytes);
                    }
                    else if (addr.Type == ArgumentType.Label)
                    {
                        GetSymbol(addr.Label).Links.Add(new(Position, 2));
                        File.Data.AddRange([0, 0]);
                    }
                    else
                    {
                        throw new("Invalid argument type for MOV");
                    }

                    Position += 2;

                    break;
                }

            case Instruction.HLT:
                File.Data.Add(0b01110110);
                Position++;
                break;
        }
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
        if (!File.Symbols.ContainsKey(label)) File.Symbols[label] = new();
        return File.Symbols[label];
    }
}

public class ObjectFile
{
    public List<byte> Data = [];

    public Dictionary<string, SymbolInfo> Symbols = [];
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