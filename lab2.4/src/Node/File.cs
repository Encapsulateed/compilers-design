using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class File : Type, INode
    {
        Type type;

        public File(Type simpleType)
        {
            this.type = simpleType;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Type:");
            type.PrintNode();
        }
    }
}
