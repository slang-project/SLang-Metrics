using System.Collections.Generic;
using SLangMetrics;

namespace SLangUnits
{
    internal class CompilationUnit
    {
        List<BlockMember> blockMembers;
        CompilationUnit(List<BlockMember> members) {
            this.blockMembers = members;

            Program.parsedProgram = this;
        }
    }

    internal abstract class BlockMember {
        
    }

    internal abstract class Statement: BlockMember{
        
    }

    internal class IfStatement: Statement {
        public IfStatement() {
            System.Console.WriteLine(this.GetType().Name);
        }
    }

    internal class WhileStatement: Statement {
        public WhileStatement() {
            System.Console.WriteLine(this.GetType().Name);
        }
    }

    internal abstract class Declaration: BlockMember {

    }

    internal class RoutineDeclaration: Declaration {
        public RoutineDeclaration() {
            System.Console.WriteLine(this.GetType().Name);
        }
    }

    internal class UnitDeclaration: Declaration {
        public UnitDeclaration() {
            System.Console.WriteLine(this.GetType().Name);
        }
    }

    internal class VariableDeclaration: Declaration {
        public VariableDeclaration() {
            System.Console.WriteLine(this.GetType().Name);
        }
    }
}
