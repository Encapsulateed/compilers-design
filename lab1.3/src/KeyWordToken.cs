using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class KeyWordToken : Token
    {
        public readonly string keyword;

        public KeyWordToken(DomainTag tag, string value, Position starting, Position following) : base(tag, starting, following)
        {
            Debug.Assert(tag == DomainTag.GOSUB_KEYWORD || tag == DomainTag.GOTO_KEYWORD || tag == DomainTag.PRINT_KEYWORD);
            keyword = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {keyword}";
        }
    }
}
