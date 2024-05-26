using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Lexer;

namespace lab2._3.src.Tokens
{
    enum DomainTag
    {
        NUMBER,
        PLUS,
        MUL,
        LB,
        RB,
        EOF
    }

    

    internal abstract class Token
    {
        public readonly DomainTag Tag;
        public readonly Fragment Coords;

        protected Token(DomainTag tag, Position statring, Position following)
        {
            Tag = tag;
            Coords = new Fragment(statring, following);
        }

        public static Dictionary<DomainTag,string> tag_to_str  = new Dictionary<DomainTag, string>() 
        {
            {DomainTag.NUMBER, "n" },
            {DomainTag.PLUS, "+" },
            {DomainTag.MUL, "*" },
            {DomainTag.LB, "(" },
            {DomainTag.RB, ")" },
            {DomainTag.EOF, "$" },
        };
    }
}
