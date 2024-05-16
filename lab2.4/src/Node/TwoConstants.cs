using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class TwoConstants : SimpleType, INode
    {
        Constant first;
        Constant second;

        public TwoConstants(Constant first, Constant second)
        {
            this.first = first;
            this.second = second;
        }

        public override void PrintNode()
        {
            Console.WriteLine($"Two Constants:\nfirst:");
            first.PrintNode();

            Console.WriteLine("second:");
            second.PrintNode(); 
        }
    }
}
