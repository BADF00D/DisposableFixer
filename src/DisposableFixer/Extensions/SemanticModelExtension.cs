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
    }
}