using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class Comment : Token
    {
        public readonly string comment;

        public Comment(string value, Position starting, Position following) : base(DomainTag.COMMENT, starting, following)
        {
            comment = value;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {comment}";
        }
    }
}
