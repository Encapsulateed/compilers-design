using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Lexer
{
    internal class Compiler
    {
        private SortedList<Position, Message> messages;
        private Dictionary<string, int> nameCodes;
        private List<string> names;

        public Compiler()
        {
            messages = new SortedList<Position, Message>();
            nameCodes = new Dictionary<string, int>();
            names = new List<string>();

        }

        public int AddName(string name)
        {
            if (nameCodes.ContainsKey(name))
                return nameCodes[name];

            int code = names.Count;
            names.Add(name);
            nameCodes[name] = code;
            return code;

        }
        public string GetName(int code)
        {
            return names[code];
        }

        public void AddMessage(bool isErr, Position c, string text)
        {
            messages[c] = new Message(isErr, text);
        }

        public void OutPutMessages()
        {
            foreach (var p in messages)
            {
                Console.Write(p.Value.IsErorr ? "Erorr " : "Warning ");
                Console.WriteLine($"{p.Key} {p.Value.Text}");

            }
        }
        public Scanner GetScaner(string program)
        {
            return new Scanner(program, this);
        }
    }
}
