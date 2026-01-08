using Retro.Nyassembler.Enums;

namespace Retro.Nyassembler.Generators;

public partial class Generator
{
    public void EncodeDB(Command command)
    {
        foreach (var arg in command.Args)
        {
            switch (arg.Type)
            {
                case ArgumentType.Literal:
                    {
                        var data = IntToBytes(arg.Value);

                        State.Data.AddRange(data);
                        State.Position += data.Count;
                        break;
                    }

                case ArgumentType.String:
                    State.Data.AddRange(arg.Values);
                    State.Position += arg.Values.Count;
                    break;

                case ArgumentType.Label:
                    GetSymbol(arg.Label).Links.Add(new(State.Position, 2));
                    State.Data.AddRange([0, 0]);
                    State.Position += 2;
                    break;

                default: throw new("Invalid operand type for DB");
            }
        }
    }
    public void EncodeEQU(Command command)
    {
        if (command.Args.Count != 1) throw new("EQU accepts one operand");
        var arg = command.Args[0];
        if (arg.Type != ArgumentType.Literal) throw new("EQU operand must be literal");
        State.Symbols[State.LastLabel].Value = arg.Value;
    }

    public void EncodeSKIP(Command command)
    {
        if (command.Args.Count != 1) throw new("SKIP accepts one operand");
        var arg = command.Args[0];
        if (arg.Type != ArgumentType.Literal) throw new("SKIP operand must be literal");

        State.Data.AddRange(new byte[arg.Value]);
        State.Position += arg.Value;
    }
}
