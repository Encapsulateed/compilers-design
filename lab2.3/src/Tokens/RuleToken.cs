using lab2._3.src.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src.Tokens
{
    internal class RuleToken : Token
    {
        public readonly string rule;

        public RuleToken(string rule, Position statring, Position following) : base(DomainTag.RULE, statring, following)
        {
            this.rule = rule;   
        }
        public override string ToString()
        {
            return $"{Tag} {Coords} {rule}";
        }
    }
}
