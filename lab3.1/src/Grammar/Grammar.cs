using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Grammar
{
    internal class Grammar
    {
        public HashSet<string> terms { get; private set; } = new HashSet<string>();
        public HashSet<string> non_terms { get; private set; } = new HashSet<string>();
        public Dictionary<string, List<List<string>>> rules { get; private set; } = new Dictionary<string, List<List<string>>>();
        public Axiom axiom { get; private set; } = new Axiom(string.Empty);

        private string CurrentNonTerm { get; set; } = string.Empty;
        private int CurrentListIndex
        {
            get
            {
                if (!rules.ContainsKey(CurrentNonTerm))
                    return -1;

                return rules[CurrentNonTerm].Count - 1;

            }
        }

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
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;

                    case "decl":
                        if (node.children.Count != 6)
                            throw new InvalidNonTermLenght(node.nterm);

                        node.children.ForEach(Traverse);

                        break;
                    case "axiom":
                        if (node.children.Count != 3)
                            throw new InvalidNonTermLenght(node.nterm);
                        if (node.children[1] is not Leaf)
                            throw new InvalidTree(node.nterm);
                        if (!axiom.isEmpty)
                            throw new TooManyAxiomException();
                       

                        var nt = ((NonTerm)((Leaf)node.children[1]).tok).nterm;

                        if (!non_terms.Contains(nt))
                            throw new NoSuchNonTerminalException(nt);

                        axiom = new Axiom(nt);

                        node.children.ForEach(Traverse);


                        break;

                    case "rule":
                        if (node.children.Count != 4)
                            throw new InvalidNonTermLenght(node.nterm);

                        var rule_left = ((NonTerm)((Leaf)node.children[0]).tok).nterm;

                        if (!non_terms.Contains(rule_left))
                            throw new NoSuchNonTerminalException(rule_left);


                        CurrentNonTerm = rule_left;

                        rules[rule_left] = new List<List<string>>
                        {
                            new List<string>()
                        };

                        node.children.ForEach(Traverse);

                        break;

                    case "non_terms":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                        {
                            var new_nt = ((NonTerm)((Leaf)node.children[0]).tok).nterm;
                            non_terms.Add(new_nt);
                        }


                        node.children.ForEach(Traverse);

                        break;

                    case "non_term_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                            node.children.ForEach(Traverse);

                        break;

                    case "terms":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);

                        var new_term = ((TermToken)((Leaf)node.children[0]).tok).term;
                        terms.Add(new_term);

                        node.children.ForEach(Traverse);

                        break;

                    case "term_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                            node.children.ForEach(Traverse);

                        break;

                    case "alts":
                        if (node.children.Count != 2)
                            throw new InvalidNonTermLenght(node.nterm);
                        node.children.ForEach(Traverse);

                        break;

                    case "alt_tail":
                        if (node.children.Count != 2 && node.children.Count != 0)
                            throw new InvalidNonTermLenght(node.nterm);

                        if (node.children.Count != 0)
                        {
                            rules[CurrentNonTerm].Add(new List<string>());
                            node.children.ForEach(Traverse);
                        }
                        break;

                    case "symbols":

                        if (node.children.Count == 2)
                        {
                            var leaf_value = ((Leaf)node.children[0]).tok;
                            string leaf = string.Empty;

                            if (leaf_value is NonTerm)
                            {
                                leaf = ((NonTerm)leaf_value).nterm;
                                if (!non_terms.Contains(leaf))
                                    throw new NoSuchNonTerminalException(leaf);
                            }
                            else if (leaf_value is TermToken)
                            {
                                leaf = ((TermToken)leaf_value).term;
                                if (!terms.Contains(leaf))
                                    throw new NoSuchTerminalException(leaf);
                            }
                            else if (leaf_value is KeyWordToken)
                            {
                                leaf = "eps";
                            }


                            rules[CurrentNonTerm][CurrentListIndex].Add(leaf);
                            node.children.ForEach(Traverse);
                        }
                        else if(node.children.Count !=0)
                        {
                            throw new InvalidNonTermLenght(node.nterm);

                        }

                        break;

                    default:
                        node.children.ForEach(Traverse);

                        break;
                }


            }

            Traverse(tree);
        }

        public void Print()
        {
            Console.WriteLine($"Аксиома грамматики: {axiom.ToString()}");
            Console.WriteLine("Правила грамматики:");
            foreach (var nt in rules.Keys)
            {
                var rule = new StringBuilder(nt);
                rule.Append(" -> ");
                foreach (var alt in rules[nt])
                {
                    foreach (var symbol in alt)
                    {
                        rule.Append(symbol.ToString());
                        rule.Append(" ");
                    }
                    if (rules[nt].Count > 1 && rules[nt].IndexOf(alt) != rules[nt].Count - 1)
                    {
                        rule.Append("| ");
                    }
                }
                Console.WriteLine(rule.ToString());
            }


        }
    
        
    }



    readonly struct Axiom
    {
        public readonly string non_term { get; }

        public Axiom(string non_term) => this.non_term = non_term;

        public bool isEmpty { get { return non_term == string.Empty; } }


        public override string ToString()
        {
            return non_term;
        }
    }
}
