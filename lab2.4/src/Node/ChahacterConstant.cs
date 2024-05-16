using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class ChahacterConstant : Constant , INode
    {
        string value { get; set; } = string.Empty;

        public ChahacterConstant(string value) => this.value = value;

        public override void PrintNode()
        {
            Console.WriteLine($"Char Constant: {value}");
        }
    }
}
