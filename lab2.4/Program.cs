using lab2._3.src.Lexer;
using lab2._3.src.Parser;
using lab2._3.src.Tokens;
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



            var tok = sc.NextToken();
            while (tok.Tag != DomainTag.EOF)
            {
                var st = tok.ToString();

                //Console.WriteLine(st);
                tok = sc.NextToken();

                if (tok.Tag == DomainTag.EOF)
                {
                    Console.WriteLine(tok.ToString());
                }

            }

             cp = new Compiler();
             sc = new Scanner(prg, cp);

            Parser parser = new Parser(sc);

            // Step 3: Parse the input to get the syntax tree
            lab2._4.src.Nodes.Program syntaxTree = parser.Parse();

            // Step 4: Print the syntax tree
            syntaxTree.Print("");


            Console.WriteLine();
            cp.OutPutMessages();
        }
    }
}
