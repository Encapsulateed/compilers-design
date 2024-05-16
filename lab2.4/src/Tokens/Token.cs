using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._4.src.Lexer;

namespace lab2._4.src.Tokens
{
    enum DomainTag
    {
        IDENT,
        NUMBER,
        HEX_NUMBER,
        PRINT_KEYWORD,
        GOTO_KEYWORD,
        GOSUB_KEYWORD,
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
