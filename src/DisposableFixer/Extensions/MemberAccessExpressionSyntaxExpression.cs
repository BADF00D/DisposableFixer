using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class MemberAccessExpressionSyntaxExpression
    {
        public static bool IsDisposeCall(this MemberAccessExpressionSyntax memberAccessExpression)
        {
            return memberAccessExpression.Name.Identifier.Text == "Dispose";
        } 
    }
}