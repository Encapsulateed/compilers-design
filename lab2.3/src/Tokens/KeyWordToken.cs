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
        public readonly string keyword;

        public KeyWordToken(DomainTag tag, string value, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.KW_NONTERMINAL || tag == DomainTag.KW_EPS || tag == DomainTag.KW_AXIOM || tag == DomainTag.KW_TERMINAL||tag == DomainTag.KW_EQ);
            keyword = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {keyword}";
        }
    }
}
