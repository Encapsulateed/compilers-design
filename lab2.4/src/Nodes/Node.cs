using System;
using System.Collections.Generic;

namespace lab2._4.src.Nodes
{
    public interface INode
    {
        void Print(string indent);
    }

    public class Program : INode
    {
        public List<Block> blocks;

        public Program(List<Block> blocks) => this.blocks = blocks;
        public void Print(string indent)
        {
            Console.WriteLine($"{indent}Program");
            foreach (var b in blocks)
            {
                b.Print(indent + "\t");
            }
        }
    }

    public abstract class Block : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    public class TypeBlock : Block, INode
    {
        public List<TypeDef> typeDefs;

        public TypeBlock(List<TypeDef> typeDefs)
        {
            this.typeDefs = typeDefs;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeBlock");
            foreach (var td in typeDefs)
            {
                td.Print(indent + "\t");
            }
        }
    }

    public class ConstBlock : Block, INode
    {
        public List<ConstDef> constDefs;

        public ConstBlock(List<ConstDef> constDefs)
        {
            this.constDefs = constDefs;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}ConstBlock");
            foreach (var td in constDefs)
            {
                td.Print(indent + "\t");
            }
        }
    }

    public class TypeDef : INode
    {
        TypeIdent id;
        Type type;

        public TypeDef(TypeIdent id, Type type)
        {
            this.id = id;
            this.type = type;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeDef");
            id.Print(indent + "\t");
            type.Print(indent + "\t");
        }
    }

    public class ConstDef : INode
    {
        ConstantIdent id;
        Constant constant;

        public ConstDef(ConstantIdent id, Constant constant)
        {
            this.id = id;
            this.constant = constant;
        }

        public void Print(string indent)
        {
            Console.WriteLine($"{indent}ConstDef");
            id.Print(indent + "\t");
            constant.Print(indent + "\t");
        }

    }

    public class Ident : INode
    {
        string id;
        public Ident(string id) { this.id = id; }
        public void Print(string indent)
        {
            Console.WriteLine($"{indent}Ident: {id}");
        }
    }


    public class TypeIdent : SimpleType, INode
    {
        string id;
        public TypeIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}TypeIdent: {id}");
        }
    }

    public abstract class Constant : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    public abstract class Type : INode
    {
        public virtual void Print(string indent)
        {
        }
    }
    public class PointerIdent : Type, INode
    {
        string id;
        public PointerIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}PointerIdent({id})");
        }
    }
    public abstract class SimpleType : Type, INode
    {
        public override void Print(string indent)
        {
        }
    }

    public class Idents : SimpleType, INode
    {
        public List<Ident> idents;

        public Idents(List<Ident> idents)
        {
            this.idents = idents;
        }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Idents");

            foreach (var id in idents)
            {
                id.Print(indent + "\t");
            }
        }
    }

    public class TwoConstants : SimpleType, INode
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
            Console.WriteLine($"{indent}Two Constants");
            first.Print(indent + "\t");
            secod.Print(indent + "\t");
        }
    }

    public class Array : Type, INode
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
            Console.WriteLine($"{indent}array");

            foreach (var st in simpleTypes)
            {
                st.Print(indent + "");
            }
            Type.Print(indent + "");

        }
    }


    public class Set : Type, INode
    {
        public SimpleType simpleType;
        public Set(SimpleType simpleType)
        {
            this.simpleType = simpleType;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Set");
            simpleType.Print(indent + "\t");
        }
    }

    public class Record : Type, INode
    {
        List<Field> fields;

        public Record(List<Field> fields)
        {
            this.fields = fields;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Record");
            foreach (var f in fields)
            {
                f.Print(indent + "\t");
            }
        }
    }

    abstract public class Field : INode
    {
        public virtual void Print(string indent)
        {
        }
    }

    public class SimpleFiled : Field, INode
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
            Console.WriteLine($"{indent}SimpleField");
            foreach (var ident in idents)
            {
                ident.Print(indent + "\t");
            }
            Type.Print(indent + "\t");
        }
    }

    public class CaseFiled : Field, INode
    {
        Ident ident;
        Ident type_ident;
        List<ConstantItem> constantItems;

        public CaseFiled(Ident ident, Ident type_ident, List<ConstantItem> constantItems)
        {
            this.ident = ident;
            this.type_ident = type_ident;
            this.constantItems = constantItems;
        }

        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}CaseField");
            ident.Print(indent + "\t");
            type_ident.Print(indent + "\t");

            foreach (var constant in constantItems)
            {
                constant.Print(indent + "\t");
            }
        }
    }

    public class ConstantItem : INode
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
            Console.WriteLine($"{indent}ConstantItem");
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

    public class UnsignedNumber : Constant, INode
    {
        double val;
        public UnsignedNumber(double val) { this.val = val; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Unsigned_Number: {val}");
        }
    }

    public class ConstantIdent : Constant, INode
    {
        string id;
        public ConstantIdent(string id) { this.id = id; }
        public override void Print(string indent)
        {
            Console.WriteLine($"{indent}Constant_Ident : {id}");
        }
    }
}
