using Retro.Compiler.Enums;
using System.Security.Cryptography;

namespace Retro.Compiler.Generators;

public partial class Generator
{
    public void EncodeCPI(Command command)
    {
        if (command.Args.Count != 1) throw new("CPI accepts one argument");
        Write(0b11111110);

        var data = command.Args[0];
        if (data.Type == ArgumentType.Literal) Write(data.Value, 1);
        else if (data.Type == ArgumentType.Label) AddLink(data.Label, 1);
        else throw new($"LXI operand #2 type {data.Type}, Literal|Label expected");
    }
}
