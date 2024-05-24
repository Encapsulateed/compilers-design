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
        public string nterm { get; private set; }
        public List<INode> children { get; private set; }

        public InnerNode(string nterm)
        {
            this.nterm = nterm;
            children = new List<INode>();
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
        public Token tok { get; private set; }

        public Leaf(Token tok)
        {
            this.tok = tok;
        }

        public void Print(string indent)
        {
            string val = string.Empty;
            if (tok.Tag == DomainTag.TERM)
                val = ((TermToken)tok).term;
            if (tok.Tag == DomainTag.NON_TERM)
                val = ((NonTerm)tok).nterm;

            Console.WriteLine($"{indent}Лист:  {tok.Tag} {val}");

        }
    }

}

