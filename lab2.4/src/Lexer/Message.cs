using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Lexer
{
    internal class Message
    {
        public readonly bool IsErorr;
        public readonly string Text;


        public Message(bool isErorr, string text)
        {
            IsErorr = isErorr;
            Text = text;
        }
    }
}
