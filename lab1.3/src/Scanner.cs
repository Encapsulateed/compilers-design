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

                    case 'p':

                        while (cur.IsLetterOrDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if (word.ToLower() == "print")
                        {
                            return new KeyWordToken(DomainTag.PRINT_KEYWORD, word, prev_cur, cur.clone());
                        }
                        else
                        {
                           
                          return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());
                        }
                        break;
                    case 'g':
                        while (cur.IsLetterOrDigit)
                        {
                            word += (char)cur.Cp;
                            cur++;
                        }
                        if (word.ToLower() == "goto")
                        {
                            return new KeyWordToken(DomainTag.GOTO_KEYWORD, word, prev_cur, cur.clone());

                        }
                        else if (word.ToLower() == "gosub")
                        {
                            return new KeyWordToken(DomainTag.GOSUB_KEYWORD, word, prev_cur, cur.clone());

                        }
                        else
                        {

                            return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());

                            break;
                        }
                    case '&':
                        // пропускаем & H
                        cur++;
                        cur++;
                        do
                        {
                            word += (char)cur.Cp;
                            cur++;

                        } while (cur.IsLetterOrDigit);
             
                        return new NumberToken(Convert.ToInt64(word.ToUpper(),16), prev_cur, cur.clone());
                    default:
                        if (cur.IsLetter)
                        {
                            do
                            {
                                word += (char)cur.Cp;
                                cur++;
                            } while (cur.IsLetterOrDigit);



                            return new IdentToken(compiler.AddName(word), prev_cur, cur.clone());

                        }
                        else if (cur.IsDecimalDigit)
                        {
                            int val = cur.Cp - '0';
                            //пропускаем первую букву
                            cur++;
                            try
                            {
                                while (cur.IsDecimalDigit)
                                {
                                    val = checked(val * 10 + cur.Cp - '0');
                                    cur++;

                                }
                            }
                            catch (OverflowException)
                            {
                                compiler.AddMessage(true, prev_cur, "константа слишком большая");
                                while (cur.IsDecimalDigit) cur++;
                            }

                            if (cur.IsLetter)
                            {
                                compiler.AddMessage(true, prev_cur, "нужен разделитель");

                            }

                            return new NumberToken(val, prev_cur, cur.clone());
                        }
                        else
                        {
                            while (cur.Cp is not '\n' && cur.Cp is not ' ' && cur.Cp is not -1)
                                cur++;
                        }
                        break;
                }
                cur++;
            }

            return new EOFToken("", cur, cur);
        }


    }
}
