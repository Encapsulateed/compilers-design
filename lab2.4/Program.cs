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


            Parser parser = new Parser(sc);

            lab2._4.src.Nodes.Program syntaxTree = parser.Parse();

            syntaxTree.Print("");
        }
    }
}
