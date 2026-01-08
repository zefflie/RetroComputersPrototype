using Retro.Compiler.Generators;

namespace Retro.Compiler;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string[] data = File.ReadAllLines("C:\\Zefflie\\Projects\\RetroComputersPrototype\\test.nya");
        var clean = new Preprocessor(data).Process();
        var parsed = new Parser(clean).Process();
        var file = new Generator(parsed).Process();

        File.WriteAllBytes("C:\\Zefflie\\Projects\\RetroComputersPrototype\\test.bin", file.Data.ToArray());

        Console.WriteLine(0);
    }
}
