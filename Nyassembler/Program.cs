using Retro.Nyassembler.Generators;

namespace Retro.Nyassembler;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Tela Nyassembler");

        List<string> src = [];
        string dst = "a.out";
        string option = "";

        foreach (var arg in args) 
        { 
            if (arg.StartsWith("-"))
            {
                if (option.Length > 0) throw new ArgumentException($"Option '{option}' requires parameter");
                switch (arg)
                {
                    case "-o": option = arg; break;
                    default: throw new ArgumentException($"Unknown '{arg}' option");
                }

                continue;
            }

            switch (option)
            {
                case "-o": dst = arg; break;
                default:
                    if (!File.Exists(arg)) throw new ArgumentException($"Source file {arg} not exists");
                    src.Add(arg);
                    break;
            }
        }

        string[] data = File.ReadAllLines(src[0]);
        var clean = new Preprocessor(data).Process();
        var parsed = new Parser(clean).Process();
        var file = new Generator(parsed).Process();
        File.WriteAllBytes(dst, file.Data.ToArray());
        Console.WriteLine($"{src[0]} > {dst}");
    }
}
