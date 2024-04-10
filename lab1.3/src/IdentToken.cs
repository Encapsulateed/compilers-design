using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class IdentToken : Token
    {
        public readonly int Code;
        public string Name;
        public IdentToken(int code, Position statring, Position following) : base(DomainTag.IDENT, statring, following)
        {
            Code = code;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {Code}";
        }
    }
}
