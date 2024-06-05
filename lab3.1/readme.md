% Лабораторная работа № 3.1 «Самоприменимый генератор компиляторов
  на основе предсказывающего анализа»
% 27 мая 2024 г.
% Алексей Митрошкин, ИУ9-61Б

# Цель работы
Целью данной работы является изучение алгоритма построения таблиц предсказывающего анализатора.


# Индивидуальный вариант
```
# объявления
non-terminal E, E1, T, T1, F;
terminal '+', '*', '(', ')', 'n';

# правила грамматики
E  ::= T E1;
E1 ::= '+' T E1 | epsilon;
T  ::= F T1;
T1 ::= '*' F T1 | epsilon;
F  ::= 'n' | '(' E ')';

axiom E;
```
# Реализация

Лексическая структура
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

Грамматика языка:
```
# объявления
non-terminal PROGRAM, DECL, RULES, AXIOM, RULE, NON_TERMS,NON_TERM_TAIL,TERMS,TERM_TAIL,ALTS,ALT_TAIL,SYMBOLS;
terminal 'kw_nonterminal', 'kw_terminal','kw_eq','kw_eps','kw_axiom','sc','comma','non_term', 'term','or';

# правила грамматики
PROGRAM ::= DECL RULES AXIOM;
RULES ::= RULE RULES | epsilon;
DECL ::= 'kw_nonterminal' NON_TERMS 'sc' 'kw_terminal' TERMS 'sc';
RULE ::= 'non_term' 'kw_eq' ALTS 'sc';
AXIOM ::= 'kw_axiom' 'non_term' 'sc';

NON_TERMS ::= 'non_term' NON_TERM_TAIL;
NON_TERM_TAIL ::= 'comma' NON_TERMS | epsilon;

TERMS ::= 'term' TERM_TAIL;
TERM_TAIL ::= 'comma' TERMS | epsilon;

ALTS ::= SYMBOLS ALT_TAIL;
ALT_TAIL ::= 'or' ALTS | epsilon;

SYMBOLS ::= 'non_term' SYMBOLS | 'term' SYMBOLS | 'kw_eps' SYMBOLS | epsilon ;

#аксиома грамматики
axiom PROGRAM;
```

# Программная реализация

Программа, порождающая таблицу разбора
Файл Program.cs
``` csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using lab2._3.src;

using lab3._1.src.Gram;
using lab3._1.src.Parser;

namespace lab3._1
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

               // Console.WriteLine(st);
                tok = sc.NextToken();

                if (tok.Tag == DomainTag.EOF)
                {
                  //  Console.WriteLine(tok.ToString());
                }

            }

            cp = new Compiler();
            sc = new Scanner(prg, cp);

            var parser = new Parser();

            var tree = parser.parse(sc);

            tree.Print("");

            Grammar g = new Grammar(tree);
           // g.Print();

            Console.WriteLine();
        
            var parsingTable = new ParsingTable(g);

         //   parsingTable.SaveToFile();
           
            /* */

        }
    }
}
```


Файл Exeptions.cs
``` csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Exeptions
{
    public class TooManyAxiomException : Exception
    {
        static string _message = "Указано более одной аксиомы грамматики!";
        public TooManyAxiomException() : base(_message) { }
    }

    public class InvalidNonTermLenght : Exception
    {
        static string _message = "Накорректная длина: ";

        public InvalidNonTermLenght(string nt) : base(_message + nt ) { }
    }

    public class InvalidTree : Exception
    {
        static string _message = "Некорректное дерево, в ноде: ";
        public InvalidTree(string nt) : base(_message + nt) { }

    }


    public class NoAxiomException : Exception
    {
        static string _message = "Отсутсвует аксиома грамматики!";

        public NoAxiomException() : base(_message) { }
    }

    public class RepetedNonTerminalException : Exception
    {
        static string _message = "Повторный нетерменал грамматики: ";

        public RepetedNonTerminalException(string nt) : base(_message + nt) { }
    }

    public class RepetedTerminalException : Exception
    {
        static string _message = "Повторный терменал грамматики: ";

        public RepetedTerminalException(string t) : base(_message + t) { }
    }

    public class NoSuchNonTerminalException : Exception
    {
        static string _message = "Нетерменал не был объявлен: ";

        public NoSuchNonTerminalException(string nt) : base(_message + nt) { }
    }

    public class NoSuchTerminalException : Exception
    {
        static string _message = "Терменал не был объявлен: ";

        public NoSuchTerminalException(string t) : base(_message + t) { }
    }

    public class GrammarNotLLException : Exception
    {
        static string _message = "Грамматика не относится на классу LL(1)!";

        public GrammarNotLLException() : base(_message) { }
    }
}

```

