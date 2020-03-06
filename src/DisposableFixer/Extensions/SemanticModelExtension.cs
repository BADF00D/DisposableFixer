using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class SemanticModelExtension
    {
        /// <summary>
        /// Retrieves the return type as INamedTypeSymbol of given object creation.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="objectCreation"></param>
        /// <returns></returns>
        public static INamedTypeSymbol GetReturnTypeOf(this SemanticModel model,
            ObjectCreationExpressionSyntax objectCreation)
        {
            var si = model.GetSymbolInfo(objectCreation);
            return (si.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;
        }

        public static INamedTypeSymbol GetReturnTypeOf(this SemanticModel model,
            InvocationExpressionSyntax invocationExpression)
        {
            var symbolInfo = model.GetSymbolInfo(invocationExpression);
            var symbol = symbolInfo.Symbol as IMethodSymbol;
            return symbol?.ReturnType as INamedTypeSymbol;
        }

        public static INamedTypeSymbol GetReturnTypeOrDefaultOf(this SemanticModel model,
            ExpressionStatementSyntax expressionStatement)
        {
            switch (expressionStatement.Expression)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    return model.GetReturnTypeOf(invocationExpressionSyntax);
                case ObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                    return model.GetReturnTypeOf(objectCreationExpressionSyntax);
                default: return null;
            }
        }
    }
}