using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class Array : Type, INode
    {
        List<SimpleType> simpleTypes = new List<SimpleType>();
        Type array_type;

        public Array(List<SimpleType> simpleTypes, Type array_type)
        {
            this.simpleTypes = simpleTypes;
            this.array_type = array_type;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Array:");
            Console.Write("array_type: ");
            array_type.PrintNode();

            Console.WriteLine("Simple Types:");
            simpleTypes.ForEach(x => x.PrintNode());
        }
    }
}
