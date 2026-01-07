using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Retro.Compiler;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string[] data = File.ReadAllLines("C:\\Zefflie\\Projects\\RetroComputersPrototype\\test.nya");
        var clean = new Preprocessor(data).Process();
        var parsed = new Parser(clean).Process();

        Console.WriteLine(0);
    }
}

