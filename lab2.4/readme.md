% Лабораторная работа № 2.4 «Рекурсивный спуск»
% 5 июня 2024 г.
% Алексей Митрошкин, ИУ9-31Б

# Цель работы
Целью данной работы является изучение алгоритмов построения парсеров методом рекурсивного спуска.


# Индивидуальный вариант
``` pascal
Type
  Coords = Record x, y: INTEGER end;
Const
  MaxPoints = 100;
type
  CoordsVector = array [1..MaxPoints] of Coords;

(* графический и текстовый дисплеи *)
const
  Heigh = 480;
  Width = 640;
  Lines = 24;
  Columns = 80;
type
  BaseColor = (red, green, blue, highlited);
  Color = set of BaseColor;
  GraphicScreen = array [1..Heigh] of array [1..Width] of Color;
  TextScreen = array 1..Lines of array 1..Columns of
    record
      Symbol : CHAR;
      SymColor : Color;
      BackColor : Color
    end;

{ определения токенов }
TYPE
  Domain = (Ident, IntNumber, RealNumber);
  Token = record
    fragment : record
      start, following : record
        row, col : INTEGER
      end
    end;
    case tokType : Domain of
      Ident : (
        name : array 1..32 of CHAR
      );
      IntNumber : (
        intval : INTEGER
      );
      RealNumber : (
        realval : REAL
      )
  end;

  Year = 1900..2050;

  List = record
    value : Token;
    next : ^List
  end;
```
# Реализация
## Лексическая структура
Язык содержит следующие терминальные домены
```
INTEGER = '[0-9]+'
REAL = '[0-9]+(\\.[0-9]*)?(e[-+]?[0-9]+)?'
IDENT = '[A-Za-z][A-Za-z0-9]*'
```
…

## Грамматика языка
```
program -> (block)+ .
                                            
block -> TYPE record_block .
block -> CONST const_block  .
         
record_block -> (type_def)+ .

const_block -> (constant_def)+ .

constant_def -> ident '=' constant ';' .

type_def -> ident '=' type ';'

type -> simple_type | '^' type_ident |  array | file | set | record .

array -> ARRAY '[' (simple_type)+ ']' OF type .
file -> FILE OF type .
set -> SET OF simple_type .
record ->  RECORD (filed)+ END .

simple_type -> type_ident | '(' (ident)+ ')' | two_constants.
     
two_constants ->  constant '..' constant

field -> (ident)+ ':' type  (';')?  .
field -> CASE ident ':' type_ident OF (const_item)+ .


const_item ->  (constant)+ ':' '(' (filed)+ ')' .


constant -> number_constant | character_constant.

number_constant -> constant_ident | unsigned_number.

unsigned_number -> unsigned_int | unsigned_float.


character_constant -> STRING .
unsigned_float -> REAL .
unsigned_int  -> INTEGER . 
ident -> IDENT
```
                   

