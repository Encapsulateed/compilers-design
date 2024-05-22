using lab2._3.src.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._3.src
{
    public interface INode
    {

        void Print(string indent);
    }

    struct StackNode { 
       public InnerNode node;
       public string value;


    }

    class InnerNode : INode
    {
        private string nterm;
        private List<INode> children;

        public InnerNode(string nterm)
        {
            this.nterm = nterm;
            this.children = new List<INode>();
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent} Внутренний узел: {nterm}");
            foreach (var child in children)
            {
                child.Print(indent + "\t");
            }
        }

        public void AddChild(INode child)
        {
            children.Add(child);
        }
    }

    class Leaf : INode
    {
        private Token tok;

        public Leaf(Token tok)
        {
            this.tok = tok;
        }

        public void Print(string indent)
        {
            if (tok.Tag == DomainTag.TERM || tok.Tag == DomainTag.NON_TERM)
            {
                Console.WriteLine($"{indent}Лист: {tok.Tag} - {tok}");
            }
            else
            {
                Console.WriteLine($"{indent} Лист: {tok}");
            }
        }
    }

}

