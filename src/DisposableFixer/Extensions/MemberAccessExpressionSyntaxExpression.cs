using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class MemberAccessExpressionSyntaxExpression
    {
        public static bool IsDisposeCall(this MemberAccessExpressionSyntax memberAccessExpression)
        {
            return memberAccessExpression.Name.Identifier.Text == Constants.Dispose;
        }

        public static bool IsDisposeCallFor(this MemberAccessExpressionSyntax memberAccessExpression,
            string variableName)
        {
            return memberAccessExpression != null
                   && memberAccessExpression.IsDisposeCall()
                   && (memberAccessExpression.Expression as IdentifierNameSyntax)?.Identifier.Text == variableName;
        }
    }
}