## Программная реализация
Файл Complier.cs
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
                return Index == Text.Length ? UnicodeCategory.OtherNotAssigned :
                 char.GetUnicodeCategory(Text, Index);
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
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Tokens;
using lab2._4.src.Tokens;

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
                    case ':':
                        cur++;

                        return new SpecialToken(DomainTag.COLON, prev_cur, cur.clone());

                    case ';':
                        cur++;

                        return new SpecialToken(DomainTag.SC, prev_cur, cur.clone());

                    case ',':
                        cur++;

                        return new SpecialToken(DomainTag.COMMA, prev_cur, cur.clone());

                    case '(':
                        cur++;

                        return new SpecialToken(DomainTag.LB, prev_cur, cur.clone());

                    case ')':
                        cur++;

                        return new SpecialToken(DomainTag.RB, prev_cur, cur.clone());

                    case '[':
                        cur++;

                        return new SpecialToken(DomainTag.SLB, prev_cur, cur.clone());

                    case ']':
                        cur++;

                        return new SpecialToken(DomainTag.SRB, prev_cur, cur.clone());

                    case '^':
                        cur++;

                        return new SpecialToken(DomainTag.ARROW, prev_cur, cur.clone());

                    case '.':
                        cur++;
                        cur++;

                        return new SpecialToken(DomainTag.TWO_DOTS, prev_cur, cur.clone());
                    case '=':
                        cur++;

                        return new SpecialToken(DomainTag.EQ, prev_cur, cur.clone());
                    default:
                        if (cur.IsLetter)
                        {
                            do
                            {
                                word += (char)cur.Cp;
                                cur++;
                            } while (cur.IsLetterOrDigit);

                            switch (word.ToLower())
                            {
                                case "type":
                                    return new KeyWordToken(DomainTag.KW_TYPE, prev_cur, 
                                    cur.clone());
                                case "array":
                                    return new KeyWordToken(DomainTag.KW_ARRAY, prev_cur,
                                     cur.clone());
                                case "set":
                                    return new KeyWordToken(DomainTag.KW_SET, prev_cur,
                                     cur.clone());
                                case "file":
                                    return new KeyWordToken(DomainTag.KW_FILE, prev_cur,
                                     cur.clone());
                                case "const":
                                    return new KeyWordToken(DomainTag.KW_CONST, prev_cur,
                                     cur.clone());
                                case "record":
                                    return new KeyWordToken(DomainTag.KW_RECORD, prev_cur,
                                     cur.clone());
                                case "of":
                                    return new KeyWordToken(DomainTag.KW_OF, prev_cur,
                                     cur.clone());
                                case "paked":
                                    return new KeyWordToken(DomainTag.KW_PAKED, prev_cur,
                                     cur.clone());
                                case "case":
                                    return new KeyWordToken(DomainTag.KW_CASE, prev_cur,
                                     cur.clone());
                                case "end":
                                    return new KeyWordToken(DomainTag.KW_END, prev_cur,
                                     cur.clone());

                                default:
                                    return new IdentToken(word, prev_cur, cur.clone());

                            }


                        }
                        else if (cur.IsDecimalDigit)
                        {
                            double val = cur.Cp - '0';
                            cur++;

                            while (cur.IsDecimalDigit)
                            {
                                val = checked(val * 10 + cur.Cp - '0');
                                cur++;

                            }

                            return new NumberToken(DomainTag.INTEGER, val, prev_cur, cur.clone());
                        }


                        break;
                }
            }

            return new EOFToken(cur, cur);
        }


    }
}

```

Файл Node.cs
```csharp
using System;
using System.Collections.Generic;

namespace lab2._4.src.Nodes
{
    public interface INode
    {
        void Print(string indent);
    }

    public class Program : INode
    {
        public List<Block> blocks;

        public Program(List<Block> blocks) => this.blocks = blocks;
        public void Print(string indent)
        {
            Console.WriteLine($"{indent}Program");
            foreach (var b in blocks)
            {
                b.Print(indent + "\t");
            }
        }
    }

    public abstract class Block : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    public class TypeBlock : Block, INode
    {
        public List<TypeDef> typeDefs;

        public TypeBlock(List<TypeDef> typeDefs)
        {
            this.typeDefs = typeDefs;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeBlock");
            foreach (var td in typeDefs)
            {
                td.Print(indent + "\t");
            }
        }
    }

    public class ConstBlock : Block, INode
    {
        public List<ConstDef> constDefs;

