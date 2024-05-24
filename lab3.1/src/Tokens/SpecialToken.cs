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
        public readonly string sym;

        public SpecialToken(DomainTag tag, string val, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.SC || tag == DomainTag.NL || tag == DomainTag.COMMA || tag == DomainTag.OR);
            sym = val;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {sym}";
        }
    }
}
