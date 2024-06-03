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

        public SpecialToken(DomainTag tag, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.SC ||
                tag == DomainTag.COLON || 
                tag == DomainTag.EQ || 
                tag == DomainTag.LB ||
                tag == DomainTag.RB || 
                tag == DomainTag.SLB ||
                tag == DomainTag.SRB ||
                tag == DomainTag.ARROW || 
                tag == DomainTag.TWO_DOTS ||
                tag == DomainTag.COMMA
                );
        }

        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}
