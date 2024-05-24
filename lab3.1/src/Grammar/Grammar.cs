using lab2._3.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Grammar
{
    internal class Grammar
    {
        public HashSet<string>? terms { get; private set; }
        public HashSet<string>? non_terms { get; private set; }
        public List<Rule>? rules { get; private set; }
        public Axiom axiom { get; private set; }

        public Grammar(INode tree) 
        {
            
        }
    }



    readonly struct Rule
    {
        public readonly string left;
        public readonly List<string> right;

        // left -> right 
        public Rule(string left, List<string> right)
        {
            this.left = left;
            this.right = right;
        }
    }



    readonly struct Axiom
    {
        readonly string non_term;

        public Axiom(string non_term) => this.non_term = non_term;  
    }
}
