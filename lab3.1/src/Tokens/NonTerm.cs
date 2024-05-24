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
