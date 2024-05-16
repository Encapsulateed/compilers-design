using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Lexer
{
    internal class Fragment
    {
        public readonly Position Starting, Following;

        public Fragment(Position starting, Position following)
        {
            Starting = starting;
            Following = following;
        }
        public override string ToString()
        {
            return $"{Starting}-{Following}";
        }
    }
}
