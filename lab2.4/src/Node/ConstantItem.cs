using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class ConstantItem : INode
    {
        public List<Filed> fileds { get;private set; } = new List<Filed>();
        public List<Constant> constants { get; private set; } = new List<Constant>();

        public ConstantItem(List<Filed> filed, List<Constant> constants)
        {
            this.fileds = filed;
            this.constants = constants;
        }

        public void PrintNode()
        {
            Console.WriteLine("Constant Item:");

            Console.WriteLine("Fileds: ");
            fileds.ForEach(f => f.PrintNode());

            Console.WriteLine("Constants: ");
            constants.ForEach(c => c.PrintNode());
        }
    }
}
