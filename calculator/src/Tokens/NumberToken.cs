using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.src.Tokens
{
    internal class NumberToken : Token
    {
        public int value;
        public NumberToken( int value,Position statring, Position following) : base(DomainTag.NUMBER, statring, following)
        {
            this.value = value; 
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {value}";
        }
    }
}
