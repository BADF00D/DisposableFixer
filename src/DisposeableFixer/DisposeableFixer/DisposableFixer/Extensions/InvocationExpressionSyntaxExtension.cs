using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class InvocationExpressionSyntaxExtension
    {
        public static bool IsCallToDispose(this InvocationExpressionSyntax node, string identifier)
        {
            var expression = node.Expression as MemberAccessExpressionSyntax;

            var identifierSyntax = expression?.Expression as IdentifierNameSyntax;
            if (identifierSyntax == null) return false;

            return identifierSyntax.Identifier.Text == identifier
                   && expression.Name.Identifier.Text == "Dispose";
        }
    }
}