using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class EOFToken : Token
    {
        public readonly string value;

        public EOFToken(string value, Position statring, Position following) : base(DomainTag.EOF, statring, following)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}
