using System.Text.RegularExpressions;

namespace Retro.Compiler.old;

class Lexer
{
    public string Source;

    public int Position;

    public bool EOF { get => Position >= Source.Length; }

    public List<TokenNotation> Notations = [
        new("operator", new(@"\+|-|\*|\\")),
        new("number", new(@"\d+")),
        new("whitespace", new(@"\s+")),
    ];

    public Lexer(string source)
    {
        Source = source;
    }

    public Token Next()
    {
        if (Position == Source.Length) return new("eof", "");

        foreach (var notation in Notations)
        {
            var mo = notation.RE.Match(Source, Position);
            if (mo != null)
            {
                if (mo.Index == Position)
                {
                    Position += mo.Length;
                    return new(notation.Type, mo.Value);
                }
            }
        }

        return new("unvalid", Source[Position].ToString());
    }
}

class TokenNotation(string type, Regex re)
{
    public string Type = type;

    public Regex RE = re;
}

class Token(string type, string source)
{
    public string Source = source;

    public string Type = type;

    public override string ToString()
    {
        return $"{Type}({Source})";
    }
}
