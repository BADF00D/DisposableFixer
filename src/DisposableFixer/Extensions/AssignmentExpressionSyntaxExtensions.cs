using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class AssignmentExpressionSyntaxExtensions
    {
        public static bool IsAssignmentToLocalVariable(this AssignmentExpressionSyntax aes, string variableName)
        {
            return aes.Left is IdentifierNameSyntax ins && ins.Identifier.Text == variableName;
        }
    }
}