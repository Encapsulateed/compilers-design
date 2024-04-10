% Лабораторная работа № 1.3 «Объектно-ориентированный
  лексический анализатор»
% 10 апреля 2024 г.
% Алексей Митрошкин, ИУ9-31Б

# Цель работы
Целью данной работы является приобретение навыка реализации лексического анализатора на объектно-ориентированном языке без применения каких-либо средств автоматизации решения задачи лексического анализа.

# Индивидуальный вариант
- Идентификаторы: последовательности буквенных символов Unicode и цифр,
начинающиеся с буквы, не чувствительны к регистру.
- Целочисленные константы: десятичные — последовательности десятичных цифр,
шестнадцатеричные — последовательности шестнадцатиричных цифр, 
начинающиеся на «&H», тоже не чувствительны к регистру.
- Ключевые слова — «PRINT», «GOTO», «GOSUB» без учёта регистра.

# Реализация
Файл `Position.cs`
```C#
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
                return Index == Text.Length ? -1 : char.ConvertToUtf32(Text.ToLower(), Index);
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
```

Файл `Token.cs`
```C#
    enum DomainTag
    {
        IDENT,
        NUMBER,
        HEX_NUMBER,
        PRINT_KEYWORD,
        GOTO_KEYWORD,
        GOSUB_KEYWORD,
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
```


Файл `Scanner.cs`
```C#        private Compiler compiler;
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

                    case 'p':

                        while (cur.IsLetterOrDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if (word.ToLower() == "print")
                        {
                            return new KeyWordToken(DomainTag.PRINT_KEYWORD, word, prev_cur, cur.clone());
                        }
                        else
                        {
                           
                          return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());
                        }
                        break;
                    case 'g':
                        while (cur.IsLetterOrDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if (word.ToLower() == "goto")
                        {
                            return new KeyWordToken(DomainTag.GOTO_KEYWORD, word, prev_cur, cur.clone());

                        }
                        else if (word.ToLower() == "gosub")
                        {
                            return new KeyWordToken(DomainTag.GOSUB_KEYWORD, word, prev_cur, cur.clone());

                        }
                        else
                        {

                            return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());

                            break;
                        }
                    case '&':
                        // пропускаем & H
                        cur++;
                        cur++;
                        do
                        {
                            word += (char)cur.Cp;
                            cur++;

                        } while (cur.IsLetterOrDigit);
             
                        return new NumberToken(Convert.ToInt64(word.ToUpper(),16), prev_cur, cur.clone());
                    default:
                        if (cur.IsLetter)
                        {
                            do
                            {
                                word += (char)cur.Cp;
                                cur++;
                            } while (cur.IsLetterOrDigit);



                            return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());

                        }
                        else if (cur.IsDecimalDigit)
                        {
                            int val = cur.Cp - '0';
                            //пропускаем первую букву
                            cur++;
                            try
                            {
                                while (cur.IsDecimalDigit)
                                {
                                    val = checked(val * 10 + cur.Cp - '0');
                                    cur++;

                                }
                            }
                            catch (OverflowException)
                            {
                                compiler.AddMessage(true, prev_cur, "константа слишком большая");
                                while (cur.IsDecimalDigit) cur++;
                            }

                            if (cur.IsLetter)
                            {
                                compiler.AddMessage(true, prev_cur, "нужен разделитель");

                            }

                            return new NumberToken(val, prev_cur, cur.clone());
                        }
                        else
                        {
                            while (cur.Cp is not '\n' && cur.Cp is not ' ' && cur.Cp is not -1)
                                cur++;
                        }
                        break;
                }
                cur++;
            }

            return new EOFToken("", cur, cur);
        }
```

Файл `Program.cs`
```C#
var lines = File.ReadAllLines("input.txt");
string prg = "";

foreach (var line in lines)
{
    prg += line + '\n';
}

var cp = new Compiler();
var sc = new Scanner(prg, cp);

Console.WriteLine(prg);


var tok = sc.NextToken();
while(tok.Tag != DomainTag.EOF)
{
    var st = tok.ToString();
    if (tok.Tag == DomainTag.IDENT)
    {
        st += $" IDENT NAME: {cp.GetName(((IdentToken)tok).Code)}";
    }
    Console.WriteLine(st);
    tok = sc.NextToken();
    
    if(tok.Tag == DomainTag.EOF)
    {
        Console.WriteLine(tok.ToString());
    }
 
}
Console.WriteLine();
cp.OutPutMessages();
```

Файл `Fragment.cs`
```C#
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
```

Файл `IdentToken.cs`
```C#
 internal class IdentToken : Token
    {
        public readonly int Code;
        public string Name;
        public IdentToken(int code, Position statring, Position following) : base(DomainTag.IDENT, statring, following)
        {
            Code = code;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {Code}";
        }
    }
```

Файл `KeyWordToken.cs`
```C#
  internal class KeyWordToken : Token
    {
        public readonly string keyword;

        public KeyWordToken(DomainTag tag, string value, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.GOSUB_KEYWORD || tag == DomainTag.GOTO_KEYWORD || tag == DomainTag.PRINT_KEYWORD);
            keyword = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {keyword}";
        }
    }
```
Файл `NumberToken.cs`
```C#
    internal class NumberToken : Token
    {
        public readonly long Value;

        public NumberToken(long value, Position starting, Position following) : base(DomainTag.NUMBER, starting, following)
        {
            Value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {Value}";
        }

    }
```

Файл `Message.cs`
```C#
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
```
# Тестирование

Входные данные

```
&ha12 alfa beta 123
m1 222222222222222222222
goto zero2 123
alfa  233234xxx  Goto Abc ABC
1a        

```

Вывод на `stdout`

```
NUMBER (1,1)-(1,6) 2578
IDENT (1,7)-(1,11) 0 IDENT NAME: alfa
IDENT (1,12)-(1,16) 1 IDENT NAME: beta
NUMBER (1,17)-(1,20) 123
IDENT (2,1)-(2,3) 2 IDENT NAME: m1
NUMBER (2,4)-(2,25) 222222222
GOTO_KEYWORD (3,1)-(3,5) goto
IDENT (3,6)-(3,11) 3 IDENT NAME: zero2
NUMBER (3,12)-(3,15) 123
IDENT (4,1)-(4,5) 0 IDENT NAME: alfa
NUMBER (4,7)-(4,13) 233234
IDENT (4,13)-(4,16) 4 IDENT NAME: xxx
GOTO_KEYWORD (4,18)-(4,22) goto
IDENT (4,23)-(4,26) 5 IDENT NAME: abc
IDENT (4,27)-(4,30) 5 IDENT NAME: abc
NUMBER (5,1)-(5,2) 1
IDENT (5,2)-(5,3) 6 IDENT NAME: a
EOF (6,1)-(6,1)

Erorr (2,4) константа слишком большая
Erorr (4,7) нужен разделитель
Erorr (5,1) нужен разделитель
```

# Вывод
Научился реализовывать объектно оринетированный лексический анализатор 