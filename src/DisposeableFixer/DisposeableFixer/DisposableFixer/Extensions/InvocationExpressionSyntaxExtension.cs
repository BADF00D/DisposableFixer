using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class InvocationExpressionSyntaxExtension
    {
        public static bool IsCallToDisposeFor(this InvocationExpressionSyntax node, string identifier)
        {
            var syntax = node.Parent as ConditionalAccessExpressionSyntax;
            if (syntax != null)
            {
                var condAccess = syntax;
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

        public static bool IsCallToDispose(this InvocationExpressionSyntax node) {
            var syntax = node.Parent as ConditionalAccessExpressionSyntax;
            if (syntax != null) {
                var condAccess = syntax;
                var identifierSyntax = condAccess.Expression as InvocationExpressionSyntax;
                if (identifierSyntax == null) return false;
                var expression = node.Expression as MemberBindingExpressionSyntax;

                return expression.Name.Identifier.Text == "Dispose";
            } else {
                var expression = node.Expression as MemberAccessExpressionSyntax;

                var identifierSyntax = expression?.Expression as IdentifierNameSyntax;
                if (identifierSyntax == null) return false;

                return expression.Name.Identifier.Text == "Dispose";
            }
        }
    }
}