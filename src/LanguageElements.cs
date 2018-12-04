using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageElements
{
    interface ICCMeasurable
    {
        int getCC();
    }

    interface IWRUMesurable
    {
        int getWRU();
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

    abstract class BlockMember { }

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

    abstract class Declaration : BlockMember { }

    class UnitDeclaration : Declaration, IWRUMesurable
    {
        public CompoundName name { get; }
        public LinkedList<UnitName> parents { get; }
        public LinkedList<Declaration> members { get; }
        private int? WRU = null;

        internal UnitDeclaration(CompoundName name, LinkedList<UnitName> parents, LinkedList<Declaration> members)
        {
            this.name = name;
            this.parents = parents;
            this.members = members;
        }

        public int getWRU()
        {
            if (WRU == null)
            {
                WRU = 0;
                foreach (var m in members)
                {
                    if (m is RoutineDeclaration routine)
                    {
                        WRU += routine.getCC();
                    }
                }
            }

            return WRU.Value;
        }
    }

    class RoutineDeclaration : Declaration, ICCMeasurable
    {
        internal CompoundName name { get; }
        internal string aliasName { get; }
        internal Block routineBlock { get; }

        internal RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = new CompoundName(name);
            this.aliasName = aliasName;
            this.routineBlock = routineBlock;
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
        internal VariableDeclaration() { }
    }

    abstract class Statement : BlockMember { }

    class IfStatement : Statement, ICCMeasurable
    {
        public Block mainBlock { get; }
        public LinkedList<Block> elsifBlockList { get; }
        public Block elseBlock { get; }
        private int? CC = null;

        internal IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elsifBlockList = elsifBlockList;
            this.elseBlock = elseBlock;
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

    abstract class Type { }

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
        public CompoundName name { get; }
        public bool hasTilde { get; }

        internal UnitName(Type type, bool hasTilde)
        {
            this.hasTilde = hasTilde;
            if (type is UnitTypeName t)
            {
                this.name = new CompoundName(t.name);  // TODO: generics
            }
            else
            {
                throw new WrongParentUnitNameException();
            }
        }

        private class WrongParentUnitNameException : System.Exception
        {
            internal WrongParentUnitNameException() : base() { }
        }
    }

    class CompoundName
    {
        public LinkedList<string> names;

        public CompoundName()
        {
            names = new LinkedList<string>();
        }

        public CompoundName(string name)
        {
            names = new LinkedList<string>();
            names.AddFirst(name);
        }

        public void AddFirst(string name)
        {
            names.AddFirst(name);
        }

        public void AddLast(string name)
        {
            names.AddLast(name);
        }

        public void AppendFront(CompoundName cName)
        {
            LinkedList<string> buffer = new LinkedList<string>();
            foreach (string n in cName.names ?? Enumerable.Empty<String>())
            {
                buffer.AddLast(n);
            }
            foreach (string n in this.names ?? Enumerable.Empty<String>())
            {
                buffer.AddLast(n);
            }
            this.names = buffer;
        }

        public void AppendBack(CompoundName cName)
        {
            foreach (string n in cName.names ?? Enumerable.Empty<String>())
            {
                this.names.AddLast(n);
            }
        }

        public bool equal(CompoundName name)
        {
            return this.ToString().Equals(name.ToString());
        }

        public override string ToString()
        {
            string result = "";
            bool dotNeeded = false;
            foreach (string name in names ?? Enumerable.Empty<string>())
            {
                if (!dotNeeded)
                {
                    result += name;
                    dotNeeded = true;
                }
                else
                {
                    result += "." + name;
                }
            }

            // TODO: delete in production
            if (result.Equals(""))
            {
                return "<emptyUnitName>";
            }
            return result;
        }
    }

    internal class NamedMember : BlockMember  // TODO: consider deletion if no pretty printing will be created
    {
        public string name { get; }
        public LinkedList<BlockMember> members { get; }

        internal NamedMember(string name, LinkedList<BlockMember> members)
        {
            this.name = name;
            this.members = members;
        }
    }
}
