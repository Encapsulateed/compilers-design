using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    enum DomainTag
    {
        START,
        STRING,
        SPACE,
        NUMBER,
        BRACKET_OP,
        SET,
        UNSET,
        U_IDENT,
        N_IDENT,
        S_IDENT,
        E_IDENT,
        S2_IDENT,
        E2_IDENT,
        st_sym,
        IDENT,
        LB,
        EOF,
        INVALID
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
