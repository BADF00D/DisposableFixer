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
            if (node.Parent is ConditionalAccessExpressionSyntax syntax)
            {
                /** indentifier?.Dispose or (identifier as IDisposbale)?.Dispose **/
                var condAccess = syntax;
                switch (condAccess.Expression)
                {
                    case IdentifierNameSyntax identifierSyntax:
                        var expression = node.Expression as MemberBindingExpressionSyntax;

                        if (identifierSyntax.Identifier.Text == identifier
                            && expression?.Name.Identifier.Text == "Dispose") return true;

                        var memberType = semanticModel.GetTypeInfo(identifierSyntax).Type.GetFullNamespace();
                        if (!configuration.DisposingMethodsAtSpecialClasses.TryGetValue(memberType, out var specialDispose))
                            return false;

                        var partlyEqual = specialDispose.Any(mc =>
                            !mc.IsStatic
                            && mc.Name == expression?.Name.Identifier.Text
                            && mc.Parameter.Length == node.ArgumentList.Arguments.Count);
                        /* We have to check the parameter types, but unfortunatelly such an example and unittest does not exists jet.
                       For now, we have enought information */
                        return partlyEqual;
                    case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                        if (!(parenthesizedExpressionSyntax.Expression is BinaryExpressionSyntax binaryExpression)) return false;
                        return (binaryExpression.Left as IdentifierNameSyntax)?.Identifier.Text == identifier;
                    case MemberAccessExpressionSyntax mae:
                        //e.g. this.Member.Dispose();
                        var isid = mae.Name.Identifier.Text == identifier;
                        var wnn = condAccess.WhenNotNull as InvocationExpressionSyntax;
                        var mbe = wnn?.Expression as MemberBindingExpressionSyntax;
                        return isid && mbe?.Name.Identifier.Text == Constants.Dispose;
                }

                return false;
            }

            {
                var expression = node.Expression as MemberAccessExpressionSyntax;

                if (!(expression?.Expression is IdentifierNameSyntax identifierSyntax)) return false;
                if (identifierSyntax.Identifier.Text == identifier && expression.Name.Identifier.Text == "Dispose") return true;
                
                var memberType = semanticModel.GetTypeInfo(identifierSyntax).Type.GetFullNamespace();
                if (!configuration.DisposingMethodsAtSpecialClasses.TryGetValue(memberType, out var specialDispose))
                    return false;

                var partlyEqual = specialDispose.Any(mc => //node.IsCallToMethod(mc)
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

        internal static bool IsMaybePartOfMethodChainUsingTrackingExtensionMethod(
            this InvocationExpressionSyntax invocationExpression) {
            return invocationExpression?.Parent is MemberAccessExpressionSyntax
                   && invocationExpression?.Parent?.Parent is InvocationExpressionSyntax;
        }

        internal static bool IsCallToMethod(this InvocationExpressionSyntax invocationExpressionSyntax,
            MethodCall method)
        {
            var memberAccessExpression = invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            var isPartlyCorrect =  
                memberAccessExpression?.Name.Identifier.Text == method.Name
                && invocationExpressionSyntax.ArgumentList.Arguments.Count == method.Parameter.Length;
            if (!isPartlyCorrect) return false;

            //todo check parameres of each ies
            
            return true;

        }
    }
}