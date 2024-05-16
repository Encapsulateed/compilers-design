using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class UnsignedFloat : UnsignedNumber, INode
    {
        float value { get; set; } = 0.0f;

        public UnsignedFloat(float value) => this.value = value;

        public override void PrintNode()
        {
            Console.WriteLine($"Unsigned Float: {value}");
        }
    }
}
