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

        public NumberToken(DomainTag tag, double value, Position starting, Position following) : base(tag, starting, following)
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