        public ConstBlock(List<ConstDef> constDefs)
        {
            this.constDefs = constDefs;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}ConstBlock");
            foreach (var td in constDefs)
            {
                td.Print(indent + "\t");
            }
        }
    }

    public class TypeDef : INode
    {
        TypeIdent id;
        Type type;

        public TypeDef(TypeIdent id, Type type)
        {
            this.id = id;
            this.type = type;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeDef");
            id.Print(indent + "\t");
            type.Print(indent + "\t");
        }
    }

    public class ConstDef : INode
    {
        ConstantIdent id;
        Constant constant;

        public ConstDef(ConstantIdent id, Constant constant)
        {
            this.id = id;
            this.constant = constant;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}ConstDef");
            id.Print(indent + "\t");
            constant.Print(indent + "\t");
        }

    }

    public class Ident : INode
    {
        string id;
        public Ident(string id) { this.id = id; }
        public void Print(string indent)
        {
            Console.WriteLine($"{indent}Ident: {id}");
        }
    }


    public class TypeIdent : SimpleType, INode
    {
        string id;
        public TypeIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeIdent: {id}");
        }
    }

    public abstract class Constant : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    public abstract class Type : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    public class PointerIdent : Type, INode
    {
        string id;
        public PointerIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}PointerIdent({id})");
        }
    }
    public abstract class SimpleType : Type, INode
    {
        public override void Print(string indent)
        {
        }
    }

    public class Idents : SimpleType, INode
    {
        public List<Ident> idents;

        public Idents(List<Ident> idents)
        {
            this.idents = idents;
        }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Idents");

            foreach (var id in idents)
            {
                id.Print(indent + "\t");
            }
        }
    }

    public class TwoConstants : SimpleType, INode
    {
        public Constant first;
        public Constant secod;

        public TwoConstants(Constant first, Constant secod)
        {
            this.first = first;
            this.secod = secod;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Two Constants");
            first.Print(indent + "\t");
            secod.Print(indent + "\t");
        }
    }

    public class Array : Type, INode
    {
        public Type Type;
        public List<SimpleType> simpleTypes;

        public Array(Type type, List<SimpleType> simpleTypes)
        {
            Type = type;
            this.simpleTypes = simpleTypes;
        }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}array");

            foreach (var st in simpleTypes)
            {
                st.Print(indent + "");
            }
            Type.Print(indent + "");

        }
    }


    public class Set : Type, INode
    {
        public SimpleType simpleType;
        public Set(SimpleType simpleType)
        {
            this.simpleType = simpleType;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Set");
            simpleType.Print(indent + "\t");
        }
    }

    public class Record : Type, INode
    {
        List<Field> fields;

        public Record(List<Field> fields)
        {
            this.fields = fields;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Record");
            foreach (var f in fields)
            {
                f.Print(indent + "\t");
            }
        }
    }

    abstract public class Field : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    public class SimpleFiled : Field, INode
    {
        List<Ident> idents;
        Type Type;

        public SimpleFiled(List<Ident> idents, Type type)
        {
            this.idents = idents;
            Type = type;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}SimpleField");
            foreach (var ident in idents)
            {
                ident.Print(indent + "\t");
            }
            Type.Print(indent + "\t");
        }
    }

    public class CaseFiled : Field, INode
    {
        Ident ident;
        Ident type_ident;
        List<ConstantItem> constantItems;

        public CaseFiled(Ident ident, Ident type_ident, List<ConstantItem> constantItems)
        {
            this.ident = ident;
            this.type_ident = type_ident;
            this.constantItems = constantItems;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}CaseField");
            ident.Print(indent + "\t");
            type_ident.Print(indent + "\t");

            foreach (var constant in constantItems)
            {
                constant.Print(indent + "\t");
            }
        }
    }

    public class ConstantItem : INode
    {
        public List<Constant> constants;
        public List<Field> fields;

        public ConstantItem(List<Constant> constants, List<Field> fields)
        {
            this.constants = constants;
            this.fields = fields;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}ConstantItem");
            foreach (var c in constants)
            {
                c.Print(indent + "\t");
            }
            foreach (var f in fields)
            {
                f.Print(indent + "\t");
            }
        }
    }

    public class UnsignedNumber : Constant, INode
    {
        double val;
        public UnsignedNumber(double val) { this.val = val; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Unsigned_Number: {val}");
        }
    }

    public class ConstantIdent : Constant, INode
    {
        string id;
        public ConstantIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Constant_Ident : {id}");
        }
    }
}

```

Файл Parser.cs
```csharp
using System;
using System.Collections.Generic;
using lab2._3.src.Tokens;
using lab2._3.src.Lexer;
using lab2._4.src.Nodes;
using lab2._4.src.Tokens;

