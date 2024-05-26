using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
using lab3._1.src.GeneratedParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
 
namespace lab2._3.src
{
    internal class Parser
    {
        Dictionary<string, string[]> debug_table = new Dictionary<string, string[]>()
        {
            {"program kw_nonterminal", new string[] {"decl","rules","axioms" } },
            {"rules kw_axiom", new string[] {} },
            {"rules non_term", new string[] {"rule","rules" } },
            {"axioms kw_axiom", new string[] {"axiom","axioms"} },
            {"decl kw_nonterminal", new string[] { "kw_nonterminal", "non_terms", "sc", "kw_terminal","terms","sc" } },
            {"axiom kw_axiom", new string[] { "kw_axiom", "non_term", "sc" } },
            {"rule non_term", new string[] { "non_term", "kw_eq","alts","sc" } },
            {"non_terms non_term", new string[] { "non_term", "non_term_tail" } },
            {"non_term_tail sc", new string[] { } },
            {"non_term_tail comma", new string[] {"comma","non_terms" } },
            {"terms term", new string[] { "term", "term_tail" } },
            {"term_tail sc", new string[] { } },
            {"term_tail comma", new string[] {"comma","terms" } },
            {"alts non_term", new string[] {"symbols","alt_tail" } },
            {"alts term", new string[] {"symbols","alt_tail" } },
            {"alts kw_eps", new string[] {"symbols","alt_tail" } },
            {"alt_tail or", new string[] {"or","alts" } },
            {"alt_tail sc", new string[] {} },
            {"symbols non_term", new string[] {"non_term", "symbols"} },
            {"symbols term", new string[] {"term", "symbols"} },
            {"symbols kw_eps", new string[] { "kw_eps", "symbols"} },
            {"symbols sc", new string[] { } },
            {"symbols or", new string[] { } }
        };
        Dictionary<string, string[]> real_table = LL1ParserTable.Table;
        bool isTerminal(string str)
        {
            return !(LL1ParserTable.NonTerms.Contains(str));
        }

        public INode parse(Scanner sc)
        {

            var stack = new Stack<StackNode>();

            InnerNode start = new InnerNode("");

            stack.Push(new StackNode() { value = LL1ParserTable.axiom, node = start });


            Token tok = sc.NextToken();

            while (tok.Tag != DomainTag.EOF && stack.Count != 0)
            {
                var top = stack.Pop();

                var lexer_tag = tok.Tag.ToString().ToLower();

                if (isTerminal(top.value))
                {
                    top.node.AddChild(new Leaf(tok));

                    tok = sc.NextToken();

                }
                else if (real_table.Keys.Contains($"{top.value} {tok.Tag.ToString().ToLower()}"))
                {
                    var inner = new InnerNode(top.value);
                    top.node.AddChild(inner);

                    var go = real_table[$"{top.value} {tok.Tag.ToString().ToLower()}"];



                    for (int i = go.Length - 1; i >= 0; i--)
                    {
                        stack.Push(new StackNode() { value = go[i], node = inner });
                    }



                }
                else
                {
                    throw new Exception($"Invalid Tokens {top.value} {lexer_tag}");
                }
            }

            return start;
        }
    }

}
