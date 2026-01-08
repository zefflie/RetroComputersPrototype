using Retro.Compiler.Enums;

namespace Retro.Compiler.Generators;

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
}
