using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Grammar
{
    internal class Grammar
    {
        public HashSet<string> terms { get; private set; } = new HashSet<string>();
        public HashSet<string> non_terms { get; private set; } = new HashSet<string>();
        public Dictionary<string,List<List<string>>>  rules { get; private set; } = new Dictionary<string, List<List<string>>>();
        public Axiom axiom { get; private set; }

        private string CurrentNonTerm { get; set; }
        private int CurrentListIndex { get 
            {
                if (rules.ContainsKey(CurrentNonTerm))
                    return -1;
                
                return rules[CurrentNonTerm].Count - 1;
                
            } }

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
                        if (node.children.Count != 2 || node.children.Count != 0)
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

                        var rule_left = ((NonTerm)node.children[0]).nterm;

                        if(!non_terms.Contains(rule_left))
                            throw new NoSuchNonTerminalException(rule_left);


                        CurrentNonTerm = rule_left;

                        rules[rule_left] = new List<List<string>>
                        {
                            new List<string>()
                        };

                        node.children.ForEach(Traverse);

                        break;

                    case "non_terms":
                        if (node.children.Count != 2 )
                            throw new InvalidNonTermLenght(node.nterm);
                        
                        if (node.children.Count != 0)
                        {
                            var new_nt = ((NonTerm)node.children[0]).nterm;
                            non_terms.Add(new_nt);
                        }
                       

                        node.children.ForEach(Traverse);

                        break;

                    case "non_term_tail":
                        if (node.children.Count != 2 || node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)                       
                            node.children.ForEach(Traverse);

                        break;

                    case "terms":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        var new_term = ((TermToken)node.children[0]).term;
                        terms.Add(new_term);

                        node.children.ForEach(Traverse);

                        break;

                    case "term_tail":
                        if (node.children.Count != 2 || node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                            node.children.ForEach(Traverse);

                        break;

                    case "alts":
                        if (node.children.Count != 2 )
                            throw new InvalidNonTermLenght(node.nterm);
                        node.children.ForEach(Traverse);

                        break;

                    case "alt_tail":
                        if (node.children.Count != 2 || node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                        {
                            node.children.ForEach(Traverse);
                            rules[CurrentNonTerm].Add(new List<string>());
                        }
                        break;

                    case "symbols":

                        if (node.children.Count == 2)
                        {
                            var leaf_value = ((Leaf)node.children[0]).tok;
                            string leaf = string.Empty;

                            if(leaf_value is NonTerm)
                            {
                                leaf = ((NonTerm)leaf_value).nterm;
                                if(!non_terms.Contains(leaf))
                                    throw new NoSuchNonTerminalException(leaf);
                            }
                            else if(leaf_value is TermToken)
                            {
                                leaf = ((TermToken)leaf_value).term;
                                if (!terms.Contains(leaf))
                                    throw new NoSuchTerminalException(leaf);
                            }

                            rules[CurrentNonTerm][CurrentListIndex].Add("asd");
                            node.children.ForEach(Traverse);
                        }
                        else if (node.children.Count == 1)
                        {

                        }
                        else if (node.children.Count == 0)
                        {

                        }
                        else
                            throw new InvalidNonTermLenght(node.nterm);

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