Файл Grammar.cs
``` csharp
using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Gram
{
    internal class Grammar
    {
        public HashSet<string> Terms { get; private set; } = new HashSet<string>();
        public HashSet<string> NonTerms { get; private set; } = new HashSet<string>();
        public Dictionary<string, List<List<string>>> Rules { get; private set; } = new Dictionary<string, List<List<string>>>();
        public Axiom axiom { get; private set; } = new Axiom(string.Empty);

        public Dictionary<string, HashSet<string>> FollowSets = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> FirstSets = new Dictionary<string, HashSet<string>>();

        public static readonly string EPS = "#";

        private string CurrentNonTerm { get; set; } = string.Empty;
        private int CurrentListIndex
        {
            get
            {
                if (!Rules.ContainsKey(CurrentNonTerm))
                    return -1;

                return Rules[CurrentNonTerm].Count - 1;

            }
        }

        public Grammar(INode tree)
        {

            void Traverse(INode tree)
            {
                if (tree is not InnerNode)
                    return;

                var node = (InnerNode)tree;

                switch (node.nterm)
                {

                    case "program":
                        if (node.children.Count != 3)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;

                    case "rules":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;

                    case "decl":
                        if (node.children.Count != 6)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;
                    case "axiom":
                        if (node.children.Count != 3)
                            throw new InvalidNonTermLenght(node.nterm);
                        if (node.children[1] is not Leaf)
                            throw new InvalidTree(node.nterm);
                        if (!axiom.isEmpty)
                            throw new TooManyAxiomException();


                        var nt = ((NonTerm)((Leaf)node.children[1]).tok).nterm;

                        if (!NonTerms.Contains(nt))
                            throw new NoSuchNonTerminalException(nt);

                        axiom = new Axiom(nt);

                        node.children.ForEach(Traverse);


                        break;

                    case "rule":
                        if (node.children.Count != 4)
                            throw new InvalidNonTermLenght(node.nterm);

                        var rule_left = ((NonTerm)((Leaf)node.children[0]).tok).nterm;

                        if (!NonTerms.Contains(rule_left))
                            throw new NoSuchNonTerminalException(rule_left);


                        CurrentNonTerm = rule_left;

                        Rules[rule_left] = new List<List<string>>
                        {
                            new List<string>()
                        };

                        node.children.ForEach(Traverse);

                        break;

                    case "non_terms":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                        {
                            var new_nt = ((NonTerm)((Leaf)node.children[0]).tok).nterm;
                            NonTerms.Add(new_nt);
                        }


                        node.children.ForEach(Traverse);

                        break;

                    case "non_term_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                            node.children.ForEach(Traverse);

                        break;

                    case "terms":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        var new_term = ((TermToken)((Leaf)node.children[0]).tok).term.Replace("\'", "");
                        Terms.Add(new_term);

                        node.children.ForEach(Traverse);

                        break;

                    case "term_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                            node.children.ForEach(Traverse);

                        break;

                    case "alts":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);
                        node.children.ForEach(Traverse);

                        break;

                    case "alt_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                        {
                            Rules[CurrentNonTerm].Add(new List<string>());
                            node.children.ForEach(Traverse);
                        }
                        break;

                    case "symbols":

                        if (node.children.Count == 2)
                        {
                            var leaf_value = ((Leaf)node.children[0]).tok;
                            string leaf = string.Empty;

                            if (leaf_value is NonTerm)
                            {
                                leaf = ((NonTerm)leaf_value).nterm;
                                if (!NonTerms.Contains(leaf))
                                    throw new NoSuchNonTerminalException(leaf);
                            }
                            else if (leaf_value is TermToken)
                            {
                                leaf = ((TermToken)leaf_value).term.Replace("\'", "");
                                if (!Terms.Contains(leaf))
                                    throw new NoSuchTerminalException(leaf);
                            }
                            else if (leaf_value is KeyWordToken)
                            {
                                leaf = EPS;
                            }


                            Rules[CurrentNonTerm][CurrentListIndex].Add(leaf);
                            node.children.ForEach(Traverse);
                        }
                        else if (node.children.Count != 0)
                        {
                            throw new InvalidNonTermLenght(node.nterm);

                        }

                        break;

                    default:
                        node.children.ForEach(Traverse);

                        break;
                }


            }

            Traverse(tree);


            FirstSets = CalculateFirstSets();


            ComputeFollowSets();

           
        }

        public void Print()
        {
            Console.WriteLine($"Аксиома грамматики: {axiom.ToString()}");
            Console.WriteLine("Правила грамматики:");
            foreach (var nt in Rules.Keys)
            {
                var rule = new StringBuilder(nt);
                rule.Append(" -> ");
                foreach (var alt in Rules[nt])
                {
                    foreach (var symbol in alt)
                    {
                        rule.Append(symbol.ToString());
                        rule.Append(" ");
                    }
                    if (Rules[nt].Count > 1 && Rules[nt].IndexOf(alt) != Rules[nt].Count - 1)
                    {
                        rule.Append("| ");
                    }
                }
                Console.WriteLine(rule.ToString());
            }


        }
        public Dictionary<string, HashSet<string>> CalculateFirstSets()
        {
            var firstByNonTerm = NonTerms.ToDictionary(nt => nt, nt => new HashSet<string>());

            foreach (var nonTerm in NonTerms)
            {
                CalculateFirst(firstByNonTerm, nonTerm);
            }

            return firstByNonTerm;
        }
        public HashSet<string> FindFirst(string symbol)
        {
            if (FirstSets.ContainsKey(symbol))
            {
                return FirstSets[symbol];
            }

            return new HashSet<string>() { symbol };
        }
        private HashSet<string> CalculateFirst(Dictionary<string, HashSet<string>> firstByNonTerm, string symbol)
        {
            if (firstByNonTerm.ContainsKey(symbol) && firstByNonTerm[symbol].Count > 0)
            {
                return firstByNonTerm[symbol];
            }

            var first = new HashSet<string>();

            if (symbol == EPS)
            {
                first.Add(EPS);
                return first;
            }

            if (Terms.Contains(symbol))
            {
                first.Add(symbol);
                return first;
            }

            foreach (var alt in Rules[symbol])
            {
                foreach (var rightSymbol in alt)
                {
                    var firstOfRight = CalculateFirst(firstByNonTerm, rightSymbol);

                    foreach (var k in firstOfRight)
                    {
                        if (k != EPS || firstOfRight.Count == 1)
                        {
                            first.Add(k);
                        }
                    }

                    if (!firstOfRight.Contains(EPS))
                    {
                        break;
                    }
                }
            }

            firstByNonTerm[symbol] = first;
            return first;
        }

        public void ComputeFollowSets()
        {
            foreach (var nonTerm in NonTerms)
            {
                FollowSets[nonTerm] = new HashSet<string>();
            }

            FollowSets[axiom.non_term].Add("$");

            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (var rule in Rules)
                {
                    string leftSide = rule.Key;
                    List<List<string>> rightSides = rule.Value;

                    foreach (var rightSide in rightSides)
                    {
                        for (int i = 0; i < rightSide.Count; i++)
                        {
                            string symbol = rightSide[i];
                            if (NonTerms.Contains(symbol))
                            {
                                if (i == rightSide.Count - 1 || rightSide[i + 1] == EPS)
                                {
                                    changed |= AddAll(FollowSets[symbol], FollowSets[leftSide]);
                                }
                                else
                                {
                                    var firstBeta = First(rightSide.GetRange(i + 1, rightSide.Count - i - 1));
                                    if (firstBeta.Contains(EPS))
                                    {
                                        firstBeta.Remove(EPS);
                                        changed |= AddAll(FollowSets[symbol], firstBeta);
                                        changed |= AddAll(FollowSets[symbol], FollowSets[leftSide]);
                                    }
                                    else
                                    {
                                        changed |= AddAll(FollowSets[symbol], firstBeta);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        private HashSet<string> First(List<string> symbols)
        {
            HashSet<string> firstSet = new HashSet<string>();

            foreach (var symbol in symbols)
            {
                if (Terms.Contains(symbol))
                {
                    firstSet.Add(symbol);
                    break;
                }
                else
                {
                    var firstOfSymbol = FirstSets[symbol];
                    firstSet.UnionWith(firstOfSymbol);

                    if (!firstOfSymbol.Contains(EPS))
                    {
                        break;
                    }
                }
            }

            return firstSet;
        }
        private bool AddAll<T>(HashSet<T> target, HashSet<T> source)
        {
            int initialCount = target.Count;
            target.UnionWith(source);
            return target.Count > initialCount;
        }
    }


    readonly struct Axiom
    {
        public readonly string non_term { get; }

        public Axiom(string non_term) => this.non_term = non_term;

        public bool isEmpty { get { return non_term == string.Empty; } }


        public override string ToString()
        {
            return non_term;
        }
    }
}

```

