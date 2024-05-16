﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2._4.src.Node
{
    internal class PointerIdent : INode
    {
        string identifire = string.Empty;

        public PointerIdent(string identifire)
        {
            this.identifire = identifire;
        }   

        public void PrintNode()
        {
            Console.WriteLine($"Pointer Ident: {identifire}");
        }
    }
}