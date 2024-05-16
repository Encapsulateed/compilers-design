using lab2._4.src.Lexer;
using lab2._4.src.Tokens;

namespace lab2._4
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var lines = File.ReadAllLines("input.txt");
            string prg = "";

            foreach (var line in lines)
            {
                prg += line + '\n';
            }

            var cp = new Compiler();
            var sc = new Scanner(prg, cp);

            //Console.WriteLine(prg);


            var tok = sc.NextToken();
            while (tok.Tag != DomainTag.EOF)
            {
                var st = tok.ToString();
                if (tok.Tag == DomainTag.IDENT)
                {
                    st += $" IDENT NAME: {cp.GetName(((IdentToken)tok).Code)}";
                }
                Console.WriteLine(st);
                tok = sc.NextToken();

                if (tok.Tag == DomainTag.EOF)
                {
                    Console.WriteLine(tok.ToString());
                }

            }
            Console.WriteLine();
            cp.OutPutMessages();
        }
    }
}