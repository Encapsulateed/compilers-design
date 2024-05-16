using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class ProgramNode : INode
    {
        public List<Block> blocks { get; private set; } = new List<Block>();

        public ProgramNode(List<Block> blocks) => this.blocks = blocks; 

        public void PrintNode()
        {
            Console.WriteLine($"Program: ");
            blocks.ForEach(x => x.PrintNode());
        }
    }
}
