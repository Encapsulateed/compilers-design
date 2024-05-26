using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using calculator.src.Tokens;
using lab2._3.src.Tokens;

namespace lab2._3.src.Lexer
{
    internal class Scanner
    {
        private Compiler compiler;
        private Position cur;
        public List<Fragment> Comments { get; private set; }

        public Scanner(string program, Compiler compiler)
        {
            this.compiler = compiler;
            cur = new Position(program);
            Comments = new List<Fragment>();
        }

        public Token NextToken()
        {
            while (cur.Cp != -1)
            {
                string word = "";

                while (cur.IsWhiteSpace)
                    cur++;
                var prev_cur = cur.clone();
                switch (cur.Cp)
                {
                    case '+':
                        cur++;
                        return new TokenPlus(prev_cur, cur.clone());
                    case '*':
                        cur++;
                        return new TokenMul(prev_cur, cur.clone());
                    case '(':
                        cur++;
                        return new LeftBracketToken(prev_cur, cur.clone());
                    case ')':
                        cur++;
                        return new RightBracketToken(prev_cur, cur.clone());
                    default:
                        if (cur.IsDecimalDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;

                            while (cur.IsDecimalDigit)
                            {
                                word += (char)cur.Cp;
                                cur++;
                            }
                            int val = Convert.ToInt32(word);

                            return new NumberToken(val, prev_cur, cur.clone());
                        }
                        break;

                }
                cur++;
            }

            return new EOFToken(cur, cur);
        }


    }
}
