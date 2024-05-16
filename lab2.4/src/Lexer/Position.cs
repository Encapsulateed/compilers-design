using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Lexer
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
}
