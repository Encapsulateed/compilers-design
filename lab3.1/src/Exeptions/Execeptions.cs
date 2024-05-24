using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace lab3._1.src.Exeptions
{
    public class TooManyAxiomException : Exception
    {
        static string _message = "Указано более одной аксиомы грамматики!";
        public TooManyAxiomException() : base(_message) { }
    }

    public class InvalidNonTermLenght: Exception
    {
        static string _message = "Накорректная длина: ";
        public InvalidNonTermLenght(string nt) : base(_message + nt) { }
    }

    public class InvalidTree : Exception
    {
        static string _message = "Некорректное дерево, в ноде: ";
        public InvalidTree(string nt) : base(_message + nt) { }

    }


    public class NoAxiomException : Exception
    {
        static string _message = "Отсутсвует аксиома грамматики!";

        public NoAxiomException() : base(_message) { }
    }

    public class RepetedNonTerminalException : Exception
    {
        static string _message = "Повторный нетерменал грамматики: ";

        public RepetedNonTerminalException(string nt) : base(_message + nt) { }
    }

    public class RepetedTerminalException : Exception
    {
        static string _message = "Повторный терменал грамматики: ";

        public RepetedTerminalException(string t) : base(_message + t) { }
    }

    public class NoSuchNonTerminalException : Exception
    {
        static string _message = "Нетерменал не был объявлен: ";

        public NoSuchNonTerminalException(string nt) : base(_message + nt) { }
    }

    public class NoSuchTerminalException : Exception
    {
        static string _message = "Терменал не был объявлен: ";

        public NoSuchTerminalException(string t) : base(_message + t) { }
    }

    public class GrammarNotLLException : Exception
    {
        static string _message = "Грамматика не относится на классу LL(1)!";

        public GrammarNotLLException() : base(_message) { }
    }
}
