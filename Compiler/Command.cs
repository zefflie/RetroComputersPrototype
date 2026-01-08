namespace Retro.Compiler;

public enum Directive
{
    EQU,
    DB,
    SKIP,

    Unknown,
}

public enum Instruction
{
    NOP,
    MOV,
    LDA,
    STA,
    HLT,

    Unknown,
}

public class Command
{
    public string? Label { get; set; }

    public Directive? Directive { get; set; }

    public Instruction? Instruction { get; set; }

    public List<Argument> Args { get; set; } = [];
}

public enum ArgumentType
{
    Unknown,
    Literal,
    String,
    Label,
    Register,
    RegisterPair,
}
public class Argument
{
    public ArgumentType Type = ArgumentType.Unknown;

    public int Value = 0;

    public List<byte> Values = [];

    public string? Label;

    public bool IsMemoryAccess = false;
}