﻿using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Tokens
{
    internal class IdentToken : Token
    {
        public string Name;
        public IdentToken(string name, Position statring, Position following) : base(DomainTag.IDENT, statring, following)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Tag} {Coords} {Name}";
        }
    }
}