Файл Compiler.cs
``` csharp
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
``` csharp
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
``` csharp
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
``` csharp
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
                return Index == Text.Length ? UnicodeCategory.OtherNotAssigned : char.GetUnicodeCategory(Text, Index);
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
                    Cp == '*' ||
                    Cp == '_';
            }
        }

        public bool isNonTerm
        {
            get
            {
                return (char.IsUpper(Text, Index) && char.IsLetter(Text, Index)) ;
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

Файл Scanner.cs
``` csharp
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
                        while (cur.isTerm || cur.IsDecimalDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if(cur.Cp != '\'' || cur.Cp == -1)
                        {
                            compiler.AddMessage(isErr: true, cur, "term must end with ' symbol!");   
                            return new Erorr(DomainTag.ERROR,prev_cur, cur.clone());    
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

                            if (cur.isNonTerm || cur.Cp == '_')
                            {
                                word += (char)cur.Cp;
                                cur++;
                                while (cur.IsDecimalDigit || cur.isNonTerm|| cur.Cp == '_')
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
                                            return new KeyWordToken(DomainTag.KW_TERMINAL, word, prev_cur, cur.clone());
                                        case "non-terminal":
                                            return new KeyWordToken(DomainTag.KW_NONTERMINAL, word, prev_cur, cur.clone());
                                        case "axiom":
                                            return new KeyWordToken(DomainTag.KW_AXIOM, word, prev_cur, cur.clone());
                                        case "epsilon":
                                            return new KeyWordToken(DomainTag.KW_EPS, word, prev_cur, cur.clone());

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

Файл LL1ParserTable.cs
``` csharp
using System;
using System.Collections.Generic;

namespace  lab3._1.src.GeneratedParser
{
    public class LL1ParserTable
    {
        public static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()
        {
            { "program kw_nonterminal", new string[] { "decl", "rules", "axiom" } },
            { "decl kw_nonterminal", new string[] { "kw_nonterminal", "non_terms", "sc", "kw_terminal", "terms", "sc" } },
            { "rules non_term", new string[] { "rule", "rules" } },
            { "rules kw_axiom", new string[] { } },
            { "axiom kw_axiom", new string[] { "kw_axiom", "non_term", "sc" } },
            { "rule non_term", new string[] { "non_term", "kw_eq", "alts", "sc" } },
            { "non_terms non_term", new string[] { "non_term", "non_term_tail" } },
            { "non_term_tail comma", new string[] { "comma", "non_terms" } },
            { "non_term_tail sc", new string[] { } },
            { "terms term", new string[] { "term", "term_tail" } },
            { "term_tail comma", new string[] { "comma", "terms" } },
            { "term_tail sc", new string[] { } },
            { "alts non_term", new string[] { "symbols", "alt_tail" } },
            { "alts term", new string[] { "symbols", "alt_tail" } },
            { "alts kw_eps", new string[] { "symbols", "alt_tail" } },
            { "alts or", new string[] { "symbols", "alt_tail" } },
            { "alts sc", new string[] { "symbols", "alt_tail" } },
            { "alt_tail or", new string[] { "or", "alts" } },
            { "alt_tail sc", new string[] { } },
            { "symbols non_term", new string[] { "non_term", "symbols" } },
            { "symbols term", new string[] { "term", "symbols" } },
            { "symbols kw_eps", new string[] { "kw_eps", "symbols" } },
            { "symbols or", new string[] { } },
            { "symbols sc", new string[] { } },
        };

        public static readonly string axiom = "program";

        public static readonly HashSet<string> NonTerms = new HashSet<string>()
        {
            "program",
            "decl",
            "rules",
            "axiom",
            "rule",
            "non_terms",
            "non_term_tail",
            "terms",
            "term_tail",
            "alts",
            "alt_tail",
            "symbols",
        };
    }
}

```


Файл Node.cs
``` csharp
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
        public string nterm { get; private set; }
        public List<INode> children { get; private set; }

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
        public Token tok { get; private set; }

        public Leaf(Token tok)
        {
            this.tok = tok;
        }

        public void Print(string indent)
        {
            string val = string.Empty;
            if (tok.Tag == DomainTag.TERM)
                val = ((TermToken)tok).term;
            if (tok.Tag == DomainTag.NON_TERM)
                val = ((NonTerm)tok).nterm;

            Console.WriteLine($"{indent}Лист:  {tok.Tag} {val}");

        }
    }

}


