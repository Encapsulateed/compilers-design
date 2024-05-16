using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace lab2._4.src.Node
{
    internal class ConstantBlock : Block, INode
    {
        public List<ConstDef> const_defs { get; private set; } = new List<ConstDef>();

        public override void PrintNode()
        {
            Console.WriteLine("Constant Block: ");
            const_defs.ForEach(x => x.PrintNode());
        }
    }
}
