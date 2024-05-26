using System;
using System.Collections.Generic;

namespace calculator.src.Parser
{
    public class LL1ParserTable
    {
        public static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()
        {
            { "e n", new string[] { "t", "e1" } },
            { "e (", new string[] { "t", "e1" } },
            { "e1 +", new string[] { "+", "t", "e1" } },
            { "e1 $", new string[] { } },
            { "e1 )", new string[] { } },
            { "t n", new string[] { "f", "t1" } },
            { "t (", new string[] { "f", "t1" } },
            { "t1 *", new string[] { "*", "f", "t1" } },
            { "t1 +", new string[] { } },
            { "t1 $", new string[] { } },
            { "t1 )", new string[] { } },
            { "f n", new string[] { "n" } },
            { "f (", new string[] { "(", "e", ")" } },
        };

        public static readonly string axiom = "e";

        public static readonly HashSet<string> NonTerms = new HashSet<string>()
        {
            "e",
            "e1",
            "t",
            "t1",
            "f",
        };
    }
}
