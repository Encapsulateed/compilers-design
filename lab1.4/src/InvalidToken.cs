using lab1._3.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._4.src
{
    internal class InvalidToken : Token
    {
        public InvalidToken( Position starting, Position following) : base(DomainTag.INVALID, starting, following)
        {
           
        }
        public override string ToString()
        {
            return $"{Tag} {Coords}";
        }
    }
}
