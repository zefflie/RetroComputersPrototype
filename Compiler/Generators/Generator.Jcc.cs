using Retro.Compiler.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Retro.Compiler.Generators;

public partial class Generator
{
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
