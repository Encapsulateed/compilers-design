using lab1._3.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._4.src
{
    internal class StringToken : Token
    {
        public readonly string Value;

        public StringToken(string value, Position starting, Position following) : base(DomainTag.STRING, starting, following)
        {
            Value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {Value}";
        }
    }
}
