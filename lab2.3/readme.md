% Лабораторная работа № 2.3 «Синтаксический анализатор на основе
  предсказывающего анализа»
% 23 мая 2024 г.
% Алексей Митрошкин, ИУ9-31Б

# Цель работы
Целью данной работы является изучение алгоритма построения таблиц предсказывающего анализатора.


# Индивидуальный вариант
# объявления
```
non-terminal E, E1, T, T1, F;
terminal '+', '*', '(', ')', n;

# правила грамматики
E  ::= T E1;
E1 ::= '+' T E1 | epsilon;
T  ::= F T1;
T1 ::= '*' F T1 | epsilon;
F  ::= n | '(' E ')';

axiom E;
```
# Реализация

## Неформальное описание синтаксиса входного языка
…
В качестве входного языка выступает язык представления правил грамматики, 
лексика и синтаксис которого восстанавливаются из примера в индивидуальном варианте.

Отметим, что каждое определение программы  начинается с объявления используемых терменалов  
нетерменалов и продолжается объявлением правил грамматики, а заканчивается объявлением аксиомы.
```
program -> decl rules axiom .
```

Декларация пребавляет собой список нетерменалов и терменалов грамматики:
```
decl -> kw_nonterminal non_terms SC kw_terminal terms SC .
```

Правила грамматики выглядят как нетерменал, раскрывающийся в что-то список альтернатив правила:
```
rule -> non_term kw_eq alts SC .
alts -> alt alts | . 
```


## Лексическая структура

```
Term : \'[a-z+*()]\'
NonTerm : [A-Z0-9]
SC : ';'
Comma : ','
KW_EPS : 'epsilon'
KW_NONTERMINAL : 'non-terminal'
KW_TERMINAL : 'terminal'
KW_AXIOM : 'axiom'
KW_EQ : '::='
OR : '|'

```

## Грамматика языка
```
program -> decl rules axiom .

rules -> rule rules | .

decl -> kw_nonterminal non_terms SC kw_terminal terms SC .

axiom -> kw_axiom non_term SC  .

rule -> non_term kw_eq alts SC .

non_terms -> non_term non_term_tail .
non_term_tail -> comma non_terms | .
 
terms -> term term_tail .

term_tail -> comma terms | .

alts -> symbols alt_tail .
alt_tail -> OR alts | .

symbols -> non_term symbols | term symbols | kw_eps symbols | .
```
## Программная реализация

Файл Scanner.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Tokens;

namespace lab2._3.src.Lexer
{
    internal class Scanner
    {
        private Compiler compiler;
        private Position cur;
        public List<Fragment> Comments { get; private set; }

        public Scanner(string program, Compiler compiler)
        {
            this.compiler = compiler;
            cur = new Position(program);
            Comments = new List<Fragment>();
        }

