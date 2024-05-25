using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using lab2._3.src;
using lab3._1.src.Grammar;

namespace lab3._1
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

            var parser = new Parser();

            var tree = parser.parse(sc);
            // tree.Print("");

            Grammar g = new Grammar(tree);
            g.Print();
        }
    }
}