namespace lab2._3.src.Parser
{
    class Parser
    {
        public Scanner sc;
        public Token currentToken;

        public Parser(Scanner sc)
        {
            this.sc = sc;
            this.currentToken = sc.NextToken();
        }

        private void Advance()
        {
            currentToken = sc.NextToken();
        }

        private bool Match(params DomainTag[] types)
        {
            foreach (var type in types)
            {
                if (currentToken.Tag == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Consume(DomainTag type, string message)
        {

            if (currentToken.Tag == type)
            {
                var token = currentToken;
                Advance();
                return token;
            }
            if (currentToken.Tag == DomainTag.EOF) 
            {
                return currentToken;
            }
            throw new Exception($"Parse error: {message} at {currentToken.Coords}");
        }
        /*
            program -> (block)+    
        */
        public Program Parse()
        {
            var blocks = new List<Block>();
            while (currentToken.Tag != DomainTag.EOF)
            {
                blocks.Add(ParseBlock());
            }
            return new Program(blocks);
        }
        /*
            block -> type_block | const_block   

            
        */

        private Block ParseBlock()
        {
            if (Match(DomainTag.KW_TYPE))
            {
                return ParseTypeBlock();
            }
            else if (Match(DomainTag.KW_CONST))
            {
                return ParseConstBlock();
            }
            throw new Exception("Expected type or const block");
        }
        // type_block -> KW_TYPE (type_def)+
        private TypeBlock ParseTypeBlock()
        {
            var typeDefs = new List<TypeDef>();
            while (!Check(DomainTag.KW_TYPE) && !Check(DomainTag.KW_CONST) && 
            currentToken.Tag != DomainTag.EOF)
            {
                typeDefs.Add(ParseTypeDef());
            }
            return new TypeBlock(typeDefs);
        }
        // const_block -> KW_CONST (const_def)+
        private ConstBlock ParseConstBlock()
        {
            var constDefs = new List<ConstDef>();
            while (!Check(DomainTag.KW_CONST) && !Check(DomainTag.KW_TYPE) && 
            currentToken.Tag != DomainTag.EOF)
            {
                constDefs.Add(ParseConstDef());
            }
            return new ConstBlock(constDefs);
        }

        // type_def -> ident KW_EQ type SC
        private TypeDef ParseTypeDef()
        {
            var ident = new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name);
            Consume(DomainTag.EQ, "Expected '='");
            var type = ParseType();
            Consume(DomainTag.SC, "Expected ';'");
            return new TypeDef(ident, type);
        }
        // const_def -> ident KW_EQ constant SC
        private ConstDef ParseConstDef()
        {
            var ident = new ConstantIdent(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name);
            Consume(DomainTag.EQ, "Expected '='");
            var constant = ParseConstant();
            Consume(DomainTag.SC, "Expected ';'");
            return new ConstDef(ident, constant);
        }

        // type -> simple_type | ARROW ident | array | record | set
        private lab2._4.src.Nodes.Type ParseType()
        {
            if (Match(DomainTag.ARROW))
            {
                return new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, 
                "Expected identifier")).Name);
            }
            else if (Match(DomainTag.KW_ARRAY))
            {
                return ParseArrayType();
            }
            else if (Match(DomainTag.KW_SET))
            {
                return ParseSetType();
            }
            else if (Match(DomainTag.KW_RECORD))
            {
                return ParseRecordType();
            }
            return ParseSimpleType();
        }

        // array -> KW_ARRAY SLB (simple_type,)* simple_type SRB KW_OF type 
        private lab2._4.src.Nodes.Array ParseArrayType()
        {
            Consume(DomainTag.SLB, "Expected '['");
            var simpleTypes = new List<SimpleType> { ParseSimpleType() };
            while (Match(DomainTag.COMMA))
            {
                simpleTypes.Add(ParseSimpleType());
            }
            Consume(DomainTag.SRB, "Expected ']'");
            Consume(DomainTag.KW_OF, "Expected 'OF'");
            var type = ParseType();
            return new lab2._4.src.Nodes.Array(type, simpleTypes);
        }

