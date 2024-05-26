using System;
using System.Collections.Generic;

namespace  lab3._1.src.GeneratedParser
{
    public class LL1ParserTable
    {
        public static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()
        {
            { "program kw_nonterminal", new string[] { "decl", "rules", "axiom" } },
            { "decl kw_nonterminal", new string[] { "kw_nonterminal", "non_terms", "sc", "kw_terminal", "terms", "sc" } },
            { "rules non_term", new string[] { "rule", "rules" } },
            { "rules kw_axiom", new string[] { } },
            { "axiom kw_axiom", new string[] { "kw_axiom", "non_term", "sc" } },
            { "rule non_term", new string[] { "non_term", "kw_eq", "alts", "sc" } },
            { "non_terms non_term", new string[] { "non_term", "non_term_tail" } },
            { "non_term_tail comma", new string[] { "comma", "non_terms" } },
            { "non_term_tail sc", new string[] { } },
            { "terms term", new string[] { "term", "term_tail" } },
            { "term_tail comma", new string[] { "comma", "terms" } },
            { "term_tail sc", new string[] { } },
            { "alts non_term", new string[] { "symbols", "alt_tail" } },
            { "alts term", new string[] { "symbols", "alt_tail" } },
            { "alts kw_eps", new string[] { "symbols", "alt_tail" } },
            { "alts or", new string[] { "symbols", "alt_tail" } },
            { "alts sc", new string[] { "symbols", "alt_tail" } },
            { "alt_tail or", new string[] { "or", "alts" } },
            { "alt_tail sc", new string[] { } },
            { "symbols non_term", new string[] { "non_term", "symbols" } },
            { "symbols term", new string[] { "term", "symbols" } },
            { "symbols kw_eps", new string[] { "kw_eps", "symbols" } },
            { "symbols or", new string[] { } },
            { "symbols sc", new string[] { } },
        };

        public static readonly string axiom = "program";

        public static readonly HashSet<string> NonTerms = new HashSet<string>()
        {
            "program",
            "decl",
            "rules",
            "axiom",
            "rule",
            "non_terms",
            "non_term_tail",
            "terms",
            "term_tail",
            "alts",
            "alt_tail",
            "symbols",
        };
    }
}
