using lab2._3.src.Tokens;
using lab3._1.src.Exeptions;
using lab3._1.src.Gram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Parser
{
    internal class ParsingTable
    {
        public Dictionary<string, string[]> Table { get; private set; }
        private Grammar _grammar;
        public ParsingTable(Grammar g)
        {   
            _grammar = g;
            Table = new Dictionary<string, string[]>();

            foreach (var nonTerm in g.NonTerms)
            {
                foreach (var rule in g.Rules[nonTerm])
                {
                    var firstSet = ComputeFirstSet(rule,g);

                    foreach (var terminal in firstSet)
                    {
                        if (terminal != Grammar.EPS)
                        {
                            AddToTable(nonTerm, terminal, rule);
                        }
                    }

                    if (firstSet.Contains(Grammar.EPS))
                    {
                        foreach (var follow in g.FollowSets[nonTerm])
                        {
                            AddToTable(nonTerm, follow, rule);
                        }
                    }
                }
            }

        
        }

        private HashSet<string> ComputeFirstSet(List<string> production, Grammar g)
        {
            var firstSet = new HashSet<string>();

            foreach (var symbol in production)
            {
                if (g.Terms.Contains(symbol))
                {
                    firstSet.Add(symbol);
                    break;
                }

                if (g.NonTerms.Contains(symbol))
                {
                    firstSet.UnionWith(g.FirstSets[symbol]);
                    if (!g.FirstSets[symbol].Contains(Grammar.EPS))
                    {
                        break;
                    }
                }
            }

            if (firstSet.Count == 0 || firstSet.All(s => s == Grammar.EPS))
            {
                firstSet.Add(Grammar.EPS);
            }

            return firstSet;
        }

        private void AddToTable(string nonTerm, string terminal, List<string> production)
        {
            string key = $"{nonTerm} {terminal}";

            if (!Table.ContainsKey(key))
            {
                Table[key] = production.ToArray();
            }
            else
            {
                throw new GrammarNotLLException();
            }
        }

        public string GenerateCSharpCode() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace  lab3._1.src.GeneratedParser");
            sb.AppendLine("{");
            sb.AppendLine("    public class LL1ParserTable");
            sb.AppendLine("    {");
            sb.AppendLine("        public static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()");
            sb.AppendLine("        {");

            foreach (var entry in Table)
            {
                string key = entry.Key.ToLower();
                string[] values = entry.Value;

                if (values.Length == 1 && values[0] == Grammar.EPS)
                {
                    sb.AppendLine($"            {{ \"{key}\", new string[] {{ }} }},");
                }
                else
                {
                    string valuesString = string.Join("\", \"", values);
                    sb.AppendLine($"            {{ \"{key}\", new string[] {{ \"{valuesString.ToLower()}\" }} }},");
                }
            }

            sb.AppendLine("        };");
            sb.AppendLine();

            sb.AppendLine($"        public static readonly string axiom = \"{_grammar.axiom.non_term.ToLower()}\";");
            sb.AppendLine();

            sb.AppendLine("        public static readonly HashSet<string> NonTerms = new HashSet<string>()");
            sb.AppendLine("        {");

            foreach (var nonTerm in _grammar.NonTerms)
            {
                sb.AppendLine($"            \"{nonTerm.ToLower()}\",");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public void SaveToFile(string dir = "src\\Parser")
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, $"{dir}\\LL1ParserTable.cs");


            var content = GenerateCSharpCode();
            File.WriteAllText(filePath, content);
        }

        public void Print()
        {
            foreach (var entry in Table)
            {
                Console.WriteLine($"{entry.Key} -> {string.Join(" ", entry.Value)}");
            }
        }
    }
}
