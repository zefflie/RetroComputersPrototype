using Retro.Nyassembler.Enums;
using Retro.Nyassembler;

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

                default: throw new("Invalid argument type for DB");
            }
        }
    }
}
