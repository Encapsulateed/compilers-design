using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
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