        public Token NextToken()
        {
            while (cur.Cp != -1)
            {
                string word = "";

                while (cur.IsWhiteSpace)
                    cur++;
                var prev_cur = cur.clone();
                switch (cur.Cp)
                {
                    case '\'':
                        word += (char)cur.Cp;
                        cur++;
                        if (cur.isTerm)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if(cur.Cp != '\'' || cur.Cp == -1)
                        {
                            compiler.AddMessage(isErr: true, cur,
                             "term must end with ' symbol!");   
                            return new Erorr(DomainTag.ERROR,prev_cur,
                             cur.clone());    
                        }

                        word += (char)cur.Cp;
                        cur++;

                        return new TermToken(word,prev_cur,cur.clone());
                    case ';':
                        cur++;
                        return new SpecialToken(DomainTag.SC,";", prev_cur, cur.clone());
                    case '\n':
                        cur++;
                        return new SpecialToken(DomainTag.NL, "\\n", prev_cur, cur.clone());
                    case ',':
                        cur++;
                        return new SpecialToken(DomainTag.COMMA, ",", prev_cur, cur.clone());
                    case ':':
                        cur++;
                        cur++;
                        cur++;
                        return new KeyWordToken(DomainTag.KW_EQ, "::=", prev_cur, cur.clone());
                    case '|':
                        cur++;
                        return new SpecialToken(DomainTag.OR, "|", prev_cur, cur.clone());
                    case '#':
                        cur++;
                      
                        while (!cur.IsNewLine)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        Comments.Add(new Fragment(prev_cur, cur.clone()));
                        break;                        
                    default:
       
             
                        if (cur.Cp != -1)
                        {

                            if (cur.isNonTerm)
                            {
                                word += (char)cur.Cp;
                                cur++;
                                while (cur.IsDecimalDigit || cur.isNonTerm)
                                {
                                    word += (char)cur.Cp;
                                    cur++;
                                }

                                return new NonTerm(word, prev_cur, cur.clone());

                            }
                            else
                            {
                                if (cur.IsLetter)
                                {
                                    word += (char)cur.Cp;

                                    cur++;
                                    while (cur.IsLetter || cur.Cp == '-')
                                    {
                                        word += (char)cur.Cp;
                                        cur++;
                                    }

                                    switch (word)
                                    {
                                        case "terminal":
                                            return new KeyWordToken(DomainTag.KW_TERMINAL,
                                             word, prev_cur, cur.clone());
                                        case "non-terminal":
                                            return new KeyWordToken(DomainTag.KW_NONTERMINAL,
                                             word, prev_cur, cur.clone());
                                        case "axiom":
                                            return new KeyWordToken(DomainTag.KW_AXIOM,
                                             word, prev_cur, cur.clone());
                                        case "epsilon":
                                            return new KeyWordToken(DomainTag.KW_EPS,
                                             word, prev_cur, cur.clone());

                                    }
                                }


                            }
                        }
                        break;
                }
                cur++;
            }

            return new EOFToken("", cur, cur);
        }


    }
}

```

Файл Parser.cs
```csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src
{
    internal class Parser
    {
        Dictionary<string, string[]> table = new Dictionary<string, string[]>()
        {
            {"program kw_nonterminal", new string[] {"decl","rules","axiom" } },

            {"rules kw_axiom", new string[] {} },
            {"rules non_term", new string[] {"rule","rules" } },

            {"decl kw_nonterminal", new string[] { "kw_nonterminal", "non_terms", "sc",
             "kw_terminal","terms","sc" } },

            {"axiom kw_axiom", new string[] { "kw_axiom", "non_term", "sc" } },

            {"rule non_term", new string[] { "non_term", "kw_eq","alts","sc" } },

            {"non_terms non_term", new string[] { "non_term", "non_term_tail" } },

            {"non_term_tail sc", new string[] { } },
            {"non_term_tail comma", new string[] {"comma","non_terms" } },

            {"terms term", new string[] { "term", "term_tail" } },

            {"term_tail sc", new string[] { } },
            {"term_tail comma", new string[] {"comma","terms" } },

            {"alts non_term", new string[] {"symbols","alt_tail" } },
            {"alts term", new string[] {"symbols","alt_tail" } },
            {"alts kw_eps", new string[] {"symbols","alt_tail" } },

            {"alt_tail or", new string[] {"or","alts" } },

            {"alt_tail sc", new string[] {} },

            {"symbols non_term", new string[] {"non_term", "symbols"} },
            {"symbols term", new string[] {"term", "symbols"} },
            {"symbols kw_eps", new string[] { "kw_eps", "symbols"} },
            {"symbols sc", new string[] { } },
            {"symbols or", new string[] { } }
        };

        bool isTerminal(string str)
        {
            return !(str == "program" || str == "rules" || str == "decl" || str =="axiom" ||
                str == "rule" || str == "non_terms" || str == "non_term_tail" ||
                str == "terms" || str =="term_tail" || str =="alts" || str =="alt_tail"
                ||str =="symbols");
        }

        public INode parse(Scanner sc)
        {

            var stack = new Stack<StackNode>();

            InnerNode start = new InnerNode("");

            stack.Push(new StackNode() { value = "program", node = start });


            Token tok = sc.NextToken();

            while (tok.Tag != DomainTag.EOF && stack.Count != 0)
            {
                var top = stack.Pop();

                var lexer_tag = tok.Tag.ToString().ToLower();
                if (isTerminal(top.value))
                {
                    top.node.AddChild(new Leaf(tok));

                    tok = sc.NextToken();

                }
                else if (table.Keys.Contains($"{top.value} {tok.Tag.ToString().ToLower()}"))
                {
                    var inner = new InnerNode(top.value);
                    top.node.AddChild(inner);

                    var go = table[$"{top.value} {tok.Tag.ToString().ToLower()}"];



                    for (int i = go.Length - 1; i >= 0; i--)
                    {
                        stack.Push(new StackNode() { value = go[i], node = inner });
                    }



                }
                else
                {
                    throw new Exception($"Invalid Tokens {top.value} {lexer_tag}");
                }
            }

            return start;
        }
    }

}

