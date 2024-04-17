using lab1._3.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1._4.src
{
    internal class FSM
    {
        public enum States
        {
            START = 1,
            SPACE,
            NUMBER ,
            LB ,
            BRACKET_OP,
            STRING,
            SET,
            UNSET,
            st_sym,
            u_IDENT,
            n_IDENT,
            s_IDENT,
            e_IDENT,
            s2_IDENT,
            e2_IDENT,
            IDENT


        }

        private int[,] automata = new int[16, 12];




        public FSM()
        {
            automata = new int[,]
             {
               //                  Li  Nu  Sp  u  n  s  e  t   (   ), \n \ANY
               /*0 START*/         {14, 3, 2,  7, -1, 11, -1, -1, 15,-1, -1, -1 },
               /*1 STRING*/        {-1, -1,  -1,  -1,  -1,  -1,  -1, -1, -1,-1, -1, -1 },
               /*2 SPACE*/         {-1, -1,  2,  -1,  -1,  -1,  -1,  -1,  -1,-1, 1, -1 },
               /*3 NUMBER*/        {-1, 3,  -1,  -1,  -1,  -1,  -1,  -1,  -1,-1, -1, -1 },
               /*4 BRACKET_OP*/    {-1, -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,-1, -1, -1 },
               /*5 SET*/           {14, -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,-1, -1, -1 },
               /*6 UNSET*/         {14, -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,-1, -1, -1 },
               /*7 u_IDENT*/       {14, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 },
               /*8 n_IDENT*/       {14, -1,  -1,  -1,  -1,  9,  -1,  -1,  -1,-1, -1, -1 },
               /*9 s_IDENT*/       {14, -1,  -1,  -1,  -1,  -1,  10,  -1,  -1,-1, -1, -1 },
               /*10 e_IDENT*/      {14, -1,  -1,  -1,  -1,  -1,  -1,  6,  -1,-1, -1, -1 },
               /*11 s2_IDENT*/     {14, -1,  -1,  -1,  -1,  -1,  12,  -1,  -1,-1, -1, -1 },
               /*12 e2_IDENT*/     {14, -1,  -1,  -1,  -1,  -1,  -1,  5,  -1,-1, -1, -1 },
               /*13 st_sym*/       {13, 13,  13,  13,  13,  13,  13,  13, -1, 1, -1, 13 },
               /*14 IDENT*/        {14, 14,  -1,  14,  14,  14, 14, 14,  -1, -1, -1, -1 },
               /*15 LB*/           {13, 13,  13,  13,  13,  13,  13, 13,  -1, 4, -1, 13 },
             };
        }

        public int NextState(int curState, Position pos)
        {
            switch (pos.Cp)
            {
                case 'u':
                    return automata[curState, 3];
                case 'n':
                    return automata[curState, 4];
                case 's':
                    return automata[curState, 5];
                case 'e':
                    return automata[curState, 6];
                case 't':
                    return automata[curState, 7];
                case '(':
                    return automata[curState, 8];
                case ')':
                    return automata[curState, 9];
                default:
                    if (pos.IsLetter)
                        return automata[curState, 0];
                    if (pos.IsDecimalDigit)
                        return automata[curState, 1];
                    if (pos.IsNewLine)
                        return automata[curState, 10];
                    if (pos.IsWhiteSpace)
                        return automata[curState, 2];
          
                    return automata[curState, 11];

            }
        }
    }
}
