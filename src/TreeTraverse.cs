using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageElements
{
    class InheritanceLoopDetectedException : Exception
    {
        public InheritanceLoopDetectedException() : base() { }
    }

    internal class InheritanceWrapper
    {
        private Dictionary<string, InheritanceNode> nodeMap;
        private LinkedList<InheritanceNode> rootNodes;
        private LinkedList<InheritanceNode> leafNodes;

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

        public void visitRecursive(int footprint)
        {
            if (visitorFootprint.Contains(footprint))
            {
                throw new InheritanceLoopDetectedException();
            }
            this.visitorFootprint.Add(footprint);
            foreach (InheritanceNode node in this.children ?? Enumerable.Empty<InheritanceNode>())
            {
                node.visitRecursive(footprint);
            }
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
        public LinkedList<int> bottomHeights;
        public int maxHH { get; }
        public double averageHH { get; }


        public Traverse(Module module)
        {
            unitList = new LinkedList<UnitDeclaration>();
            bottomHeights = new LinkedList<int>();
            CompoundName outerScope = new CompoundName();

            foreach (BlockMember child in module.members ?? Enumerable.Empty<BlockMember>())
            {
                traverse(child, ref unitList, outerScope, ref bottomHeights, 1);
            }

            maxHH = 0;
            averageHH = 0;

            foreach (int hh in bottomHeights ?? Enumerable.Empty<int>())
            {
                if (hh > maxHH)
                {
                    maxHH = hh;
                }
                averageHH += hh;
            }

            if (bottomHeights.Any())
            {
                averageHH /= bottomHeights.Count;
            }
        }

        private void traverse(BlockMember obj, ref LinkedList<UnitDeclaration> unitList, CompoundName outerScope, ref LinkedList<int> bottomHeights, int outerHeight)
        {
            Block block = obj as Block;
            if (block != null)
            {
                ++outerHeight;
                if (!block.members.Any())
                {
                    bottomHeights.AddLast(outerHeight);
                }

                foreach (BlockMember child in block.members ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(child, ref unitList, outerScope, ref bottomHeights, outerHeight);
                }
                return;
            }

            UnitDeclaration unitDecl = obj as UnitDeclaration;
            if (unitDecl != null)
            {
                ++outerHeight;
                if (!unitDecl.members.Any())
                {
                    bottomHeights.AddLast(outerHeight);
                }

                unitDecl.name.AppendFront(outerScope);
                unitList.AddLast(unitDecl);
                foreach (BlockMember child in unitDecl.members ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(child, ref unitList, unitDecl.name, ref bottomHeights, outerHeight);
                }
                return;
            }

            RoutineDeclaration routineDecl = obj as RoutineDeclaration;
            if (routineDecl != null)
            {
                routineDecl.name.AppendFront(outerScope);
                traverse(routineDecl.routineBlock, ref unitList, routineDecl.name, ref bottomHeights, outerHeight);
                return;
            }

            VariableDeclaration variableDecl = obj as VariableDeclaration;
            if (variableDecl != null)
            {
                ++outerHeight;
                bottomHeights.AddLast(outerHeight);
                return;
            }

            IfStatement ifStmnt = obj as IfStatement;
            if (ifStmnt != null)
            {
                traverse(ifStmnt.mainBlock, ref unitList, outerScope, ref bottomHeights, outerHeight);
                foreach (Block elsifBlock in ifStmnt.elsifBlockList ?? Enumerable.Empty<BlockMember>())
                {
                    traverse(elsifBlock, ref unitList, outerScope, ref bottomHeights, outerHeight);
                }
                traverse(ifStmnt.elseBlock, ref unitList, outerScope, ref bottomHeights, outerHeight);
                return;
            }

            LoopStatement loopStmnt = obj as LoopStatement;
            if (loopStmnt != null)
            {
                traverse(loopStmnt.loopBlock, ref unitList, outerScope, ref bottomHeights, outerHeight);
                return;
            }

        }
    }
}
