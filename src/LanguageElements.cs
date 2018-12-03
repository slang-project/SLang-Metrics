using System.Collections.Generic;
using System.Linq;
using SLangMetrics;

namespace LanguageElements
{
    interface ICCMeasurable
    {
        int getCC();
    }

    interface IWMCMesurable
    {
        int getWMC();
    }

    class Module : ICCMeasurable
    {
        public LinkedList<BlockMember> members { get; }
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
                    if (m is ICCMeasurable statement)
                    {
                        CC *= statement.getCC();
                    }
                }
            }

            return CC.Value;
        }
    }

    abstract class BlockMember
    {
    }


    class Block : BlockMember, ICCMeasurable
    {
        public LinkedList<BlockMember> members { get; }
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
                    if (!(m is RoutineDeclaration) && m is ICCMeasurable statement)
                    {
                        CC *= statement.getCC();
                    }
                }
            }

            return CC.Value;
        }
    }

    abstract class Declaration : BlockMember
    {
    }

    class UnitDeclaration : Declaration, IWMCMesurable
    {
        public CompoundName name { get; }
        public LinkedList<UnitName> parents { get; }
        public LinkedList<Declaration> members { get; }
        private int? WMC = null;

        internal UnitDeclaration(CompoundName name, LinkedList<UnitName> parents, LinkedList<Declaration> members)
        {
            this.name = name;
            this.parents = parents;
            this.members = members;
            System.Console.WriteLine(this.GetType().Name);  // TODO: remove
        }

        public int getWMC()
        {
            if (WMC == null)
            {
                WMC = 0;
                foreach (var m in members)
                {
                    if (m is RoutineDeclaration routine)
                    {
                        WMC += routine.getCC();
                    }
                }
            }

            return WMC.Value;
        }
    }

    class RoutineDeclaration : Declaration, ICCMeasurable
    {
        public string name { get; }
        public string aliasName { get; }
        public Block routineBlock { get; }

        internal RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = name;
            this.aliasName = aliasName;
            this.routineBlock = routineBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO: remove
        }

        public int getCC()
        {
            if (routineBlock != null)
                return routineBlock.getCC();
            else
                return 0;
        }
    }

    class VariableDeclaration : Declaration
    {
        internal VariableDeclaration()
        {
            System.Console.WriteLine(this.GetType().Name);  // TODO: remove
        }
    }

    abstract class Statement : BlockMember
    {
    }


    class IfStatement : Statement, ICCMeasurable
    {
        public Block mainBlock { get; }
        public Block elseBlock { get; }
        public LinkedList<Block> elsifBlockList { get; }
        private int? CC = null;

        internal IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elseBlock = elseBlock;
            this.elsifBlockList = elsifBlockList;
            System.Console.WriteLine(this.GetType().Name);  // TODO: remove
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

    class LoopStatement : Statement, ICCMeasurable
    {
        public Block loopBlock { get; }
        private int? CC = null;

        internal LoopStatement(Block loopBlock)
        {
            this.loopBlock = loopBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO: remove
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

    abstract class Type
    {
    }

    class UnitTypeName : Type
    {
        public string name { get; }
        public object generics;  // TODO: generics

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
                throw new WrongParentUnitNameException();  // TODO: review
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
