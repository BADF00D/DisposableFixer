using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class SymanticModelExtension
    {
        public static INamedTypeSymbol GetReturnTypeOf(this SemanticModel model,
            ObjectCreationExpressionSyntax objectCreation)
        {
            var si = model.GetSymbolInfo(objectCreation);
            return (si.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;
        }
    }
}