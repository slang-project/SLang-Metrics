using System.Collections.Generic;
using SLangMetrics;

namespace LanguageElements
{
    internal interface ICCMesurable
    {
        int getCC();
    }

    internal class Module
    {
        internal LinkedList<BlockMember> members { get; }

        internal Module(LinkedList<BlockMember> members)
        {
            this.members = members;
        }
    }

    internal abstract class BlockMember
    {
    }

    internal class Block : BlockMember
    {
        internal LinkedList<BlockMember> members { get; }

        internal Block(LinkedList<BlockMember> members)
        {
            this.members = members;
        }
    }

    internal abstract class Declaration : BlockMember
    {
    }

    internal class UnitDeclaration : Declaration
    {
        internal CompoundName name { get; }
        internal LinkedList<UnitName> parents { get; }
        internal LinkedList<Declaration> members { get; }

        public UnitDeclaration(CompoundName name, LinkedList<UnitName> parents, LinkedList<Declaration> members)
        {
            this.name = name;
            this.parents = parents;
            this.members = members;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    internal class RoutineDeclaration : Declaration
    {
        internal string name { get; }
        internal string aliasName { get; }
        internal Block routineBlock { get; }

        public RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = name;
            this.aliasName = aliasName;
            this.routineBlock = routineBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    internal class VariableDeclaration : Declaration
    {
        public VariableDeclaration()
        {
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    internal abstract class Statement : BlockMember
    {
    }

    internal class IfStatement : Statement
    {
        internal Block mainBlock { get; }
        internal Block elseBlock { get; }
        internal LinkedList<Block> elsifBlockList { get; }

        public IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elseBlock = elseBlock;
            this.elsifBlockList = elsifBlockList;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    internal class LoopStatement : Statement
    {
        internal Block loopBlock { get; }

        public LoopStatement(Block loopBlock)
        {
            this.loopBlock = loopBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }
    }

    internal abstract class Type
    {
    }

    internal class UnitTypeName : Type
    {
        internal string name { get; }
        internal object generics;  // TODO generics

        public UnitTypeName(string name, object generics)
        {
            this.name = name;
            this.generics = generics;
        }
    }

    internal class UnitName
    {
        internal string name { get; }
        internal bool hasTilde { get; }

        public UnitName(Type type, bool hasTilde)
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
            public WrongParentUnitNameException() : base()
            {
            }
        }
    }

    internal class CompoundName
    {
        internal LinkedList<string> names;

        public CompoundName(string name)
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