```

Файл Node.cs
```csharp
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src
{
    public interface INode
    {

        void Print(string indent);
    }

    struct StackNode { 
       public InnerNode node;
       public string value;


    }

    class InnerNode : INode
    {
        private string nterm;
        private List<INode> children;

        public InnerNode(string nterm)
        {
            this.nterm = nterm;
            children = new List<INode>();
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent} Внутренний узел: {nterm}");
            foreach (var child in children)
            {
                child.Print(indent + "\t");
            }
        }

        public void AddChild(INode child)
        {
            children.Add(child);
        }
    }

    class Leaf : INode
    {
        private Token tok;

        public Leaf(Token tok)
        {
            this.tok = tok;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}Лист:  {tok}");

        }
    }

}


```

Файл Compiler.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Lexer
{
    internal class Compiler
    {
        private SortedList<Position, Message> messages;
        private Dictionary<string, int> nameCodes;
        private List<string> names;

        public Compiler()
        {
            messages = new SortedList<Position, Message>();
            nameCodes = new Dictionary<string, int>();
            names = new List<string>();

        }

        public int AddName(string name)
        {
            if (nameCodes.ContainsKey(name))
                return nameCodes[name];

            int code = names.Count;
            names.Add(name);
            nameCodes[name] = code;
            return code;

        }
        public string GetName(int code)
        {
            return names[code];
        }

        public void AddMessage(bool isErr, Position c, string text)
        {
            messages[c] = new Message(isErr, text);
        }

        public void OutPutMessages()
        {
            foreach (var p in messages)
            {
                Console.Write(p.Value.IsErorr ? "Erorr " : "Warning ");
                Console.WriteLine($"{p.Key} {p.Value.Text}");

            }
        }
        public Scanner GetScaner(string program)
        {
            return new Scanner(program, this);
        }
    }
}

```

Файл Fragment.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Lexer
{
    internal class Fragment
    {
        public readonly Position Starting, Following;

        public Fragment(Position starting, Position following)
        {
            Starting = starting;
            Following = following;
        }
        public override string ToString()
        {
            return $"{Starting}-{Following}";
        }
    }
}

```


Файл Message.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Lexer
{
    internal class Message
    {
        public readonly bool IsErorr;
        public readonly string Text;


        public Message(bool isErorr, string text)
        {
            IsErorr = isErorr;
            Text = text;
        }
    }
}

```

