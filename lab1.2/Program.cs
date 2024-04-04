using System.Text.RegularExpressions;

public enum TokenType
{
    NUMBER,
    OPERATOR,
    COMMENT,
    INVALID
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public override string ToString()
    {
        return $"{Type} ({Line}, {Column}): {Value}";
    }
}

public class Lexer
{
    private readonly StreamReader _reader;
    private int _line = 1;
    private int _column = 1;

    private static readonly Regex NumberRegex = new Regex(@"\b\d+\b");
    private static readonly Regex OperatorRegex = new Regex(@"[\+\-\*/\(\)]");
    private static readonly Regex CommentRegex = new Regex(@"\(\*\(.|\n\)*?\*\)");

    public Lexer(string filename)
    {
        _reader = new StreamReader(filename);
    }

    public IEnumerable<Token> Tokenize()
    {
        while (!_reader.EndOfStream)
        {
            var line = _reader.ReadLine();
            var matches = CommentRegex.Matches(line);
            foreach (Match match in matches)
            {
                yield return new Token
                {
                    Type = TokenType.COMMENT,
                    Value = match.Value,
                    Line = _line,
                    Column = line.IndexOf(match.Value) + 1
                };
            }

            var cleanedLine = CommentRegex.Replace(line, " ");
           
            var tokens = TokenizeLine(cleanedLine);
            foreach (var token in tokens)
            {
                token.Line = _line;
                yield return token;
            }

            _line++;
        }
        _reader.Close();
    }

    private IEnumerable<Token> TokenizeLine(string line)
    {
        var matches = Regex.Matches(line, @"(\b\d+\b|[\+\-\*/\(\)]|[aA-zZ])");

        

        if (matches.Count == 0)
        {   
            if (line != "")
                Console.WriteLine($"syntax error ({_line},{_column})");
        }

        foreach (Match match in matches)
        {   
            var value = match.Value;
            
            TokenType type;
            if (NumberRegex.IsMatch(value))
            {
                type = TokenType.NUMBER;
            }
            else if (OperatorRegex.IsMatch(value))
            {
                type = TokenType.OPERATOR;
            }
            else
            {
                 type = TokenType.INVALID;
                 //Console.WriteLine($"syntax error ({_line},{_column})");
            }
            yield return new Token
            {
                Type = type,
                Value = value,
                Column = match.Index + 1
            };

        }

        
    }
}

class Program
{
    static void Main(string[] args)
    {
        var lexer = new Lexer("input.txt");
        foreach (var token in lexer.Tokenize())
        {
            if(token.Type != TokenType.INVALID)
            {
                Console.WriteLine(token);
            }
            else
            {
                Console.WriteLine($"syntax at ({token.Line},{token.Column})");
            }
        }
    }
}