```


Файл Parser.cs
``` csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using lab3._1.src.GeneratedParser;
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
        Dictionary<string, string[]> debug_table = new Dictionary<string, string[]>()
        {
            {"program kw_nonterminal", new string[] {"decl","rules","axioms" } },
            {"rules kw_axiom", new string[] {} },
            {"rules non_term", new string[] {"rule","rules" } },
            {"axioms kw_axiom", new string[] {"axiom","axioms"} },
            {"decl kw_nonterminal", new string[] { "kw_nonterminal", "non_terms", "sc", "kw_terminal","terms","sc" } },
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
        Dictionary<string, string[]> real_table = LL1ParserTable.Table;
        bool isTerminal(string str)
        {
            return !(LL1ParserTable.NonTerms.Contains(str));
        }

        public INode parse(Scanner sc)
        {

            var stack = new Stack<StackNode>();

            InnerNode start = new InnerNode("");

            stack.Push(new StackNode() { value = LL1ParserTable.axiom, node = start });


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
                else if (real_table.Keys.Contains($"{top.value} {tok.Tag.ToString().ToLower()}"))
                {
                    var inner = new InnerNode(top.value);
                    top.node.AddChild(inner);

                    var go = real_table[$"{top.value} {tok.Tag.ToString().ToLower()}"];



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


Файл ParsingTable.cs
``` csharp
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using lab3._1.src.Gram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Parser
{
    internal class ParsingTable
    {
        public Dictionary<string, string[]> Table { get; private set; }
        private Grammar _grammar;
        public ParsingTable(Grammar g)
        {   
            _grammar = g;
            Table = new Dictionary<string, string[]>();

            foreach (var nonTerm in g.NonTerms)
            {
                foreach (var rule in g.Rules[nonTerm])
                {
                    var firstSet = ComputeFirstSet(rule,g);

                    foreach (var terminal in firstSet)
                    {
                        if (terminal != Grammar.EPS)
                        {
                            AddToTable(nonTerm, terminal, rule);
                        }
                    }

                    if (firstSet.Contains(Grammar.EPS))
                    {
                        foreach (var follow in g.FollowSets[nonTerm])
                        {
                            AddToTable(nonTerm, follow, rule);
                        }
                    }
                }
            }

        
        }

        private HashSet<string> ComputeFirstSet(List<string> production, Grammar g)
        {
            var firstSet = new HashSet<string>();

            foreach (var symbol in production)
            {
                if (g.Terms.Contains(symbol))
                {
                    firstSet.Add(symbol);
                    break;
                }

                if (g.NonTerms.Contains(symbol))
                {
                    firstSet.UnionWith(g.FirstSets[symbol]);
                    if (!g.FirstSets[symbol].Contains(Grammar.EPS))
                    {
                        break;
                    }
                }
            }

            if (firstSet.Count == 0 || firstSet.All(s => s == Grammar.EPS))
            {
                firstSet.Add(Grammar.EPS);
            }

            return firstSet;
        }

        private void AddToTable(string nonTerm, string terminal, List<string> production)
        {
            string key = $"{nonTerm} {terminal}";

            if (!Table.ContainsKey(key))
            {
                Table[key] = production.ToArray();
            }
            else
            {
                throw new GrammarNotLLException();
            }
        }

        public string GenerateCSharpCode() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace  lab3._1.src.GeneratedParser");
            sb.AppendLine("{");
            sb.AppendLine("    public class LL1ParserTable");
            sb.AppendLine("    {");
            sb.AppendLine("        public static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()");
            sb.AppendLine("        {");

            foreach (var entry in Table)
            {
                string key = entry.Key.ToLower();
                string[] values = entry.Value;

                if (values.Length == 1 && values[0] == Grammar.EPS)
                {
                    sb.AppendLine($"            {{ \"{key}\", new string[] {{ }} }},");
                }
                else
                {
                    string valuesString = string.Join("\", \"", values);
                    sb.AppendLine($"            {{ \"{key}\", new string[] {{ \"{valuesString.ToLower()}\" }} }},");
                }
            }

            sb.AppendLine("        };");
            sb.AppendLine();

            sb.AppendLine($"        public static readonly string axiom = \"{_grammar.axiom.non_term.ToLower()}\";");
            sb.AppendLine();

            sb.AppendLine("        public static readonly HashSet<string> NonTerms = new HashSet<string>()");
            sb.AppendLine("        {");

            foreach (var nonTerm in _grammar.NonTerms)
            {
                sb.AppendLine($"            \"{nonTerm.ToLower()}\",");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public void SaveToFile(string dir = "src\\Parser")
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, $"{dir}\\LL1ParserTable.cs");


            var content = GenerateCSharpCode();
            File.WriteAllText(filePath, content);
        }

        public void Print()
        {
            foreach (var entry in Table)
            {
                Console.WriteLine($"{entry.Key} -> {string.Join(" ", entry.Value)}");
            }
        }
    }
}

```

Файл EOFToken.cs
``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    internal class EOFToken : Token
    {
        public readonly string value;

        public EOFToken(string value, Position statring, Position following) : base(DomainTag.EOF, statring, following)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}

```


Файл KeyWordToken.cs
``` csharp
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

        public KeyWordToken(DomainTag tag, string value, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.KW_NONTERMINAL || tag == DomainTag.KW_EPS || tag == DomainTag.KW_AXIOM || tag == DomainTag.KW_TERMINAL||tag == DomainTag.KW_EQ);
            keyword = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {keyword}";
        }
    }
}

```


Файл NonTerm.cs
``` csharp
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
        public NonTerm(string nterm, Position statring, Position following) : base(DomainTag.NON_TERM, statring, following)
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
``` csharp
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

        public SpecialToken(DomainTag tag, string val, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.SC || tag == DomainTag.NL || tag == DomainTag.COMMA || tag == DomainTag.OR);
            sym = val;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {sym}";
        }
    }
}

```


Файл TermToken.cs
``` csharp
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
        public TermToken(string term, Position statring, Position following) : base(DomainTag.TERM, statring, following)
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


Файл Token.cs
``` csharp
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


Программа калькулятора


# Тестирование

Входные данные

```
…
```

Вывод на `stdout`

```
…
```

# Вывод
‹пишете, чему научились›