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
        KW_TYPE,
        KW_CONST,
        KW_ARRAY,
        KW_FILE,
        KW_SET,
        KW_RECORD,
        KW_OF,
        KW_PAKED,
        KW_CASE,
        KW_END,
        EQ,
        SC,
        COMMA,
        LB,
        RB,
        SLB,
        SRB,
        ARROW,
        TWO_DOTS,
        COLON,
        IDENT,
        INTEGER,
        REAL,
        INVALID,
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
