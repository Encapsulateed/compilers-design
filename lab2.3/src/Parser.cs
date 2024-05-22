using lab2._3.src.Lexer;
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
            { "program kw_nonterminal",new List<string>(){"decls", "rules", "axioms"} },
            { "program kw_terminal",new List<string>(){ "decls", "rules", "axioms" } },

            { "decls kw_nonterminal",new List<string>(){ "kw_nonterminal", "non_terms", "sc", "nl"} },
            { "decls kw_terminal",new List<string>(){ "kw_terminal", "non_terms", "sc", "nl"} },

            { "rules non_term",new List<string>(){ "rule", "rules"} },
            { "rule kw_axiom",new List<string>(){ } },
            { "rules",new List<string>(){ } },

            { "axioms kw_axiom",new List<string>(){ "axiom", "axioms" } },
            { "axioms",new List<string>(){} },

            { "rule non_term",new List<string>(){"non_term", "kw_eq", "rule_body","sc","nl"} },

            { "axiom kw_axiom",new List<string>(){ "kw_axiom", "non_term","sc","nl" } },

            { "non_terms non_terms",new List<string>(){ "non_term" } },
            { "non_terms nl",new List<string>(){  } },
            { "non_terms comma",new List<string>(){"comma", "non_terms"} },

            { "terms term",new List<string>(){"term", "term_tail"} },

            { "term_tail nl",new List<string>(){} },
            { "term_tail comma",new List<string>(){"comma", "terms"} },

            { "rule_body symbols",new List<string>(){"alts"} },
            { "rule_body kw_eps",new List<string>(){"alts"} },

            { "alts symbols",new List<string>(){"alt","alt_tail","sc", "nl"} },
            { "alts kw_eps",new List<string>(){ "alt", "alt_tail", "sc", "nl" } },

            { "alt_tail sc",new List<string>(){ } },
            { "alt_tail or",new List<string>(){"or","alts" } },

            { "alt symbols",new List<string>(){"symbols" } },
            { "alt kw_eps",new List<string>(){ "kw_eps" } },

            // Возможно это надо удалить, они терминальные 
            { "nl nl",new List<string>(){ "nl" } },
            { "sc sc",new List<string>(){ "sc" } },
            { "kw_eq kw_eq",new List<string>(){ "kw_eq" } },
            { "comma comma",new List<string>(){ "comma" } },
            { "or or",new List<string>(){ "or" } },
        };

        bool isTerminal(string str)
        {
            return str == "nl" || str == "sc" || str == "KW_EQ" || str == "or" ||str =="comma" || str =="symbols" || 
                   str == "KW_AXIOM" || str == "KW_NONTERMINAL" || str == "KW_TERMINAL" || str == "KW_EPS";
        }

        public INode parse(Scanner sc)
        {

            var stack = new Stack<StackNode>();

            InnerNode start = new InnerNode("");

            stack.Push(new StackNode() { value = "program", node = start });


            var tok = sc.NextToken();

            while (tok != null || tok!.Tag != Tokens.DomainTag.EOF) 
            {
                var lower_tag = tok.Tag.ToString().ToLower(); 

                var top = stack.Peek();
                var parent = top.node;
                var val = top.value;


             
                Console.WriteLine($"will check {val} {lower_tag}");
                if (isTerminal(val))
                {
                    Console.WriteLine($"Here terminal {lower_tag} and val: {val}");
                    if (val == lower_tag)
                    {
                        parent.AddChild(new Leaf(tok));
                        tok = sc.NextToken();
                    }
                    else
                    {
                        throw new Exception("Parsing erorr");
                    }
                }
                else if (table.ContainsKey($"{val} {lower_tag}"))
                {
                    lower_tag = tok.Tag.ToString().ToLower();
                    Console.WriteLine($"check {val} {lower_tag}");
                   

                    var go_to = table[$"{val} {lower_tag}"];

                    var inner = new InnerNode(val);
                    parent.AddChild(inner);

                    foreach (var go in go_to)
                        stack.Push(new StackNode() { value = go, node = inner });


                }
                else
                {
                    Console.WriteLine($"invalid tok was {val} {lower_tag}");
                    stack.Pop();
                 //   throw new Exception($"Invalid Token {val} {lower_tag}");
                }
            }

            return start;
        }
    }
}
