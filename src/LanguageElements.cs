using System;
using System.Collections.Generic;
using System.Linq;
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

        public LinkedList<BlockMember> getMembers()
        {
            return members;
        }

        public string getName()
        {
            return "module";
        }

        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(getName());

            List<BlockMember> members = getMembers().ToList();
            for (int i = 0; i < members.Count; i++)
            {
                if(members[i] != null) 
                {
                    members[i].PrintPretty(indent, i == members.Count - 1);
                }
            }
        }


    }

    internal abstract class BlockMember
    {
        public abstract LinkedList<BlockMember> getMembers();
        public abstract string getName();

        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(getName());

            List<BlockMember> members = getMembers().ToList();

            for (int i = 0; i < members.Count; i++)
            {
                if(members[i] != null) 
                {
                    members[i].PrintPretty(indent, i == members.Count - 1);
                }
            }
        }
    }

    internal class Block : BlockMember
    {
        
        internal LinkedList<BlockMember> members { get; }

        internal Block(LinkedList<BlockMember> members)
        {
            this.members = members;
        }

        public override LinkedList<BlockMember> getMembers()
        {
            return members;
        }

        public override string getName()
        {
            return "block";
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

        public override LinkedList<BlockMember> getMembers()
        {
            IEnumerable<BlockMember> decls = members.Select(str => (BlockMember) str);
            LinkedList<BlockMember> blockMembers = new LinkedList<BlockMember>(decls);
            return blockMembers;
        }

        public override string getName()
        {
            return name.ToString();
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

        public override LinkedList<BlockMember> getMembers()
        {
            return routineBlock.getMembers();
        }

        public override string getName()
        {
            return this.name;
        }
    }

    internal class VariableDeclaration : Declaration
    {
        public VariableDeclaration()
        {
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }

        public override LinkedList<BlockMember> getMembers()
        {
            return new LinkedList<BlockMember>();
        }

        public override string getName()
        {
            return "variableDeclaration";  // TODO: delete
        }
    }

    internal abstract class Statement : BlockMember
    {
    }

    internal class IfStatement : Statement
    {
        internal Block mainBlock { get; }
        internal LinkedList<Block> elsifBlockList { get; }
        internal Block elseBlock { get; }

        public IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elsifBlockList = elsifBlockList;
            this.elseBlock = elseBlock;
            System.Console.WriteLine(this.GetType().Name);  // TODO remove
        }

        public override LinkedList<BlockMember> getMembers()
        {
            LinkedList<BlockMember> branchedMembers = new LinkedList<BlockMember>();
            
            branchedMembers.AddLast(new NamedMember("if_true", mainBlock.getMembers()));
            
            foreach(Block block in this.elsifBlockList ?? Enumerable.Empty<Block>())
            {
                branchedMembers.AddLast(new NamedMember("if_elsif", block.getMembers()));
            }

            branchedMembers.AddLast(new NamedMember("if_else", elseBlock.getMembers()));

            return branchedMembers;
        }

        public override string getName()
        {
            return "branch";
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

        public override LinkedList<BlockMember> getMembers()
        {
            return loopBlock.getMembers();
        }

        public override string getName()
        {
            return "loobBlock";
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
            if (type is UnitTypeName)
            {
                this.name = ((UnitTypeName)type).name;
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

        public override string ToString()
        {
            string result = "";
            foreach (string name in names ?? Enumerable.Empty<string>())
            {
                result += name;
            }
            if (result.Equals("")){
                return "emptyUnitName";
            }
            return result;
        }
    }

    internal class NamedMember : BlockMember  // TODO: review how  
    {
        string name;
        LinkedList<BlockMember> members;

        internal NamedMember(string name, LinkedList<BlockMember> members)
        {
            this.name = name;
            this.members = members;
        }

        public override LinkedList<BlockMember> getMembers()
        {
            return members;
        }

        public override string getName()
        {
            return name;
        }
    }
}
