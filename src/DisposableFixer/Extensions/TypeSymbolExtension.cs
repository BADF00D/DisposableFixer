using System.Linq;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.Extensions
{
    public static class TypeSymbolExtension
    {
        private const string DisposableInterface = "IDisposable";

        public static bool IsDisposeableOrImplementsDisposable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsIDisposable() || typeSymbol.ImplementsIDisposable();
        }

        private static bool IsIDisposable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.Name == DisposableInterface;
        }

        private static bool ImplementsIDisposable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.AllInterfaces.Any(i => i.Name == DisposableInterface);
        }
        public static string GetVariableName(this ITypeSymbol typeSymbol)
        {
            var name = typeSymbol.MetadataName.ToCharArray();
            return name[0].ToString().ToLower()[0] + typeSymbol.MetadataName.Substring(1);
        }
    }
}