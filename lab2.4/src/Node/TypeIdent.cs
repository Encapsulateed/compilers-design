﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class TypeIdent
    {
        string identifire = string.Empty;

        public TypeIdent(string identifire)
        {
            this.identifire = identifire;
        }   

        public void PrintNode()
        {
            Console.WriteLine($"Type Ident: {identifire}");
        }
    }
}
