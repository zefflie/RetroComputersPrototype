namespace Retro.Compiler.old;

class SourceCode
{
    public string Filename;

    public string Content;

    public SourceCode(string filename)
    {
        Filename = filename;
        Content = File.ReadAllText(filename);
    }
}

