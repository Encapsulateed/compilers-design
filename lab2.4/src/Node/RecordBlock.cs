using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class RecordBlock : Block, INode
    {
        public List<TypeDef> type_defs { get; private set; } = new List<TypeDef>();

        public RecordBlock(List<TypeDef> type_defs) => this.type_defs = type_defs;  

        public override void PrintNode()
        {
            Console.WriteLine("Record Block: ");
            type_defs.ForEach(x => x.PrintNode());
        }
    }
}
