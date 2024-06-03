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

        public EOFToken(Position statring, Position following) : base(DomainTag.EOF, statring, following)
        {
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}
