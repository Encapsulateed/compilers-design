using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class SemiColonToken : Token
    {
        public SemiColonToken(DomainTag tag, Position statring, Position following) : base(tag, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} ;";
        }
    }
}
