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
