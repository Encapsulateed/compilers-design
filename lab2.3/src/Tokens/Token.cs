using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    enum DomainTag
    {
        TERM,
        NON_TERM,
        KW_AXIOM,
        KW_EPS,
        KW_NONTERMINAL,
        KW_TERMINAL,
        KW_EQ,
        NL,
        ERROR,
        SC,
        OR,
        COMMA,
        EOF
    }

   

    internal abstract class Token
    {
        public readonly DomainTag Tag;
        public readonly Fragment Coords;

        protected Token(DomainTag tag, Position statring, Position following)
        {
            Tag = tag;
            Coords = new Fragment(statring, following);
        }


    }
}
