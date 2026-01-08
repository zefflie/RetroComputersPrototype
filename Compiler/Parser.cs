using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace Retro.Compiler;
public class Parser
{
    public static Regex RELabel = new(@"([_A-Za-z][_A-Za-z0-9]*):\s*");

    public static Regex REName = new(@"(\w+)\s+");

    static public Regex REMemory = new(@"\[\s*(.*)\s*\]");

    static public Regex REString = new(@"^""(.*)""$");

    static public Regex REChar = new(@"^'(\\?.)'$");

    static public Regex RENumber10 = new(@"^\d+$");

    static public Regex RENumber2 = new(@"^0b([01]+)$");

    static public Regex RENumber16 = new(@"^0x([0-9A-Fa-f]+)$");

    static public Regex REEscape = new(@"\\(.)");

    static public Regex RELabelArg = new(@"^[_A-Za-z][_A-Za-z0-9]*$");

    static public string[] Registers = ["B", "C", "D", "E", "H", "L", "noway", "A"];

    static public string[] RegisterPairs = ["BC", "DE", "HL", "PSW"];

    private static readonly Dictionary<string, string> EscapeTable = new()
    {
        ["0"] = "\0",     // Null character
        ["a"] = "\a",     // Alert/beep
        ["b"] = "\b",     // Backspace
        ["f"] = "\f",     // Form feed
        ["n"] = "\n",     // New line
        ["r"] = "\r",     // Carriage return
        ["t"] = "\t",     // Tab
        ["v"] = "\v",     // Vertical tab
    };

    public List<string> Clean;

    public Parser(List<string> clean)
    {
        Clean = clean;
    }

    public List<Command> Process()
    {
        List<Command> result = [];

        foreach (var line in Clean)
        {
            var templine = line;
            Command command = new Command();

            // Метка
            var mo_label = RELabel.Match(templine);
            if (mo_label.Success)
            {
                templine = templine.Substring(mo_label.Length);
                command.Label = mo_label.Groups[1].Value;
            }

            // Команда
            bool found = false;
            var mo_name = REName.Match(templine);
            if (mo_name.Success)
            {
                var name = mo_name.Groups[1].Value.ToUpper();

                if (Enum.TryParse<Directive>(name, false, out var directive))
                {
                    command.Directive = directive;
                    found = true;
                    templine = templine.Substring(mo_name.Length);
                }

                if (Enum.TryParse<Instruction>(name, false, out var instruction))
                {
                    command.Instruction = instruction;
                    found = true;
                    templine = templine.Substring(mo_name.Length);
                }

                if (!found) throw new($"Unknown {name} instruction");
            }

            // Аргументы
            if (found)
            {
                List<string> rawargs = SplitArgs(templine);
                command.Args = ParseArgs(rawargs);
                result.Add(command);
            }
        }

        return result;
    }

    public List<Argument> ParseArgs(List<string> rawargs)
    {
        List<Argument> args = [];

        foreach (var rawarg in rawargs)
        {
            args.Add(ParseArg(rawarg));
        }

        return args;
    }

    public Argument ParseArg(string rawarg)
    {
        Argument arg = new();

        // Обращение к памяти
        var mo_memory = REMemory.Match(rawarg);
        if (mo_memory.Success)
        {
            rawarg = mo_memory.Groups[1].Value;
            arg.IsMemoryAccess = true;
        }

        // Строки
        var mo_string = REString.Match(rawarg);
        if (mo_string.Success)
        {
            if (arg.IsMemoryAccess) throw new("Memory access can't be string");
            var content = ResolveEscapes(mo_string.Groups[1].Value);
            arg.Values = Encoding.ASCII.GetBytes(content).ToList();
            arg.Type = ArgumentType.String;
        }

        // Символы
        var mo_char = REChar.Match(rawarg);
        if (mo_char.Success)
        {
            var content = ResolveEscapes(mo_char.Groups[1].Value);
            arg.Value = Encoding.ASCII.GetBytes(content)[0];
            arg.Type = ArgumentType.Literal;
        }

        // Числа
        var mo_number10 = RENumber10.Match(rawarg);
        if (mo_number10.Success)
        {
            arg.Value = Convert.ToInt32(mo_number10.Value);
            arg.Type = ArgumentType.Literal;
        }

        var mo_number2 = RENumber2.Match(rawarg);
        if (mo_number2.Success)
        {
            arg.Value = Convert.ToInt32(mo_number2.Value, 2);
            arg.Type = ArgumentType.Literal;
        }

        var mo_number16 = RENumber16.Match(rawarg);
        if (mo_number16.Success)
        {
            arg.Value = Convert.ToInt32(mo_number16.Value, 16);
            arg.Type = ArgumentType.Literal;
        }

        // Метки
        var mo_label = RELabelArg.Match(rawarg);
        if (mo_label.Success)
        {
            var upper = mo_label.Value.ToUpper();
            // Регистры
            if (Registers.Contains(upper))
            {
                arg.Value = Registers.IndexOf(upper);
                arg.Type = ArgumentType.Register;
            }
            // Регистровые пары
            else if (RegisterPairs.Contains(upper))
            {
                arg.Value = RegisterPairs.IndexOf(upper);
                arg.Type = ArgumentType.RegisterPair;
            }
            // Все-же метка
            else
            {
                arg.Label = mo_label.Value;
                arg.Type = ArgumentType.Label;
            }
        }

        if (arg.Type == ArgumentType.Unknown) throw new($"Invalid argument {rawarg}");

        return arg;
    }

    public string ResolveEscapes(string str)
    {
        return REEscape.Replace(str, mo => {
            var ch = mo.Groups[1].Value;

            if (EscapeTable.TryGetValue(ch, out string replacement)) return replacement;

            return ch;
        });
    }

    public List<string> SplitArgs(string templine)
    {
        List<string> args = [];
        string temp = "";
        int quoted = 0;

        // Делимитер
        for (int i = 0; i < templine.Length; i++)
        {
            var ch = templine[i];


            if (ch == '\'')
            {
                if (quoted == 0) quoted = 1;
                if (quoted == 1) quoted = 0;
            }

            if (ch == '"')
            {
                if (quoted == 0) quoted = 2;
                if (quoted == 2) quoted = 0;
            }

            if (quoted == 0 && ch == ' ') continue;

            if (quoted == 0 && ch == ',')
            {
                args.Add(temp);
                temp = "";
                continue;
            }

            temp += ch.ToString();
        }

        if (temp.Length > 0) args.Add(temp);

        return args;
    }
}