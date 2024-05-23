using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
                    case '\'':
                        word += (char)cur.Cp;
                        cur++;
                        if (cur.isTerm)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if(cur.Cp != '\'' || cur.Cp == -1)
                        {
                            compiler.AddMessage(isErr: true, cur, "term must end with ' symbol!");   
                            return new Erorr(DomainTag.ERROR,prev_cur, cur.clone());    
                        }

                        word += (char)cur.Cp;
                        cur++;

                        return new TermToken(word,prev_cur,cur.clone());
                    case ';':
                        cur++;
                        return new SpecialToken(DomainTag.SC,";", prev_cur, cur.clone());
                    case '\n':
                        cur++;
                        return new SpecialToken(DomainTag.NL, "\\n", prev_cur, cur.clone());
                    case ',':
                        cur++;
                        return new SpecialToken(DomainTag.COMMA, ",", prev_cur, cur.clone());
                    case ':':
                        cur++;
                        cur++;
                        cur++;
                        return new KeyWordToken(DomainTag.KW_EQ, "::=", prev_cur, cur.clone());
                    case '|':
                        cur++;
                        return new SpecialToken(DomainTag.OR, "|", prev_cur, cur.clone());
                    case '#':
                        cur++;
                      
                        while (!cur.IsNewLine)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        Comments.Add(new Fragment(prev_cur, cur.clone()));
                        break;                        
                    default:
       
             
                        if (cur.Cp != -1)
                        {

                            if (cur.isNonTerm)
                            {
                                word += (char)cur.Cp;
                                cur++;
                                while (cur.IsDecimalDigit || cur.isNonTerm)
                                {
                                    word += (char)cur.Cp;
                                    cur++;
                                }

                                return new NonTerm(word, prev_cur, cur.clone());

                            }
                            else
                            {
                                if (cur.IsLetter)
                                {
                                    word += (char)cur.Cp;

                                    cur++;
                                    while (cur.IsLetter || cur.Cp == '-')
                                    {
                                        word += (char)cur.Cp;
                                        cur++;
                                    }

                                    switch (word)
                                    {
                                        case "terminal":
                                            return new KeyWordToken(DomainTag.KW_TERMINAL, word, prev_cur, cur.clone());
                                        case "non-terminal":
                                            return new KeyWordToken(DomainTag.KW_NONTERMINAL, word, prev_cur, cur.clone());
                                        case "axiom":
                                            return new KeyWordToken(DomainTag.KW_AXIOM, word, prev_cur, cur.clone());
                                        case "epsilon":
                                            return new KeyWordToken(DomainTag.KW_EPS, word, prev_cur, cur.clone());

                                    }
                                }


                            }
                        }
                        break;
                }
                cur++;
            }

            return new EOFToken("", cur, cur);
        }


    }
}
