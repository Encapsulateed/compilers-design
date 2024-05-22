using lab2._3.src.Lexer;
using lab2._3.src.Tokens;
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
        Dictionary<string, List<string>> table = new Dictionary<string, List<string>>()
        {
            { "program kw_nonterminal",new List<string>(){"decls", "rules", "axiom"} },
            { "program kw_terminal",new List<string>(){ "decls", "rules", "axiom" } },
            { "program non_term",new List<string>(){ "decls", "rules", "axiom" } },
            { "program kw_axiom",new List<string>(){ "decls", "rules", "axiom" } },
        

            { "decls kw_nonterminal",new List<string>(){ "decl","decls"} },
            { "decls kw_terminal",new List<string>(){  "decl","decls"} },
            { "decls non_term",new List<string>(){} },
            { "decls kw_axiom",new List<string>(){} },

            { "rules non_term",new List<string>(){ "rule", "rules"} },
            { "rules kw_axiom",new List<string>(){ } },

            { "axiom kw_axiom",new List<string>(){ "kw_axiom","non_term","sc"} },

            { "decl kw_nonterminal",new List<string>(){ "kw_nonterminal","non_terms","sc" } },
            { "decl kw_terminal",new List<string>(){ "kw_terminal","terms","sc" } },



            { "rule non_term",new List<string>(){"non_term", "kw_eq","alts","sc"} },

             { "non_terms non_term",new List<string>(){"non_term","non_term_tail"} },
             { "non_term_tail sc",new List<string>(){} },
             { "non_term_tail comma",new List<string>(){"comma","non_terms"} },


             { "terms term",new List<string>(){"term","term_tail"} },
             { "term_tail sc",new List<string>(){} },
             { "term_tail comma",new List<string>(){"comma","terms"} },

            { "alts non_term",new List<string>(){"symbols","alt_tail"} },
            { "alts term",new List<string>(){"symbols","alt_tail"} },
            { "alts kw_eps",new List<string>(){"symbols","alt_tail"} },


            { "alt_tail sc",new List<string>(){ } },
            { "alt_tail or",new List<string>(){"or","alts" } },

            { "alt symbols",new List<string>(){"symbols" } },
            { "alt or",new List<string>(){ "kw_eps" } },

            { "symbols non_term",new List<string>(){ "non_term","symbols"} },
            { "symbols term",new List<string>(){ "term","symbols"} },
            { "symbols kw_eps",new List<string>(){ "kw_eps", "symbols"} },
        };

        bool isTerminal(string str)
        {
            return !(str == "program" || str == "decls" || str == "rules" || str =="decl" ||
                str == "axiom" || str == "rule" || str == "non_terms" || str == "non_term_tail" ||
                str == "terms" || str =="term_tail" || str =="alts" || str =="alt_tail"||str =="symbols");
        }

        public INode parse(Scanner sc)
        {

            var stack = new Stack<StackNode>();

            InnerNode start = new InnerNode("");

            stack.Push(new StackNode() { value = "program", node = start });


            Token tok = sc.NextToken();
           
            while (tok.Tag != DomainTag.EOF && stack.Count != 0) 
            {
                var lower_tag = tok.Tag.ToString().ToLower();

                var top = stack.Pop();

                if (isTerminal(top.value))
                {
                    top.node.AddChild(new Leaf(tok));
                    tok = sc.NextToken();
                    

                }
                else if (table.ContainsKey($"{top.value} {lower_tag}"))
                {
                    var inner = new InnerNode(top.value);
                    top.node.AddChild(inner);

                    var go_to = table[$"{top.value} {tok.Tag.ToString().ToLower()}"];
      
                    go_to.Reverse();

                    foreach (var go in go_to)
                        stack.Push(new StackNode() { value = go, node = inner });
                    
                }
                else
                {
                       throw new Exception($"Invalid Token {top.value} {lower_tag}");
                }
            }

            return start;
        }
        private static void PrintStack<StackNode>(Stack<StackNode> stack)
        {
            if (stack == null)
            {
                Console.WriteLine("Stack is null.");
                return;
            }

            if (stack.Count == 0)
            {
                Console.WriteLine("Stack is empty.");
                return;
            }

            Console.WriteLine("Stack contents:");

            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }
        }
    }

}