Файл Position.cs
```csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Lexer
{
    internal class Position : IComparable<Position>
    {
        public string Text { get; private set; }
        public int Index { get; private set; }
        public int Pos { get; private set; }
        public int Line { get; private set; }

        public int Cp
        {
            get
            {
                return Index == Text.Length ? -1 : char.ConvertToUtf32(Text, Index);
            }
            private set { }
        }

        public UnicodeCategory Uc
        {
            get
            {
                return Index == Text.Length ? UnicodeCategory.OtherNotAssigned
                 : char.GetUnicodeCategory(Text, Index);
            }
            private set { }
        }
        public bool IsWhiteSpace
        {
            get
            {
                return Index != Text.Length && char.IsWhiteSpace(Text, Index);
            }
            private set { }
        }
        public bool IsLetter
        {
            get
            {
                return Index != Text.Length && char.IsLetter(Text, Index);
            }
            private set { }
        }
        public bool IsLetterOrDigit
        {
            get
            {
                return Index != Text.Length && char.IsLetterOrDigit(Text, Index);
            }
            private set { }
        }
        public bool IsDecimalDigit
        {
            get
            {
                return Index != Text.Length && Text[Index] >= '0' && Text[Index] <= '9';
            }
            private set { }
        }
        public bool isTerm
        {
            get
            {
                return (char.IsLower(Text, Index) && char.IsLetter(Text, Index)) || 
                    Cp == '(' || 
                    Cp == ')' || 
                    Cp == '+' || 
                    Cp == '*';
            }
        }

        public bool isNonTerm
        {
            get
            {
                return (char.IsUpper(Text, Index) && char.IsLetter(Text, Index));
            }
        }
        public bool IsNewLine
        {
            get
            {
                if (Index == Text.Length)
                    return true;

                if (Text[Index] == '\r' && Index + 1 < Text.Length)
                    return Text[Index + 1] == '\n';

                return Text[Index] == '\n';
            }
            private set { }
        }

        public Position(string text)
        {
            Text = text;
            Line = Pos = 1;
            Index = 0;
        }

        public int CompareTo(Position? other)
        {
            return Index.CompareTo(other?.Index);
        }
      
        public override string ToString()
        {
            return $"({Line},{Pos})";
        }

        public static Position operator ++(Position p)
        {
            if (p.Index < p.Text.Length)
            {
                if (p.IsNewLine)
                {
                    if (p.Text[p.Index] == '\r')
                        p.Index++;
                    p.Line++;
                    p.Pos = 1;
                }
                else
                {
                    if (char.IsHighSurrogate(p.Text[p.Index]))
                        p.Index++;
                    p.Pos++;


                }
                p.Index++;
            }
            return p;
        }

        public Position clone()
        {
            var pp = new Position(Text);
            pp.Line = Line;
            pp.Pos = Pos;
            pp.Index = Index;

            return pp;
        }
    }
}

```

Файл Token.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    enum DomainTag
    {
        TERM,
        NON_TERM,
        KW_AXIOM,
        KW_EPS,
        KW_NONTERMINAL,
        KW_TERMINAL,
        KW_EQ,
        NL,
        ERROR,
        SC,
        OR,
        COMMA,
        COMMENT,
        EOF
    }

   

    internal abstract class Token
    {
        public readonly DomainTag Tag;
        public readonly Fragment Coords;

        protected Token(DomainTag tag, Position statring, Position following)
        {
            Tag = tag;
            Coords = new Fragment(statring, following);
        }


    }
}

```

Файл TermToken.cs
```csharp
using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class TermToken : Token
    {
        public readonly string term;
        public TermToken(string term, Position statring, Position following)
         : base(DomainTag.TERM, statring, following)
        {
            this.term = term;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {term}";
        }
    }
}

```

Файл NonTermToken.cs
```csharp
using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class NonTerm : Token
    {
        public readonly string nterm;
        public NonTerm(string nterm, Position statring, Position following) 
        : base(DomainTag.NON_TERM, statring, following)
        {
            this.nterm = nterm;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {nterm}";
        }
    }
}

```


Файл SpecialToken.cs
```csharp
using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class SpecialToken : Token
    {
        public readonly string sym;

        public SpecialToken(DomainTag tag, string val, Position starting, Position following)
         : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.SC || tag == DomainTag.NL ||
             tag == DomainTag.COMMA || tag == DomainTag.OR);
            sym = val;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {sym}";
        }
    }
}

```

Файл KeyWordToken.cs
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    internal class KeyWordToken : Token
    {
        public readonly string keyword;

        public KeyWordToken(DomainTag tag, string value, Position starting, Position following)
         : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.KW_NONTERMINAL || tag == DomainTag.KW_EPS
             || tag == DomainTag.KW_AXIOM || tag == DomainTag.KW_TERMINAL||tag == DomainTag.KW_EQ);
            keyword = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {keyword}";
        }
    }
}

```


