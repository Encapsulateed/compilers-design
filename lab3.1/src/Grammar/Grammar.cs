using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
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
             void Traverse(INode tree)
            {
                if (tree is not InnerNode)
                    return;

                var node = (InnerNode)tree;

                switch (node.nterm)
                {
                    case "program":
                        if (node.children.Count != 3)
                            throw new InvalidNonTermLenght(node.nterm);
                        
                        node.children.ForEach(Traverse);

                        break;

                    case "rules":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;

                    case "decl":
                        if (node.children.Count != 6)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;
                    case "axiom":
                        if(node.children.Count != 3)
                            throw new InvalidNonTermLenght(node.nterm);
                        if (node.children[1] is not Leaf)
                            throw new InvalidTree(node.nterm);
                        if (!axiom.isEmpty)
                            throw new TooManyAxiomException();

                        var nt = ((NonTerm)((Leaf)node.children[1]).tok).nterm;
                        axiom = new Axiom(nt);
                    
                        break;

                    case "rule":
                        if(node.children.Count != 4)
                            throw new InvalidNonTermLenght(node.nterm);
                        break;

                    case "non_terms":
                        break;

                    case "non_term_tail":
                        break;

                    case "terms":
                        break;

                    case "term_tail":
                        break;

                    case "alts":
                        break;

                    case "alt_tail":
                        break;

                    case "symbols":
                        break;
                }
            }
        }
    }



    readonly struct Rule
    {
        public readonly string left;
        public readonly List<string> right;

        public Rule(string left, List<string> right)
        {
            this.left = left;
            this.right = right;
        }
    }


    readonly struct Axiom
    {
        readonly string non_term = string.Empty;

        public Axiom(string non_term) => this.non_term = non_term;  

        public bool isEmpty { get { return non_term == string.Empty; } }
    }
}
