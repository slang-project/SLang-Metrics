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

    internal class InheritanceWrapper
    {
        private Dictionary<string, InheritanceNode> nodeMap;
        private LinkedList<InheritanceNode> rootNodes;
        private LinkedList<InheritanceNode> leafNodes;

        private string treeImage;
        private int maxHierarchyHeight;
        private double averageHierarchyHeight;
        private InheritanceNode topNode;

        public class NonExistantUnitException : Exception
        {
            public NonExistantUnitException() : base() { }
        }

        public InheritanceWrapper(LinkedList<UnitDeclaration> unitList)
        {
            nodeMap = new Dictionary<string, InheritanceNode>();
            rootNodes = new LinkedList<InheritanceNode>();
            leafNodes = new LinkedList<InheritanceNode>();

            foreach (UnitDeclaration ud in unitList ?? Enumerable.Empty<UnitDeclaration>())
            {
                string unitName = ud.name.ToString();
                if (!nodeMap.ContainsKey(unitName))
                {
                    nodeMap[unitName] = new InheritanceNode(unitName);
                }
                foreach (UnitName parent in ud.parents ?? Enumerable.Empty<UnitName>())
                {
                    string parentName = parent.name.ToString();

                    if (!nodeMap.ContainsKey(parentName))
                    {
                        nodeMap[parentName] = new InheritanceNode(parentName);
                    }

                    nodeMap[parentName].addChild(nodeMap[unitName]);
                }
            }

            foreach (InheritanceNode node in nodeMap.Values ?? Enumerable.Empty<InheritanceNode>())
            {
                if (!node.parents.Any())
                {
                    rootNodes.AddLast(node);
                }
                if (!node.children.Any())
                {
                    leafNodes.AddLast(node);
                }
            }

            topNode = new InheritanceNode("Any");
            foreach (InheritanceNode node in rootNodes ?? Enumerable.Empty<InheritanceNode>())
            {
                topNode.addChild(node);
            }

            topNode.findDescendants();
            topNode.initPathPropogation();

            this.maxHierarchyHeight = 0;
            this.averageHierarchyHeight = 0;

            foreach (InheritanceNode node in leafNodes ?? Enumerable.Empty<InheritanceNode>())
            {
                int nodeHeight = node.getMaxHierarchyHeight();
                if (nodeHeight > maxHierarchyHeight)
                {
                    maxHierarchyHeight = nodeHeight;
                }
                averageHierarchyHeight += nodeHeight;
            }
            if (leafNodes.Any())
            {
                averageHierarchyHeight /= leafNodes.Count;
            }
        }

        public int getDescendantsCount(string className)
        {
            if (nodeMap.ContainsKey(className))
            {
                return nodeMap[className].descendants.Count;
            }
            else
            {
                throw new NonExistantUnitException();
            }
        }

        public int getHierachyHeight(string className)
        {
            if (nodeMap.ContainsKey(className))
            {
                return nodeMap[className].getMaxHierarchyHeight();
            }
            else
            {
                throw new NonExistantUnitException();
            }
        }

        public LinkedList<string> getHierarchyPaths(string className)
        {
            if (nodeMap.ContainsKey(className))
            {
                return nodeMap[className].getInheritancePaths();
            }
            else
            {
                throw new NonExistantUnitException();
            }
        }

        public int getMaxHierarchyHeight()
        {
            return maxHierarchyHeight;
        }

        public double getAverageHierarchyHeight()
        {
            return averageHierarchyHeight;
        }

        public void printTreeRepresentation()
        {
            topNode.printPretty("", true);
        }

        public LinkedList<string> getUnitNames()
        {
            return new LinkedList<string>(this.nodeMap.Keys);
        }
    }

    internal class InheritanceNode
    {
        public string name;
        public LinkedList<InheritanceNode> children;
        public LinkedList<InheritanceNode> parents;
        public HashSet<InheritanceNode> descendants { get; }
        public LinkedList<LinkedList<InheritanceNode>> pathsFromRoot;

        public bool visited;
        public HashSet<int> visitorFootprint;

        public InheritanceNode(string name)
        {
            parents = new LinkedList<InheritanceNode>();
            children = new LinkedList<InheritanceNode>();
            descendants = new HashSet<InheritanceNode>();
            pathsFromRoot = new LinkedList<LinkedList<InheritanceNode>>();

            this.name = name;
            visited = false;
        }

        public void addChild(InheritanceNode node)
        {
            children.AddLast(node);
            node.parents.AddLast(this);
        }

        public void visit()
        {
            this.visited = true;
        }

        public void visit(int footprint)
        {
            this.visited = true;
            this.visitorFootprint.Add(footprint);
        }

        public bool containsFootprint(int footprint)
        {
            return this.visitorFootprint.Contains(footprint);
        }

        public void cleanVisited()
        {
            this.visited = false;
            this.visitorFootprint.Clear();
            foreach (InheritanceNode node in children ?? Enumerable.Empty<InheritanceNode>())
            {
                node.cleanVisited();
            }
        }

        public void printPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("└─");
                indent += "  ";
            }
            else
            {
                Console.Write("├─");
                indent += "│ ";
            }
            Console.WriteLine(name.ToString());

            List<InheritanceNode> convertedChildren = children.ToList();
            for (int i = 0; i < convertedChildren.Count; i++)
            {
                convertedChildren[i].printPretty(indent, i == convertedChildren.Count - 1);
            }
        }

        /// <summary>
        /// Go through inheritance tree and calculate set
        /// of descendants for each node.
        /// After that we can get number of descendants
        /// for each node.
        /// </summary>
        public void findDescendants()
        {
            this.descendants.UnionWith(children);
            foreach (InheritanceNode c in children)
            {
                c.findDescendants();
                this.descendants.UnionWith(c.descendants);
            }
        }

        public void initPathPropogation()
        {
            LinkedList<InheritanceNode> newPath = new LinkedList<InheritanceNode>();
            newPath.AddLast(this);
            pathsFromRoot.AddLast(newPath);
            foreach (InheritanceNode child in this.children ?? Enumerable.Empty<InheritanceNode>())
            {
                child.propogatePathsFromRoot(pathsFromRoot);
            }
        }

        private void propogatePathsFromRoot(LinkedList<LinkedList<InheritanceNode>> pathsFromRootOuter)
        {
            foreach (LinkedList<InheritanceNode> path in pathsFromRootOuter ?? Enumerable.Empty<LinkedList<InheritanceNode>>())
            {
                this.pathsFromRoot.AddLast(path);
            }

            LinkedList<LinkedList<InheritanceNode>> pathsExtended = new LinkedList<LinkedList<InheritanceNode>>();

            foreach (LinkedList<InheritanceNode> path in this.pathsFromRoot ?? Enumerable.Empty<LinkedList<InheritanceNode>>())
            {
                LinkedList<InheritanceNode> pathExtended = new LinkedList<InheritanceNode>();
                foreach (InheritanceNode node in path ?? Enumerable.Empty<InheritanceNode>())
                {
                    pathExtended.AddLast(node);
                }
                pathExtended.AddLast(this);
                pathsExtended.AddLast(pathExtended);
            }

            foreach (InheritanceNode child in this.children ?? Enumerable.Empty<InheritanceNode>())
            {
                child.propogatePathsFromRoot(pathsExtended);
            }
        }
        public int getMaxHierarchyHeight()
        {
            int max = 0;
            foreach (LinkedList<InheritanceNode> path in pathsFromRoot ?? Enumerable.Empty<LinkedList<InheritanceNode>>())
            {
                if (path.Count > max)
                {
                    max = path.Count;
                }
            }
            return max + 1; // consider node itself as part of path
        }

        public LinkedList<string> getInheritancePaths()
        {
            LinkedList<string> paths = new LinkedList<string>();
            foreach (LinkedList<InheritanceNode> path in this.pathsFromRoot ?? Enumerable.Empty<LinkedList<InheritanceNode>>())
            {
                string pathString = "(";
                bool first = true;
                foreach (InheritanceNode node in path ?? Enumerable.Empty<InheritanceNode>())
                {
                    if (!first)
                    {
                        pathString += " -> ";
                    }
                    else
                    {
                        first = false;
                    }
                    pathString += node.name;
                }
                if (!first)
                {
                    pathString += " -> ";
                }
                pathString += this.name + ")";
                paths.AddLast(pathString);
            }
            return paths;
        }
    }

    internal class Traverse
    {
        public LinkedList<UnitDeclaration> unitList;

        public Traverse(Module module)
        {
            unitList = new LinkedList<UnitDeclaration>();
            CompoundName outerScope = new CompoundName();

            foreach (BlockMember child in module.members ?? Enumerable.Empty<BlockMember>())
            {
                traverse(child, ref unitList, outerScope);
            }
        }

        private void traverse(BlockMember obj, ref LinkedList<UnitDeclaration> unitList, CompoundName outerScope)
        {
            Block block = obj as Block;
            if (block != null)
            {
                foreach (BlockMember child in block.members ?? Enumerable.Empty<BlockMember>())
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
                foreach (BlockMember child in unitDecl.members ?? Enumerable.Empty<BlockMember>())
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
                foreach (Block elsifBlock in ifStmnt.elsifBlockList ?? Enumerable.Empty<BlockMember>())
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
        internal VariableDeclaration()
        {
        }
    }

    abstract class Statement : BlockMember
    {
    }


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
            internal WrongParentUnitNameException() : base()
            {
            }
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
