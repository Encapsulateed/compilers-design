using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using lab2._3.src.Tokens;
using lab2._4.src.Tokens;

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
                    case ':':
                        cur++;

                        return new SpecialToken(DomainTag.COLON, prev_cur, cur.clone());

                    case ';':
                        cur++;

                        return new SpecialToken(DomainTag.SC, prev_cur, cur.clone());

                    case ',':
                        cur++;

                        return new SpecialToken(DomainTag.COMMA, prev_cur, cur.clone());

                    case '(':
                        cur++;

                        return new SpecialToken(DomainTag.LB, prev_cur, cur.clone());

                    case ')':
                        cur++;

                        return new SpecialToken(DomainTag.RB, prev_cur, cur.clone());

                    case '[':
                        cur++;

                        return new SpecialToken(DomainTag.SLB, prev_cur, cur.clone());

                    case ']':
                        cur++;

                        return new SpecialToken(DomainTag.SRB, prev_cur, cur.clone());

                    case '^':
                        cur++;

                        return new SpecialToken(DomainTag.ARROW, prev_cur, cur.clone());

                    case '.':
                        cur++;
                        cur++;

                        return new SpecialToken(DomainTag.TWO_DOTS, prev_cur, cur.clone());
                    case '=':
                        cur++;

                        return new SpecialToken(DomainTag.EQ, prev_cur, cur.clone());
                    default:
                        if (cur.IsLetter)
                        {
                            do
                            {
                                word += (char)cur.Cp;
                                cur++;
                            } while (cur.IsLetterOrDigit);

                            switch (word.ToLower())
                            {
                                case "type":
                                    return new KeyWordToken(DomainTag.KW_TYPE, prev_cur, cur.clone());
                                case "array":
                                    return new KeyWordToken(DomainTag.KW_ARRAY, prev_cur, cur.clone());
                                case "set":
                                    return new KeyWordToken(DomainTag.KW_SET, prev_cur, cur.clone());
                                case "file":
                                    return new KeyWordToken(DomainTag.KW_FILE, prev_cur, cur.clone());
                                case "const":
                                    return new KeyWordToken(DomainTag.KW_CONST, prev_cur, cur.clone());
                                case "record":
                                    return new KeyWordToken(DomainTag.KW_RECORD, prev_cur, cur.clone());
                                case "of":
                                    return new KeyWordToken(DomainTag.KW_OF, prev_cur, cur.clone());
                                case "paked":
                                    return new KeyWordToken(DomainTag.KW_PAKED, prev_cur, cur.clone());
                                case "case":
                                    return new KeyWordToken(DomainTag.KW_CASE, prev_cur, cur.clone());
                                case "end":
                                    return new KeyWordToken(DomainTag.KW_END, prev_cur, cur.clone());

                                default:
                                    return new IdentToken(word, prev_cur, cur.clone());

                            }


                        }

                        else if (cur.IsDecimalDigit)
                        {
                            double val = cur.Cp - '0';
                            cur++;

                            while (cur.IsDecimalDigit)
                            {
                                val = checked(val * 10 + cur.Cp - '0');
                                cur++;

                            }

                            return new NumberToken(DomainTag.INTEGER, val, prev_cur, cur.clone());
                        }
                        else
                        {
                            return new InvalidToken(prev_cur, cur.clone());

                        }


                }
                cur++;
            }

            return new EOFToken(cur, cur);
        }


    }
}
