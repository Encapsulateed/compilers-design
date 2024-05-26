using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.src.Tokens
{
    internal class RightBracketToken : Token
    {
        string val = ")";

        public RightBracketToken( Position statring, Position following) : base(DomainTag.RB, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {val}";
        }
    }
}