Файл Program.cs
```csharp
using lab2._3.src;
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;

namespace lab2._3
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var lines = File.ReadAllLines("input.txt");
            string prg = "";

            foreach (var line in lines)
            {
                prg += line + '\n';
            }

            var cp = new Compiler();
            var sc = new Scanner(prg, cp);

            var tok = sc.NextToken();
            while (tok.Tag != DomainTag.EOF)
            {
                var st = tok.ToString();

                Console.WriteLine(st);
                tok = sc.NextToken();

                if (tok.Tag == DomainTag.EOF)
                {
                    Console.WriteLine(tok.ToString());
                }

            }
            Console.WriteLine();
            cp.OutPutMessages();
            
            cp = new Compiler();
            sc = new Scanner(prg, cp);

            var parser = new Parser();

            var tree = parser.parse(sc);
            tree.Print("");


        }
    }
}
```


# Тестирование

Входные данные

```
non-terminal E, E1, T, T1, F;
terminal '+', '*', '(', ')', 'n';

E  ::= T E1;
E1 ::= '+' T E1 | epsilon;
T  ::= F T1;
T1 ::= '*' F T1 | epsilon;
F  ::= 'n' | '(' E ')';

axiom E;
```

Вывод на `stdout`
<!-- ENABLE LONG LINES -->
```
 Внутренний узел:
         Внутренний узел: program
                 Внутренний узел: decl
                        Лист:  KW_NONTERMINAL (1,1)-(1,13) non-terminal
                         Внутренний узел: non_terms
                                Лист:  NON_TERM (1,14)-(1,15) E
                                 Внутренний узел: non_term_tail
                                        Лист:  COMMA (1,15)-(1,16) ,
                                         Внутренний узел: non_terms
                                                Лист:  NON_TERM (1,17)-(1,19) E1
                                                 Внутренний узел: non_term_tail
                                                        Лист:  COMMA (1,19)-(1,20) ,
                                                         Внутренний узел: non_terms
                                                                Лист:  NON_TERM (1,21)-(1,22) T
                                                                 Внутренний узел: non_term_tail
                                                                        Лист:  COMMA (1,22)-(1,23) ,
                                                                         Внутренний узел: non_terms
                                                                                Лист:  NON_TERM (1,24)-(1,26) T1
                                                                                 Внутренний узел: non_term_tail
                                                                                        Лист:  COMMA (1,26)-(1,27) ,
                                                                                         Внутренний узел: non_terms
                                                                                                Лист:  NON_TERM (1,28)-(1,29) F
                                                                                                 Внутренний узел: non_term_tail
                        Лист:  SC (1,29)-(1,30) ;
                        Лист:  KW_TERMINAL (2,1)-(2,9) terminal
                         Внутренний узел: terms
                                Лист:  TERM (2,10)-(2,13) '+'
                                 Внутренний узел: term_tail
                                        Лист:  COMMA (2,13)-(2,14) ,
                                         Внутренний узел: terms
                                                Лист:  TERM (2,15)-(2,18) '*'
                                                 Внутренний узел: term_tail
                                                        Лист:  COMMA (2,18)-(2,19) ,
                                                         Внутренний узел: terms
                                                                Лист:  TERM (2,20)-(2,23) '('
                                                                 Внутренний узел: term_tail
                                                                        Лист:  COMMA (2,23)-(2,24) ,
                                                                         Внутренний узел: terms
                                                                                Лист:  TERM (2,25)-(2,28) ')'
                                                                                 Внутренний узел: term_tail
                                                                                        Лист:  COMMA (2,28)-(2,29) ,
                                                                                         Внутренний узел: terms
                                                                                                Лист:  TERM (2,30)-(2,33) 'n'
                                                                                                 Внутренний узел: term_tail
                        Лист:  SC (2,33)-(2,34) ;
                 Внутренний узел: rules
                         Внутренний узел: rule
                                Лист:  NON_TERM (4,1)-(4,2) E
                                Лист:  KW_EQ (4,4)-(4,7) ::=
                                 Внутренний узел: alts
                                         Внутренний узел: symbols
                                                Лист:  NON_TERM (4,8)-(4,9) T
                                                 Внутренний узел: symbols
                                                        Лист:  NON_TERM (4,10)-(4,12) E1
                                                         Внутренний узел: symbols
                                         Внутренний узел: alt_tail
                                Лист:  SC (4,12)-(4,13) ;
                         Внутренний узел: rules
                                 Внутренний узел: rule
                                        Лист:  NON_TERM (5,1)-(5,3) E1
                                        Лист:  KW_EQ (5,4)-(5,7) ::=
                                         Внутренний узел: alts
                                                 Внутренний узел: symbols
                                                        Лист:  TERM (5,8)-(5,11) '+'
                                                         Внутренний узел: symbols
                                                                Лист:  NON_TERM (5,12)-(5,13) T
                                                                 Внутренний узел: symbols
                                                                        Лист:  NON_TERM (5,14)-(5,16) E1
                                                                         Внутренний узел: symbols
                                                 Внутренний узел: alt_tail
                                                        Лист:  OR (5,17)-(5,18) |
                                                         Внутренний узел: alts
                                                                 Внутренний узел: symbols
                                                                        Лист:  KW_EPS (5,19)-(5,26) epsilon
                                                                         Внутренний узел: symbols
                                                                 Внутренний узел: alt_tail
                                        Лист:  SC (5,26)-(5,27) ;
                                 Внутренний узел: rules
                                         Внутренний узел: rule
                                                Лист:  NON_TERM (6,1)-(6,2) T
                                                Лист:  KW_EQ (6,4)-(6,7) ::=
                                                 Внутренний узел: alts
                                                         Внутренний узел: symbols
                                                                Лист:  NON_TERM (6,8)-(6,9) F
                                                                 Внутренний узел: symbols
                                                                        Лист:  NON_TERM (6,10)-(6,12) T1
                                                                         Внутренний узел: symbols
                                                         Внутренний узел: alt_tail
                                                Лист:  SC (6,12)-(6,13) ;
                                         Внутренний узел: rules
                                                 Внутренний узел: rule
                                                        Лист:  NON_TERM (7,1)-(7,3) T1
                                                        Лист:  KW_EQ (7,4)-(7,7) ::=
                                                         Внутренний узел: alts
                                                                 Внутренний узел: symbols
                                                                        Лист:  TERM (7,8)-(7,11) '*'
                                                                         Внутренний узел: symbols
                                                                                Лист:  NON_TERM (7,12)-(7,13) F
                                                                                 Внутренний узел: symbols
                                                                                        Лист:  NON_TERM (7,14)-(7,16) T1                                                                                         Внутренний узел: symbols
                                                                 Внутренний узел: alt_tail
                                                                        Лист:  OR (7,17)-(7,18) |
                                                                         Внутренний узел: alts
                                                                                 Внутренний узел: symbols
                                                                                        Лист:  KW_EPS (7,19)-(7,26) epsilon
                                                                                         Внутренний узел: symbols
                                                                                 Внутренний узел: alt_tail
                                                        Лист:  SC (7,26)-(7,27) ;
                                                 Внутренний узел: rules
                                                         Внутренний узел: rule
                                                                Лист:  NON_TERM (8,1)-(8,2) F
                                                                Лист:  KW_EQ (8,4)-(8,7) ::=
                                                                 Внутренний узел: alts
                                                                         Внутренний узел: symbols
                                                                                Лист:  TERM (8,8)-(8,11) 'n'
                                                                                 Внутренний узел: symbols
                                                                         Внутренний узел: alt_tail
                                                                                Лист:  OR (8,12)-(8,13) |
                                                                                 Внутренний узел: alts
                                                                                         Внутренний узел: symbols
                                                                                                Лист:  TERM (8,14)-(8,17) '('
                                                                                                 Внутренний узел: symbols
                                                                                                        Лист:  NON_TERM (8,18)-(8,19) E
                                                                                                         Внутренний узел: symbols
                                                                                                                Лист:  TERM (8,20)-(8,23) ')'
                                                                                                                 Внутренний узел: symbols
                                                                                         Внутренний узел: alt_tail
                                                                Лист:  SC (8,23)-(8,24) ;
                                                         Внутренний узел: rules
                 Внутренний узел: axiom
                        Лист:  KW_AXIOM (10,1)-(10,6) axiom
                        Лист:  NON_TERM (10,7)-(10,8) E
                        Лист:  SC (10,8)-(10,9) ;

```

# Вывод
Вспомнил, что такое LL парсер и управляющая таблица, научился разрабатывать
синтаксический анализатор на основе предсказывающего анализа.