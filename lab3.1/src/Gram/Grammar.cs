using lab2._3.src;
using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Gram
{
    internal class Grammar
    {
        public HashSet<string> Terms { get; private set; } = new HashSet<string>();
        public HashSet<string> NonTerms { get; private set; } = new HashSet<string>();
        public Dictionary<string, List<List<string>>> Rules { get; private set; } = new Dictionary<string, List<List<string>>>();
        public Axiom axiom { get; private set; } = new Axiom(string.Empty);

        public Dictionary<string, HashSet<string>> FollowSets = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> FirstSets = new Dictionary<string, HashSet<string>>();

        public static readonly string EPS = "#";

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

                        var new_term = ((TermToken)((Leaf)node.children[0]).tok).term.Replace("\'", "");
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
                                leaf = ((TermToken)leaf_value).term.Replace("\'", "");
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


            FirstSets = CalculateFirstSets();


            ComputeFollowSets();

           
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
                CalculateFirst(firstByNonTerm, nonTerm);
            }

            return firstByNonTerm;
        }
        public HashSet<string> FindFirst(string symbol)
        {
            if (FirstSets.ContainsKey(symbol))
            {
                return FirstSets[symbol];
            }

            return new HashSet<string>() { symbol };
        }
        private HashSet<string> CalculateFirst(Dictionary<string, HashSet<string>> firstByNonTerm, string symbol)
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
                    var firstOfRight = CalculateFirst(firstByNonTerm, rightSymbol);

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

        public void ComputeFollowSets()
        {
            foreach (var nonTerm in NonTerms)
            {
                FollowSets[nonTerm] = new HashSet<string>();
            }

            FollowSets[axiom.non_term].Add("$");

            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (var rule in Rules)
                {
                    string leftSide = rule.Key;
                    List<List<string>> rightSides = rule.Value;

                    foreach (var rightSide in rightSides)
                    {
                        for (int i = 0; i < rightSide.Count; i++)
                        {
                            string symbol = rightSide[i];
                            if (NonTerms.Contains(symbol))
                            {
                                if (i == rightSide.Count - 1 || rightSide[i + 1] == EPS)
                                {
                                    changed |= AddAll(FollowSets[symbol], FollowSets[leftSide]);
                                }
                                else
                                {
                                    var firstBeta = First(rightSide.GetRange(i + 1, rightSide.Count - i - 1));
                                    if (firstBeta.Contains(EPS))
                                    {
                                        firstBeta.Remove(EPS);
                                        changed |= AddAll(FollowSets[symbol], firstBeta);
                                        changed |= AddAll(FollowSets[symbol], FollowSets[leftSide]);
                                    }
                                    else
                                    {
                                        changed |= AddAll(FollowSets[symbol], firstBeta);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        private HashSet<string> First(List<string> symbols)
        {
            HashSet<string> firstSet = new HashSet<string>();

            foreach (var symbol in symbols)
            {
                if (Terms.Contains(symbol))
                {
                    firstSet.Add(symbol);
                    break;
                }
                else
                {
                    var firstOfSymbol = FirstSets[symbol];
                    firstSet.UnionWith(firstOfSymbol);

                    if (!firstOfSymbol.Contains(EPS))
                    {
                        break;
                    }
                }
            }

            return firstSet;
        }
        private bool AddAll<T>(HashSet<T> target, HashSet<T> source)
        {
            int initialCount = target.Count;
            target.UnionWith(source);
            return target.Count > initialCount;
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
