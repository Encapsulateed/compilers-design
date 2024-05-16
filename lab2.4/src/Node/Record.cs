using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class Record : Type, INode
    {
        List<Filed> fileds = new List<Filed>();

        public Record(List<Filed> fileds)
        {
            this.fileds = fileds;
        }

        public override void PrintNode()
        {
            Console.WriteLine("Fileds;");
            fileds.ForEach(filed => filed.PrintNode());
        }
    }
}
