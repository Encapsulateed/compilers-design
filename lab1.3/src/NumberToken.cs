using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class NumberToken : Token
    {
        public readonly long Value;

        public NumberToken(long value, Position starting, Position following) : base(DomainTag.NUMBER, starting, following)
        {
            Value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {Value}";
        }

    }
}
