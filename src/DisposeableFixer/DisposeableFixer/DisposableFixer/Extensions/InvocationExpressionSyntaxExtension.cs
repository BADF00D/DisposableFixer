using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    internal static class InvocationExpressionSyntaxExtension
    {
        public static bool IsCallToDisposeFor(this InvocationExpressionSyntax node, string identifier, SemanticModel semanticModel, IConfiguration configuration)
        {
            var syntax = node.Parent as ConditionalAccessExpressionSyntax;
            if (syntax != null)
            {
                /** indentifier?.Dispose or (identifier as IDisposbale)?.Dispose **/
                var condAccess = syntax;
                var identifierSyntax = condAccess.Expression as IdentifierNameSyntax;
                if (identifierSyntax != null)
                {
                    var expression = node.Expression as MemberBindingExpressionSyntax;

                    if (identifierSyntax.Identifier.Text == identifier
                        && expression.Name.Identifier.Text == "Dispose") return true;

                    var memberType = semanticModel.GetTypeInfo(identifierSyntax).Type.GetFullNamespace();
                    IReadOnlyCollection<MethodCall> _specialDispose;
                    if (!configuration.DisposingMethodsAtSpecialClasses.TryGetValue(memberType, out _specialDispose))
                        return false;

                    var partlyEqual = _specialDispose.Any(mc =>
                        !mc.IsStatic
                        && mc.Name == expression.Name.Identifier.Text
                        && mc.Parameter.Length == node.ArgumentList.Arguments.Count);
                    /* We have to check the parameter types, but unfortunatelly such an example and unittest does not exists jet.
                       For now, we have enought information */
                    return partlyEqual;
                }
                var parenthesizedExpressionSyntax = condAccess.Expression as ParenthesizedExpressionSyntax;
                if (parenthesizedExpressionSyntax == null) return false;
                {
                    var binaryExpression = parenthesizedExpressionSyntax.Expression as BinaryExpressionSyntax;
                    if (binaryExpression == null) return false;
                    return (binaryExpression.Left as IdentifierNameSyntax)?.Identifier.Text == identifier;
                }
            }
            else
            {
                var expression = node.Expression as MemberAccessExpressionSyntax;

                var identifierSyntax = expression?.Expression as IdentifierNameSyntax;
                if (identifierSyntax == null) return false;

                if(identifierSyntax.Identifier.Text == identifier && expression.Name.Identifier.Text == "Dispose") return true;

                
                var memberType = semanticModel.GetTypeInfo(identifierSyntax).Type.GetFullNamespace();
                IReadOnlyCollection<MethodCall> _specialDispose;
                if (!configuration.DisposingMethodsAtSpecialClasses.TryGetValue(memberType, out _specialDispose))
                    return false;

                var partlyEqual = _specialDispose.Any(mc => 
                    !mc.IsStatic 
                    && mc.Name == expression.Name.Identifier.Text
                    && mc.Parameter.Length == node.ArgumentList.Arguments.Count);
                /* We have to check the parameter types, but unfortunatelly such an example and unittest does not exists jet.
                       For now, we have enought information */
                return partlyEqual;
            }
        }

        public static bool IsCallToDispose(this InvocationExpressionSyntax node) {
            var syntax = node.Parent as ConditionalAccessExpressionSyntax;
            if (syntax != null) {
                var mbe = node.Expression as MemberBindingExpressionSyntax;
                return mbe?.Name.Identifier.Text == "Dispose";
            }
            var expression = node.Expression as MemberAccessExpressionSyntax;

            var identifierSyntax = expression?.Expression as IdentifierNameSyntax;
            if (identifierSyntax == null) return false;

            return expression.Name.Identifier.Text == "Dispose";
        }

        public static bool UsesVariableInArguments(this InvocationExpressionSyntax invocationExpression, string variable)
        {
            return invocationExpression.ArgumentList.Arguments
                .Select(arg => arg.Expression as IdentifierNameSyntax)
                .Any(identifier => identifier?.Identifier.Text == variable);
        }
    }
}