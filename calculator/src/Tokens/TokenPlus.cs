using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.src.Tokens
{
    internal class TokenPlus : Token
    {
        string val = "+";

        public TokenPlus(Position statring, Position following) : base(DomainTag.PLUS, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {val}";
        }
    }
}
