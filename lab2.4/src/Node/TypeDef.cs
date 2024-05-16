using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class TypeDef : INode
    {
        Ident identifire;
        Type type;

        public TypeDef(Ident identifire, Type type)
        {
            this.identifire = identifire;
            this.type = type;
        }

        public void PrintNode()
        {
            Console.WriteLine("TypeDef: ");
            identifire.PrintNode();
            type.PrintNode();
        }
    }
}
