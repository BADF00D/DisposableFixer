using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class InvocationExpressionSyntaxExtension
    {
        public static bool IsCallToDispose(this InvocationExpressionSyntax node, string identifier)
        {
            if (node.Parent is ConditionalAccessExpressionSyntax)
            {
                var condAccess = node.Parent as ConditionalAccessExpressionSyntax;
                var identifierSyntax = condAccess.Expression as IdentifierNameSyntax;
                if (identifierSyntax == null) return false;
                var expression = node.Expression as MemberBindingExpressionSyntax;

                return identifierSyntax.Identifier.Text == identifier
                       && expression.Name.Identifier.Text == "Dispose";
            }
            else
            {
                var expression = node.Expression as MemberAccessExpressionSyntax;

                var identifierSyntax = expression?.Expression as IdentifierNameSyntax;
                if (identifierSyntax == null) return false;

                return identifierSyntax.Identifier.Text == identifier
                       && expression.Name.Identifier.Text == "Dispose";
            }
        }
    }
}