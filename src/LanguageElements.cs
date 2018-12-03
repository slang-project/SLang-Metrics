using System.Collections.Generic;
using SLangMetrics;

namespace LanguageElements
{
    interface ICCMesurable
    {
        int getCC();
    }

    class Module
    {
        public LinkedList<BlockMember> members { get; }

        internal Module(LinkedList<BlockMember> members)
        {
            this.members = members;
        }
    }

    abstract class BlockMember
    {
    }

    class Block : BlockMember
    {
        public LinkedList<BlockMember> members { get; }

        internal Block(LinkedList<BlockMember> members)
        {
            this.members = members;
        }
    }

    abstract class Declaration : BlockMember
    {
    }

    class UnitDeclaration : Declaration
    {
        public CompoundName name { get; }
        public LinkedList<UnitName> parents { get; }
        public LinkedList<Declaration> members { get; }

        internal UnitDeclaration(CompoundName name, LinkedList<UnitName> parents, LinkedList<Declaration> members)
        {
            this.name = name;
            this.parents = parents;
            this.members = members;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    class RoutineDeclaration : Declaration
    {
        public string name { get; }
        public string aliasName { get; }
        public Block routineBlock { get; }

        internal RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = name;
            this.aliasName = aliasName;
            this.routineBlock = routineBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    class VariableDeclaration : Declaration
    {
        internal VariableDeclaration()
        {
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    abstract class Statement : BlockMember
    {
    }

    class IfStatement : Statement
    {
        public Block mainBlock { get; }
        public Block elseBlock { get; }
        public LinkedList<Block> elsifBlockList { get; }

        internal IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elseBlock = elseBlock;
            this.elsifBlockList = elsifBlockList;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    class LoopStatement : Statement
    {
        public Block loopBlock { get; }

        internal LoopStatement(Block loopBlock)
        {
            this.loopBlock = loopBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    abstract class Type
    {
    }

    class UnitTypeName : Type
    {
        public string name { get; }
        public object generics;  // TODO generics

        internal UnitTypeName(string name, object generics)
        {
            this.name = name;
            this.generics = generics;
        }
    }

    class UnitName
    {
        public string name { get; }
        public bool hasTilde { get; }

        internal UnitName(Type type, bool hasTilde)
        {
            this.hasTilde = hasTilde;
            if (type is UnitTypeName t)
            {
                this.name = t.name;
            }
            else
            {
                throw new WrongParentUnitNameException();  // TODO review
            }
        }

        private class WrongParentUnitNameException : System.Exception
        {
            internal WrongParentUnitNameException() : base()
            {
            }
        }
    }

    class CompoundName
    {
        public LinkedList<string> names;

        internal CompoundName(string name)
        {
            names = new LinkedList<string>();
            names.AddFirst(name);
        }

        internal void AddFirst(string name)
        {
            names.AddFirst(name);
        }

        internal void AddLast(string name)
        {
            names.AddLast(name);
        }
    }
}
