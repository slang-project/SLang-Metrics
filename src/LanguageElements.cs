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
        internal CompoundName name { get; }
        internal string aliasName { get; }
        internal Block routineBlock { get; }

        public RoutineDeclaration(string name, string aliasName, Block routineBlock)
        {
            this.name = new CompoundName();
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
        internal LinkedList<Block> elsifBlockList { get; }
        internal Block elseBlock { get; }

        public IfStatement(Block mainBlock, LinkedList<Block> elsifBlockList, Block elseBlock)
        {
            this.mainBlock = mainBlock;
            this.elsifBlockList = elsifBlockList;
            this.elseBlock = elseBlock;
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

    internal abstract class Type {}

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
