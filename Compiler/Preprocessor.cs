using System.Text.RegularExpressions;

namespace Retro.Compiler;

public class Preprocessor
{
    public static Regex REComment = new(@";.*$");

    public string[] Source;

    public Preprocessor(string[] source)
    {
        Source = source;
    }

    public List<string> Process()
    {
        List<string> result = [];

        foreach (var line in Source)
        {
            var temp = REComment.Replace(line, "").Trim();
            if (temp.Length == 0) continue;
            result.Add(temp);
        }

        return result;
    }
}
