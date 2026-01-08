using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler;

public class Command
{
    public string? Label { get; set; }

    public Directive? Directive { get; set; }

    public Instruction? Instruction { get; set; }

    public List<Argument> Args { get; set; } = [];
}

public class Argument
{
    public ArgumentType Type = ArgumentType.Unknown;

    public int Value = 0;

    public List<byte> Values = [];

    public string? Label;

    public bool IsMemoryAccess = false;
}