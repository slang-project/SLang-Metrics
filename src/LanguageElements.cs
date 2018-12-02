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
        private LinkedList<BlockMember> members { get; }

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
        private LinkedList<BlockMember> members { get; }

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
        private CompoundName name { get; }
        private LinkedList<UnitName> parents { get; }
        private LinkedList<Declaration> members { get; }

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
        private string name { get; }
        private string aliasName { get; }
        private Block routineBlock { get; }

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
        private Block mainBlock { get; }
        private Block elseBlock { get; }
        private LinkedList<Block> elsifBlockList { get; }

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
        private Block loopBlock { get; }

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
        private string name { get; }
        private object generics;  // TODO generics

        public UnitTypeName(string name, object generics)
        {
            this.name = name;
            this.generics = generics;
        }
    }

    internal class UnitName
    {
        private bool hasTilde { get; }

        public UnitName(Type type, bool hasTilde)
        {
            this.hasTilde = hasTilde;
        }
    }

    internal class CompoundName
    {
        private LinkedList<string> names;

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
