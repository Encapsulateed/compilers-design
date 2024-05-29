using calculator.src.Parser;
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

            while (tok.Tag != DomainTag.EOF )
            {
                var top = stack.Pop();

                var lexer_tag = Token.tag_to_str[tok.Tag];

                if (isTerminal(top.value))
                {
                    top.node.AddChild(new Leaf(tok));

                    tok = sc.NextToken();
                }
                else if (real_table.Keys.Contains($"{top.value} {lexer_tag}"))
                {
                    var inner = new InnerNode(top.value);
                    top.node.AddChild(inner);

                    lexer_tag = Token.tag_to_str[tok.Tag];


                    var go = real_table[$"{top.value} {lexer_tag}"];



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
