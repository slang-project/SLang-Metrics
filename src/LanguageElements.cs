using System.Collections.Generic;
using System.Linq;
using SLangMetrics;

namespace LanguageElements
{
    internal interface ICCMesurable
    {
        int getCC();
    }

    internal class Module : ICCMesurable
    {
        internal LinkedList<BlockMember> members { get; }
        private int? CC = null;

        internal Module(LinkedList<BlockMember> members)
        {
            this.members = members;
        }

        public int getCC()
        {
            if (CC == null)
            {
                CC = 1;
                foreach (var m in members)
                {
                    if (m is ICCMesurable statement)
                    {
                        CC *= statement.getCC();
                    }
                }
            }

            return CC.Value;
        }
    }

    internal abstract class BlockMember
    {
    }

    internal class Block : BlockMember, ICCMesurable
    {
        internal LinkedList<BlockMember> members { get; }
        private int? CC = null;

        internal Block(LinkedList<BlockMember> members)
        {
            this.members = members;
        }

        public int getCC()
        {
            if (CC == null)
            {
                CC = 1;
                foreach (var m in members)
                {
                    if (m is ICCMesurable statement)
                    {
                        CC *= statement.getCC();
                    }
                }
            }

            return CC.Value;
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
            System.Console.WriteLine(this.GetType().Name); // TODO remove
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
            System.Console.WriteLine(this.GetType().Name); // TODO remove
        }
    }

    internal class VariableDeclaration : Declaration
    {
        public VariableDeclaration()
        {
            System.Console.WriteLine(this.GetType().Name); // TODO remove
        }
    }

    internal abstract class Statement : BlockMember
    {
    }

    internal class IfStatement : Statement, ICCMesurable
    {
        internal Block mainBlock { get; }
        internal Block elseBlock { get; }
        internal LinkedList<Block> elsifBlockList { get; }
        private int? CC = null;

        public IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elseBlock = elseBlock;
            this.elsifBlockList = elsifBlockList;
            System.Console.WriteLine(this.GetType().Name); // TODO remove
        }

        public int getCC()
        {
            if (CC == null)
            {
                CC = mainBlock.getCC();
                if (elseBlock != null)
                    CC += elseBlock.getCC();
                foreach (Block block in elsifBlockList ?? Enumerable.Empty<Block>())
                {
                    CC += block.getCC();
                }
            }

            return CC.Value;
        }
    }

    internal class LoopStatement : Statement, ICCMesurable
    {
        internal Block loopBlock { get; }
        private int? CC = null;

        public LoopStatement(Block loopBlock)
        {
            this.loopBlock = loopBlock;
            System.Console.WriteLine(this.GetType().Name); // TODO remove
        }

        public int getCC()
        {
            if (CC == null)
            {
                CC = 1 + loopBlock.getCC();
            }

            return CC.Value;
        }
    }

    internal abstract class Type
    {
    }

    internal class UnitTypeName : Type
    {
        internal string name { get; }
        internal object generics; // TODO generics

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
            if (type is UnitTypeName)
            {
                this.name = ((UnitTypeName) type).name;
            }
            else
            {
                throw new WrongParentUnitNameException(); // TODO review
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