        // set -> KW_SET KW_OF type 

        private Set ParseSetType()
        {
            Consume(DomainTag.KW_OF, "Expected 'OF'");
            var simpleType = ParseSimpleType();
            return new Set(simpleType);
        }

        // record -> KW_RECORD (field)+ KW_END 

        private Record ParseRecordType()
        {
            var fields = new List<Field>();
            while (!Check(DomainTag.EOF) )
            {

                fields.Add(ParseField());
                if (Check(DomainTag.KW_END))
                    break;

            }

            Consume(DomainTag.KW_END, "Expected 'end'");
            return new Record(fields);
        }

        //set -> ident | LB idents RB | two_constants

        private SimpleType ParseSimpleType()
        {
            if (currentToken.Tag == DomainTag.IDENT)
            {
                return new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, 
                "Expected identifier")).Name);
            }
            else if (Match(DomainTag.LB))
            {
                var idents = new List<Ident> { new Ident(((IdentToken)Consume(DomainTag.IDENT, 
                "Expected identifier")).Name) };
                while (Match(DomainTag.COMMA))
                {
                    idents.Add(new Ident(((IdentToken)Consume(DomainTag.IDENT, 
                    "Expected identifier")).Name));
                }
                Consume(DomainTag.RB, "Expected ')'");
                return new Idents(idents);
            }
            return ParseTwoConstants();
        }

        // two_constants -> constant TWO_DOTS constant
        private TwoConstants ParseTwoConstants()
        {
            var first = ParseConstant();
            Consume(DomainTag.TWO_DOTS, "Expected '..'");
            var second = ParseConstant();
            return new TwoConstants(first, second);
        }

        // field -> simple_field | case_field
        private Field ParseField()
        {
            if (Match(DomainTag.KW_CASE))
            {
                return ParseCaseField();
            }
            return ParseSimpleField();
        }

        // simple_field -> idents COLON type SC 

        private SimpleFiled ParseSimpleField()
        {

            var idents = new List<Ident> { new Ident(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name) };
            while (Match(DomainTag.COMMA))
            {
                idents.Add(new Ident(((IdentToken)Consume(DomainTag.IDENT, 
                "Expected identifier")).Name));
            }
            Consume(DomainTag.COLON, "Expected ':'");
            var type = ParseType();

            if (Check(DomainTag.SC))
                Advance();

            return new SimpleFiled(idents, type);
        }

        // simple_field -> KW_CASE ident COLON ident OF constant_item (;constant_item)* 

        private CaseFiled ParseCaseField()
        {
            var ident = new Ident(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name);

            Consume(DomainTag.COLON, "Expected ':'");

            var typeIdent = new Ident(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name);

            Consume(DomainTag.KW_OF, "Expected 'of'");


            var constantItems = new List<ConstantItem> { ParseConstantItem() };

            while (Match(DomainTag.SC))
            {
                constantItems.Add(ParseConstantItem());
            }
            return new CaseFiled(ident, typeIdent, constantItems);
        }

        // constant_item -> constant (,constant)* COLON LB (filed)+ RB

        private ConstantItem ParseConstantItem()
        {
            var constants = new List<Constant> { ParseConstant() };

            while (Match(DomainTag.COMMA))
            {
                constants.Add(ParseConstant());
            }
            Consume(DomainTag.COLON, "Expected ':'");

            Consume(DomainTag.LB, "Expected '('");

            var fields = new List<Field> { ParseField() };

            while (Match(DomainTag.SC))
            {
                fields.Add(ParseField());
            }
            Consume(DomainTag.RB, "Expected ')'");

            return new ConstantItem(constants, fields);
        }

        // constant -> number_constant
        // number_constant -> INTEGER

        private Constant ParseConstant()
        {
            if (currentToken.Tag == DomainTag.INTEGER)
            {
                return new UnsignedNumber(((NumberToken)Consume(DomainTag.INTEGER, 
                "Expected integer")).Value);
            }
            return new ConstantIdent(((IdentToken)Consume(DomainTag.IDENT, 
            "Expected identifier")).Name);
        }

        private bool Check(DomainTag type) => currentToken.Tag == type;
    }
}

