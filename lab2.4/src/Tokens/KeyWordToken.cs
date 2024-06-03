using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    internal class KeyWordToken : Token
    {

        public KeyWordToken(DomainTag tag, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.KW_ARRAY ||
                         tag == DomainTag.KW_TYPE ||
                         tag == DomainTag.KW_CONST ||
                         tag == DomainTag.KW_FILE ||
                         tag == DomainTag.KW_SET ||
                         tag == DomainTag.KW_RECORD ||
                         tag == DomainTag.KW_OF ||
                         tag == DomainTag.KW_PAKED ||
                         tag == DomainTag.KW_CASE ||
                         tag == DomainTag.KW_END);
        }

        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}
