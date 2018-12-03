using System;
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

    // internal abstract class PrettyPrint
    // {
    //     public abstract PPData accept(PPVisitor visitor);

    //     public void PrintPretty(string indent, bool last)
    //     {
    //         PPData currentData = this.accept(this);

    //         Console.Write(indent);
    //         if (last)
    //         {
    //             Console.Write("\\-");
    //             indent += "  ";
    //         }
    //         else
    //         {
    //             Console.Write("|-");
    //             indent += "| ";
    //         }
    //         Console.WriteLine(getName());

    //         List<BlockMember> members = getMembers().ToList();

    //         for (int i = 0; i < members.Count; i++)
    //         {
    //             if(members[i] != null) 
    //             {
    //                 members[i].PrintPretty(indent, i == members.Count - 1);
    //             }
    //             else
    //             {
    //                 new NamedMember("<unknown element>", new LinkedList<BlockMember>()).PrintPretty(indent, i == members.Count - 1);
    //             }
    //         }
    //     }
    // }


    internal class InheritanceNode
    {
        public CompoundName name;
        public LinkedList<InheritanceNode> children;

        public InheritanceNode(CompoundName name, LinkedList<InheritanceNode> children)
        {
            this.name = name;
            this.children = children;
        }
    }

    internal class Traverse 
    {
        public LinkedList<UnitDeclaration> unitList;
        

        public Traverse(Module module)
        {
            unitList = new LinkedList<UnitDeclaration>();
            CompoundName outerScope = new CompoundName();
            
            foreach(BlockMember child in module.members ?? Enumerable.Empty<BlockMember>())
            {
                traverse(child, ref unitList, outerScope);
            }
        }

        private void traverse(BlockMember obj, ref LinkedList<UnitDeclaration> unitList, CompoundName outerScope)
        {
            Block block  = obj as Block;
            if (block != null)
            {

                foreach(BlockMember child in block.members ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(child, ref unitList, outerScope);
                }
                return;    
            }

            UnitDeclaration unitDecl = obj as UnitDeclaration;
            if (unitDecl != null)
            {
                unitDecl.name.AppendFront(outerScope);
                unitList.AddLast(unitDecl);
                foreach(BlockMember child in unitDecl.members ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(child, ref unitList, unitDecl.name);
                }             
                return;
            }

            RoutineDeclaration routineDecl = obj as RoutineDeclaration;
            if (routineDecl != null)
            {
                routineDecl.name.AppendFront(outerScope);
                traverse(routineDecl.routineBlock, ref unitList, routineDecl.name);
                return;    
            }

            VariableDeclaration variableDecl = obj as VariableDeclaration;
            if (variableDecl != null)
            {
                return;    
            }

            IfStatement ifStmnt = obj as IfStatement;
            if (ifStmnt != null)
            {
                traverse(ifStmnt.mainBlock, ref unitList, outerScope);
                foreach(Block elsifBlock in ifStmnt.elsifBlockList ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(elsifBlock, ref unitList, outerScope);
                }
                traverse(ifStmnt.elseBlock, ref unitList, outerScope);
                return;
            }

            LoopStatement loopStmnt = obj as LoopStatement;
            if (loopStmnt != null)
            {
                traverse(loopStmnt.loopBlock, ref unitList, outerScope);
                return;
            }

        }
    }

    internal abstract class BlockMember
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
        internal CompoundName name { get; }
        internal string aliasName { get; }
        internal Block routineBlock { get; }

        internal RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = new CompoundName();
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
            this.elsifBlockList = elsifBlockList;
            this.elseBlock = elseBlock;
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

        public CompoundName(){
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
            foreach(string n in cName.names ?? Enumerable.Empty<String>())
            {
                buffer.AddLast(n);
            }
            foreach(string n in this.names ?? Enumerable.Empty<String>())
            {
                buffer.AddLast(n);
            }
            this.names = buffer;
        }

        public void AppendBack(CompoundName cName)
        {
            foreach(string n in cName.names ?? Enumerable.Empty<String>())
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
                if(!dotNeeded){
                    result += name;
                    dotNeeded = true;
                }
                else
                {
                    result += "." + name;
                }
            }

            // TODO: delete in production
            if (result.Equals("")){ 
                return "<emptyUnitName>"; 
            }
            return result;
        }
    }

    internal class NamedMember : BlockMember  // TODO: consider deletion if no pretty printing will be created 
    {
        public string name {get;}
        public LinkedList<BlockMember> members {get;}

        internal NamedMember(string name, LinkedList<BlockMember> members)
        {
            this.name = name;
            this.members = members;
        }
    }
}
