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
        TYPE_IDENT,
        CONSTANT_IDENT,
        POINTER_IDENT,
        INT_CONSTANT,
        REAL_CONSTANT,
        STRING_CONSTANT,
        KEYWORD,
        SPEC_TOKEN,
        ERROR,
        EOF,

    }

    enum KeyWord
    {
        TYPE,
        CASE,
        END,
        ARRAY,
        FILE,
        SET,
        RECORD, 
        OF,
        CONST,
        INTEGER,
        REAL,
        BOOLEAN,
        CHAR,

    }


    enum SPEC_TOKENS
    {
        LB,
        RB,
        SLB,
        SRB,
        COMMA,
        COLON,
        SEMICOLON,
        OF,
        CONST,
        INTEGER,
        REAL,
        BOOLEAN,
        CHAR,

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
