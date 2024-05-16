using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class Ident : INode
    {
        string identifire = string.Empty;

        public Ident(string identifire)
        {
            this.identifire = identifire;
        }   

        public void PrintNode()
        {
            Console.WriteLine($"Ident: {identifire}");
        }
    }
}
