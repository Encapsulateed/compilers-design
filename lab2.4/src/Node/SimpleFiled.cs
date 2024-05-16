using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class SimpleFiled : Filed, INode
    {
        Type type;
        List<Ident> idents = new List<Ident>();    

        public SimpleFiled(Type type, List<Ident> idents)
        {
            this.type = type;
            this.idents = idents;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Simple Filed: ");

            type.PrintNode();
            Console.WriteLine("Idents: ");
            idents.ForEach(x => Console.WriteLine(x));  
        }

    }

}
