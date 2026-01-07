using System.Collections.Generic;
using System.Data;

namespace Retro.Compiler.old;


class Parser
{
    private Rule[] Rules = [
        new ("Expression", ["BinaryOp"]),
        new ("Expression", ["Atom"]),

        new ("BinaryOp",   ["Expression", "operator", "Expression"]),

        new ("Atom",       ["number"]),
        ];

    private Lexer _lexer;

    private List<AstBase> _stack;

    public Parser(SourceCode source)
    {
        _lexer = new(source.Content);
        _stack = new List<AstBase>();
    }

    // Получить новый элемент от лексера
    public AstToken Shift()
    {
        while (true)
        {
            var token = _lexer.Next();
            if (token.Type == "whitespace") continue;
            var node = new AstToken(token);
            _stack.Add(node);
            return node;
        }
    }

    // Сократить стек
    public AstBase? Reduce()
    {
        AstBase? node = null;

        // Обход списка правил
        foreach (var rule in Rules)
        {
            // Проверка
            if (rule.Notation.Length > _stack.Count) continue;
            var slice = _stack.GetRange(_stack.Count - rule.Notation.Length, rule.Notation.Length);
            if (!rule.Match(slice)) continue;

            // Сокращение
            var subnodes = new AstBase[rule.Notation.Length];
            for (var i = 0; i < rule.Notation.Length; i++) subnodes[i] = slice[i];
            _stack.RemoveRange(_stack.Count - rule.Notation.Length, rule.Notation.Length);
            node = new AstNode(rule.Type, subnodes);
            _stack.Add(node);
            break;
        }

        return node;
    }

    // Один шаг парсера
    public AstBase Step()
    {
        AstBase node = Shift();

        while (true)
        {
            var reduced = Reduce();
            if (reduced == null) break;
            node = reduced;
        }

        return node;
    }

    // Полный парсинг
    public AstBase Parse()
    {
        _stack.Clear();

        while (true)
        {
            var node = Step();
            if (node.Type == "eof") break;
            if (node.Type == "unexpected") throw new("Unexpected symbol");
        }
        if (_stack.Count > 2) throw new("Uncompleted");

        return _stack[0];
    }
}

class Rule(string type, string[] notation)
{
    public string Type = type;
    public string[] Notation = notation;


    // Выявляет совпадение с правилом
    public bool Match(List<AstBase> sequence)
    {
        int last = 0;

        for (var i = 0; i < Notation.Length; i++)
        {
            var type = Notation[i];
            var item = sequence[i];

            if (type == item.Type) last++;
            else break;
        }

        return last == Notation.Length;
    }
}

abstract class AstBase(string type)
{
    public string Type = type;

    public abstract string ValueToString();

    public override string ToString()
    {
        return $"{Type}{{{ValueToString()}}}";
    }
}

class AstToken(Token token) : AstBase(token.Type)
{
    public Token Token = token;

    public override string ValueToString()
    {
        return Token.ToString();
    }
}

class AstNode(string type, AstBase[] subnodes) : AstBase(type)
{
    public AstBase[] Subnodes = subnodes;

    public override string ValueToString()
    {
        string str = "";
        foreach (var node in Subnodes) str += node.ToString() + ", ";
        if (str.Length > 0) str = str.Substring(0, str.Length-2);
        return str;
    }
}