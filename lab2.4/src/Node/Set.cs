using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class Set : Type, INode
    {
        SimpleType simpleType;

        public Set(SimpleType simpleType)
        {
            this.simpleType = simpleType;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Simple Type:");
            simpleType.PrintNode();
        }
    }
}
