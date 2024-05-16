using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class CaseFiled : Filed, INode
    {
        Ident ident;
        TypeIdent type_ident;
        List<Constant> constants = new List<Constant>();

        public CaseFiled(Ident ident, TypeIdent type_ident, List<Constant> constants)
        {
            this.ident = ident;
            this.type_ident = type_ident;
            this.constants = constants;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Case Filed: ");

            ident.PrintNode();
            type_ident.PrintNode();

            Console.WriteLine("Constants: ");
            constants.ForEach(c => c.PrintNode());
        }
    }
}
