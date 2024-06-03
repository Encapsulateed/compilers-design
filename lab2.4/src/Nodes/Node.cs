using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace lab2._4.src.Nodes
{
    public interface INode
    {

        void Print(string indent);
    }

    class Program : INode
    {
        public List<Block> blocks;

        public Program(List<Block> blocks) => this.blocks = blocks;
        public void Print(string indent)
        {
            Console.Write(indent + "Prograrm( ");
            foreach (var b in blocks)
            {
                b.Print(indent + '\t');
            }
            Console.WriteLine(")");
        }
    }
    abstract class Block : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    class TypeBlock : Block, INode
    {
        public List<TypeDef> typeDefs;

        public TypeBlock(List<TypeDef> typeDefs)
        {
            this.typeDefs = typeDefs;
        }

        public override void Print(string indent)
        {
            Console.Write(indent + "TypeDef( ");
            foreach (var td in typeDefs)
            {
                td.Print(indent + '\t');
            }
            Console.WriteLine(")");
        }
    }
    class ConstBlock : Block, INode
    {
        public List<ConstDef> constDefs;

        public ConstBlock(List<ConstDef> constDefs)
        {
            this.constDefs = constDefs;
        }

        public override void Print(string indent)
        {
            Console.Write(indent + "ConstDef( ");
            foreach (var td in constDefs)
            {
                td.Print(indent + '\t');
            }
            Console.WriteLine(")");
        }
    }

    class TypeDef : INode
    {
        Ident id;
        Type type;

        public TypeDef(Ident id, Type type)
        {
            this.id = id;
            this.type = type;
        }

        public void Print(string indent)
        {
            id.Print(indent);
            type.Print(indent);
        }
    }

    class ConstDef : INode
    {
        Ident id;
        Constant constant;

        public ConstDef(Ident id, Constant constant)
        {
            this.id = id;
            this.constant = constant;
        }

        public void Print(string indent)
        {
            id.Print(indent);
            constant.Print(indent);
        }

    }

    class Ident : INode
    {
        string id;
        public Ident(string id) { this.id = id; }
        public void Print(string indent)
        {
            Console.WriteLine(indent + $"Ident(${id})");
        }
    }


    abstract class Constant : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    abstract class Type : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    abstract class SimpleType : Type, INode
    {
        public override void Print(string indent)
        {
        }
    }

    class Idents : SimpleType, INode 
    {
        public List<Ident> idents;

        public Idents (List<Ident> idents)
        {
            this.idents = idents;
        }
        public override void Print(string indent)
        {
            foreach(var id in idents)
            {
                id.Print(indent + "\t");
            }
        }
    }

     class TwoConstants : SimpleType, INode
    {
        public Constant first;
        public Constant secod; 

        public TwoConstants(Constant first, Constant secod)
        {
            this.first = first;
            this.secod = secod;
        }

        public override void Print(string indent) 
        { 
            first.Print(indent + "\t");
            secod.Print(indent + "\t");
        }
    }

    class Array : Type, INode
    {
        public Type Type;
        public List<SimpleType> simpleTypes;

        public Array(Type type, List<SimpleType> simpleTypes)
        {
            Type = type;
            this.simpleTypes = simpleTypes;
        }
        public override void Print(string indent) 
        { 
            Type.Print(indent+'\t');
            foreach(var st in simpleTypes)
            {
                st.Print(indent + '\t');
            }
        }
    }


    class Set : Type, INode
    {
        public SimpleType simpleType;
        public Set(SimpleType simpleType)
        {
            this.simpleType = simpleType;
        }

        public override void Print(string indent)
        {
            simpleType.Print(indent + '\t');

        }
    }

    class Record : Type, INode
    {
        List<Field> fields;

        public Record(List<Field> fields)
        {
            this.fields = fields;
        }

        public override void Print(string indent)
        {
            foreach(var f in fields)
            {
                f.Print(indent + "\t");
            }
        }
    }

    abstract class Field : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    class SimpleFiled : Field, INode
    {
        List<Ident> idents; 
        Type Type;

        public SimpleFiled(List<Ident> idents, Type type)
        {
            this.idents = idents;
            Type = type;
        }

        public override void Print(string indent)
        {
            foreach (var ident in idents)
            {
                ident.Print(indent + "\t");
            }
            Type.Print(indent + "\t");
        }
    }

    class CaseFiled : Field, INode 
    {
        Type ident;
        Type type_ident;
        List<ConstantItem> constantItems;

        public CaseFiled(Type ident, Type type_ident, List<ConstantItem> constantItems)
        {
            this.ident = ident;
            this.type_ident = type_ident;
            this.constantItems = constantItems;
        }

        public override void Print(string indent)
        {
            ident.Print(indent+"\t");
            type_ident.Print(indent + "\t");

            foreach(var constant in constantItems)
            {
                constant.Print(indent + "\t");
            }
        }
    }

    class ConstantItem : INode
    {
        public List<Constant> constants;
        public List<Field> fields;

        public ConstantItem(List<Constant> constants, List<Field> fields)
        {
            this.constants = constants;
            this.fields = fields;
        }

        public void Print(string indent)
        {
            foreach (var c in constants)
            {
                c.Print(indent + "\t");
            }
            foreach (var f in fields)
            {
                f.Print(indent + "\t");
            }
        }
    }

    class UnsignedNumber : Constant, INode
    {
        double val;
        public UnsignedNumber(double val) { this.val = val; }
        public override void Print(string indent)
        {
            Console.WriteLine(indent + $"Unsigned_Number(${val})");
        }
    }

    class ConstantIdent : Constant, INode
    {
        string id;
        public ConstantIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine(indent + $"Constant_Ident(${id})");
        }
    }

}
