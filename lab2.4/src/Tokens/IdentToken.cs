using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._4.src.Lexer;

namespace lab2._4.src.Tokens
{
    internal class IdentToken : Token
    {
        public readonly int Code;
        public string Name = string.Empty;

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
