using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    internal static class ExpressionSyntaxExtension
    {
        public static ITypeSymbol GetTypeSymbol(this ExpressionSyntax es, SemanticModel semanticModel)
        {
            switch(es)
            {
                case ObjectCreationExpressionSyntax oce: return (semanticModel.GetSymbolInfo(es).Symbol as IMethodSymbol)?.ReceiverType;
                case InvocationExpressionSyntax ie: return (semanticModel.GetSymbolInfo(es).Symbol as IMethodSymbol)?.ReceiverType;
            }
            return null;
        }
    }
}
