using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class UnsignedInt : UnsignedNumber, INode
    {
        int value { get; set; }

        public UnsignedInt(int value) => this.value = value;

        public override void PrintNode()
        {
            Console.WriteLine($"Unsigned Integer: {value}");
        }
    }
}
