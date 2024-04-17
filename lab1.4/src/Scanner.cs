using lab1._4.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace lab1._3.src
{
    internal class Scanner
    {
        private Compiler compiler;
        private Position cur;
        public List<Fragment> Comments { get; private set; }

        private FSM _fsm;
        public Scanner(string program, Compiler compiler)
        {
            this.compiler = compiler;
            cur = new Position(program);
            Comments = new List<Fragment>();
            _fsm = new FSM();
        }

        public Token NextToken()
        {


            var prev_cur = cur.clone();
            while (cur.Cp != -1)
            {

                while (cur.IsWhiteSpace)
                    cur++;

                string word = "";
                var curr_state = 0;
                var prev_state = 0;
                prev_cur = cur.clone();


                while (curr_state != -1 && cur.Cp != -1)
                {
                    prev_state = curr_state;
                    curr_state = _fsm.NextState(curr_state, cur);

                    if (curr_state != -1)
                    {
                        word += (char)cur.Cp;
                        cur++;
                    }
                }

                Console.WriteLine(prev_state);
                Console.WriteLine(curr_state);

                if (curr_state != -1)
                {
                    return Select(curr_state, word, prev_cur, cur);
                }
                if (prev_state != 0 && prev_state != 2 && prev_state != 13 && prev_state != 15) 
                {
                    return Select(prev_state, word, prev_cur, cur);
                }
                else
                {
                    while (cur.Cp != -1 && cur.IsWhiteSpace == false)
                    {
                        cur++;
                    }

                    compiler.AddMessage(true, prev_cur.clone(), "syntax erorr");
                    return new InvalidToken(prev_cur, cur.clone());

                }

            }

            return new EOFToken("", cur, cur);
        }


        Token Select(int state, string word, Position s, Position e)
        {
            var t = (DomainTag)state;
            Console.WriteLine(t);
            if (t == DomainTag.IDENT || t == DomainTag.S_IDENT || t == DomainTag.S2_IDENT || t == DomainTag.U_IDENT
                || t == DomainTag.N_IDENT || t == DomainTag.E_IDENT || t == DomainTag.E2_IDENT || t == DomainTag.S2_IDENT)
            {
                return new IdentToken(t, compiler.AddName(word), s, e);
            }
            else if (t == DomainTag.SET || t == DomainTag.UNSET || t == DomainTag.BRACKET_OP)
            {
                return new KeyWordToken(t, word, s, e);
            }
            else if (t == DomainTag.NUMBER)
            {
                return new NumberToken(Convert.ToInt64(word), s, e);
            }
            else if (t == DomainTag.STRING)
            {
                return new StringToken(word, s, e);
            }
            else if (t == DomainTag.EOF || t == DomainTag.START) 
            {
                return new EOFToken("", cur, cur);

            }

            return new InvalidToken(s,e);

        }
    }

   
}