```


Файл EOFToken.cs
```csharp
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

        public EOFToken(Position statring, Position following)
         : base(DomainTag.EOF, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}

```


Файл IdentToken.cs
```csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Tokens
{
    internal class IdentToken : Token
    {
        public string Name;
        public IdentToken(string name, Position statring, Position following) 
        : base(DomainTag.IDENT, statring, following)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {Name}";
        }
    }
}

```


Файл InvalidToken.cs
```csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Tokens
{
    internal class InvalidToken : Token
    {
        public InvalidToken(Position statring, Position following) 
        : base(DomainTag.INVALID, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}

```

Файл KeyWorkdToken.cs
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

        public KeyWordToken(DomainTag tag, Position starting, Position following) 
        : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.KW_ARRAY ||
                         tag == DomainTag.KW_TYPE ||
                         tag == DomainTag.KW_CONST ||
                         tag == DomainTag.KW_FILE ||
                         tag == DomainTag.KW_SET ||
                         tag == DomainTag.KW_RECORD ||
                         tag == DomainTag.KW_OF ||
                         tag == DomainTag.KW_PAKED ||
                         tag == DomainTag.KW_CASE ||
                         tag == DomainTag.KW_END);
        }

        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}

```

Файл NumberToken.cs
```csharp
using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Tokens
{
    internal class NumberToken : Token
    {
        public readonly double Value;

        public NumberToken(DomainTag tag, double value, Position starting, Position following) 
        : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.REAL || tag == DomainTag.INTEGER);

            Value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {Value}";
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

        public SpecialToken(DomainTag tag, Position starting, Position following) 
        : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.SC ||
                tag == DomainTag.COLON || 
                tag == DomainTag.EQ || 
                tag == DomainTag.LB ||
                tag == DomainTag.RB || 
                tag == DomainTag.SLB ||
                tag == DomainTag.SRB ||
                tag == DomainTag.ARROW || 
                tag == DomainTag.TWO_DOTS ||
                tag == DomainTag.COMMA
                );
        }

        public override string ToString()
        {
            return $"{Tag} {Coords}";
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
        KW_TYPE,
        KW_CONST,
        KW_ARRAY,
        KW_FILE,
        KW_SET,
        KW_RECORD,
        KW_OF,
        KW_PAKED,
        KW_CASE,
        KW_END,
        EQ,
        SC,
        COMMA,
        LB,
        RB,
        SLB,
        SRB,
        ARROW,
        TWO_DOTS,
        COLON,
        IDENT,
        INTEGER,
        REAL,
        INVALID,
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

Файл Program.cs
```csharp
using lab2._3.src.Lexer;
using lab2._3.src.Parser;
using lab2._3.src.Tokens;
using lab2._4.src.Tokens;

namespace lab2._4
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


            Parser parser = new Parser(sc);

            lab2._4.src.Nodes.Program syntaxTree = parser.Parse();

            syntaxTree.Print("");
        }
    }
}

```

# Тестирование

Входные данные
``` pascal
Type
  Coords = Record x, y: INTEGER end;
Const
  MaxPoints = 100;
type
  CoordsVector = array [1..MaxPoints] of Coords;

(* графический и текстовый дисплеи *)
const
  Heigh = 480;
  Width = 640;
  Lines = 24;
  Columns = 80;
type
  BaseColor = (red, green, blue, highlited);
  Color = set of BaseColor;
  GraphicScreen = array [1..Heigh] of array [1..Width] of Color;
  TextScreen = array 1..Lines of array 1..Columns of
    record
      Symbol : CHAR;
      SymColor : Color;
      BackColor : Color
    end;

{ определения токенов }
TYPE
  Domain = (Ident, IntNumber, RealNumber);
  Token = record
    fragment : record
      start, following : record
        row, col : INTEGER
      end
    end;
    case tokType : Domain of
      Ident : (
        name : array 1..32 of CHAR
      );
      IntNumber : (
        intval : INTEGER
      );
      RealNumber : (
        realval : REAL
      )
  end;

  Year = 1900..2050;

  List = record
    value : Token;
    next : ^List
  end;
