using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Grammar
{
    internal class Grammar
    {
        public HashSet<string> Terms { get; private set; } = new HashSet<string>();
        public HashSet<string> NonTerms { get; private set; } = new HashSet<string>();
        public Dictionary<string, List<List<string>>> Rules { get; private set; } = new Dictionary<string, List<List<string>>>();
        public Axiom axiom { get; private set; } = new Axiom(string.Empty);

        public Dictionary<string, HashSet<string>> FollowSets = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> FirsrtSets = new Dictionary<string, HashSet<string>>();

        private const string EPS = "$";

        private string CurrentNonTerm { get; set; } = string.Empty;
        private int CurrentListIndex
        {
            get
            {
                if (!Rules.ContainsKey(CurrentNonTerm))
                    return -1;

                return Rules[CurrentNonTerm].Count - 1;

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

                        if (!NonTerms.Contains(nt))
                            throw new NoSuchNonTerminalException(nt);

                        axiom = new Axiom(nt);

                        node.children.ForEach(Traverse);


                        break;

                    case "rule":
                        if (node.children.Count != 4)
                            throw new InvalidNonTermLenght(node.nterm);

                        var rule_left = ((NonTerm)((Leaf)node.children[0]).tok).nterm;

                        if (!NonTerms.Contains(rule_left))
                            throw new NoSuchNonTerminalException(rule_left);


                        CurrentNonTerm = rule_left;

                        Rules[rule_left] = new List<List<string>>
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
                            NonTerms.Add(new_nt);
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

                        var new_term = ((TermToken)((Leaf)node.children[0]).tok).term[1].ToString(); ;
                        Terms.Add(new_term);

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
                            Rules[CurrentNonTerm].Add(new List<string>());
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
                                if (!NonTerms.Contains(leaf))
                                    throw new NoSuchNonTerminalException(leaf);
                            }
                            else if (leaf_value is TermToken)
                            {
                                leaf = ((TermToken)leaf_value).term[1].ToString();
                                if (!Terms.Contains(leaf))
                                    throw new NoSuchTerminalException(leaf);
                            }
                            else if (leaf_value is KeyWordToken)
                            {
                                leaf = EPS;
                            }


                            Rules[CurrentNonTerm][CurrentListIndex].Add(leaf);
                            node.children.ForEach(Traverse);
                        }
                        else if (node.children.Count != 0)
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
            FirsrtSets = CalculateFirstSets();
            FollowSets = CalculateFollowSets();
        }

        public void Print()
        {
            Console.WriteLine($"Аксиома грамматики: {axiom.ToString()}");
            Console.WriteLine("Правила грамматики:");
            foreach (var nt in Rules.Keys)
            {
                var rule = new StringBuilder(nt);
                rule.Append(" -> ");
                foreach (var alt in Rules[nt])
                {
                    foreach (var symbol in alt)
                    {
                        rule.Append(symbol.ToString());
                        rule.Append(" ");
                    }
                    if (Rules[nt].Count > 1 && Rules[nt].IndexOf(alt) != Rules[nt].Count - 1)
                    {
                        rule.Append("| ");
                    }
                }
                Console.WriteLine(rule.ToString());
            }


        }
        public Dictionary<string, HashSet<string>> CalculateFirstSets()
        {
            var firstByNonTerm = NonTerms.ToDictionary(nt => nt, nt => new HashSet<string>());

            foreach (var nonTerm in NonTerms)
            {
                FindFirst(firstByNonTerm, nonTerm);
            }

            return firstByNonTerm;
        }

        private HashSet<string> FindFirst(Dictionary<string, HashSet<string>> firstByNonTerm, string symbol)
        {
            if (firstByNonTerm.ContainsKey(symbol) && firstByNonTerm[symbol].Count > 0)
            {
                return firstByNonTerm[symbol];
            }

            var first = new HashSet<string>();

            if (symbol == EPS)
            {
                first.Add(EPS);
                return first;
            }

            if (Terms.Contains(symbol))
            {
                first.Add(symbol);
                return first;
            }

            foreach (var alt in Rules[symbol])
            {
                foreach (var rightSymbol in alt)
                {
                    var firstOfRight = FindFirst(firstByNonTerm, rightSymbol);

                    foreach (var k in firstOfRight)
                    {
                        if (k != EPS || firstOfRight.Count == 1)
                        {
                            first.Add(k);
                        }
                    }

                    if (!firstOfRight.Contains(EPS))
                    {
                        break;
                    }
                }
            }

            firstByNonTerm[symbol] = first;
            return first;
        }

        public Dictionary<string, HashSet<string>> CalculateFollowSets()
        {
            var firstByNonTerm = CalculateFirstSets();
            var followByNonTerm = NonTerms.ToDictionary(nt => nt, nt => new HashSet<string>());

            foreach (var nonTerm in NonTerms)
            {
                FindFollow(firstByNonTerm, followByNonTerm, nonTerm);
            }

            return followByNonTerm;
        }

        private HashSet<string> FindFollow(Dictionary<string, HashSet<string>> firstByNonTerm, Dictionary<string, HashSet<string>> followByNonTerm, string symbol)
        {
            if (followByNonTerm.ContainsKey(symbol) && followByNonTerm[symbol].Count > 0)
            {
                return followByNonTerm[symbol];
            }

            var followSet = new HashSet<string>();

            if (symbol == axiom.non_term)
            {
                followSet.Add("$");
            }

            var visited = new HashSet<string> { symbol };

            foreach (var (key, rule) in Rules)
            {
                foreach (var alt in rule)
                {
                    for (int i = 0; i < alt.Count; i++)
                    {
                        var rightSymbol = alt[i];
                        if (rightSymbol == symbol && i < alt.Count - 1)
                        {
                            var next = alt[i + 1];
                            var firstNext = FindFirst(firstByNonTerm, next);

                            foreach (var firstSymbol in firstNext)
                            {
                                if (firstSymbol != EPS)
                                {
                                    followSet.Add(firstSymbol);
                                }
                            }

                            if (firstNext.Contains(EPS))
                            {
                                if (!visited.Contains(key))
                                {
                                    visited.Add(key);
                                    var followOfKey = FindFollow(firstByNonTerm, followByNonTerm, key);
                                    foreach (var followSymbol in followOfKey)
                                    {
                                        followSet.Add(followSymbol);
                                    }
                                }
                            }
                        }

                        if (rightSymbol == symbol && i == alt.Count - 1)
                        {
                            if (!visited.Contains(key))
                            {
                                visited.Add(key);
                                var followOfKey = FindFollow(firstByNonTerm, followByNonTerm, key);
                                foreach (var followSymbol in followOfKey)
                                {
                                    followSet.Add(followSymbol);
                                }
                            }
                        }
                    }
                }
            }

            followByNonTerm[symbol] = followSet;

            return followSet;
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
