using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class ConstDef : INode
    {
        Ident identifire;
        Constant constant;

        public ConstDef(Ident identifire, Constant constant)
        {
            this.identifire = identifire;
            this.constant = constant;
        }

        public void PrintNode()
        {
            Console.WriteLine("Ident:");
            identifire.PrintNode();

            Console.WriteLine("Constant:");
            constant.PrintNode();
        }
    }
}