```

Вывод на `stdout`

```
Program
        TypeBlock
                TypeDef
                        TypeIdent: Coords
                        Record
                                SimpleField
                                        Ident: x
                                        Ident: y
                                        TypeIdent: ABC
        ConstBlock
                ConstDef
                        Constant_Ident : MaxPoints
                        Unsigned_Number: 100
        TypeBlock
                TypeDef
                        TypeIdent: CoordsVector
                        array
                        Two Constants
                                Unsigned_Number: 1
                                Constant_Ident : MaxPoints
                        TypeIdent: Coords
        ConstBlock
                ConstDef
                        Constant_Ident : Heigh
                        Unsigned_Number: 480
                ConstDef
                        Constant_Ident : Width
                        Unsigned_Number: 640
                ConstDef
                        Constant_Ident : Lines
                        Unsigned_Number: 24
                ConstDef
                        Constant_Ident : Columns
                        Unsigned_Number: 80
        TypeBlock
                TypeDef
                        TypeIdent: BaseColor
                        Idents
                                Ident: red
                                Ident: green
                                Ident: blue
                                Ident: highlited
                TypeDef
                        TypeIdent: Color
                        Set
                                TypeIdent: BaseColor
                TypeDef
                        TypeIdent: GraphicScreen
                        array
                        Two Constants
                                Unsigned_Number: 1
                                Constant_Ident : Heigh
                        array
                        Two Constants
                                Unsigned_Number: 1
                                Constant_Ident : Width
                        TypeIdent: Color
                TypeDef
                        TypeIdent: TextScreen
                        array
                        Two Constants
                                Unsigned_Number: 1
                                Constant_Ident : Lines
                        array
                        Two Constants
                                Unsigned_Number: 1
                                Constant_Ident : Columns
                        Record
                                SimpleField
                                        Ident: Symbol
                                        TypeIdent: CHAR
                                SimpleField
                                        Ident: SymColor
                                        TypeIdent: Color
                                SimpleField
                                        Ident: BackColor
                                        TypeIdent: Color
        TypeBlock
                TypeDef
                        TypeIdent: Domain
                        Idents
                                Ident: Ident
                                Ident: IntNumber
                                Ident: RealNumber
                TypeDef
                        TypeIdent: Token
                        Record
                                SimpleField
                                        Ident: fragment
                                        Record
                                                SimpleField
                                                        Ident: start
                                                        Ident: following
                                                        Record
                                                                SimpleField
                                                                        Ident: row
                                                                        Ident: col
                                                                        TypeIdent: INTEGER
                                CaseField
                                        Ident: tokType
                                        Ident: Domain
                                        ConstantItem
                                                Constant_Ident : Ident
                                                SimpleField
                                                        Ident: name
                                                        array
                                                        Two Constants
                                                                Unsigned_Number: 1
                                                                Unsigned_Number: 32
                                                        TypeIdent: CHAR
                                        ConstantItem
                                                Constant_Ident : IntNumber
                                                SimpleField
                                                        Ident: intval
                                                        TypeIdent: INTEGER
                                        ConstantItem
                                                Constant_Ident : RealNumber
                                                SimpleField
                                                        Ident: realval
                                                        TypeIdent: REAL
                TypeDef
                        TypeIdent: Year
                        Two Constants
                                Unsigned_Number: 1900
                                Unsigned_Number: 2050
                TypeDef
                        TypeIdent: List
                        Record
                                SimpleField
                                        Ident: value
                                        TypeIdent: Token
                                SimpleField
                                        Ident: next
                                        TypeIdent: List

```

# Вывод
В ходе данной лабораторной работы я научился разрабатывать синтаксические анализаторы
методом рекурсивного спуска. Если честно, этот метод показался мне наиболее понятным и
очевидным в реализации - просто описал РБНФ грамматику -> описал классы данных 
-> описал правила грамматики в коде. 