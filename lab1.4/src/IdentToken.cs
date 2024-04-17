using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class IdentToken : Token
    {
        public readonly int Code;
        public string Name = string.Empty;
        public IdentToken(DomainTag tag, int code, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.IDENT || tag == DomainTag.S_IDENT || tag == DomainTag.S2_IDENT || tag == DomainTag.U_IDENT 
                || tag == DomainTag.N_IDENT || tag == DomainTag.E_IDENT || tag == DomainTag.E2_IDENT || tag == DomainTag.S2_IDENT);
            Code = code;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {Code}";
        }
    }
}
