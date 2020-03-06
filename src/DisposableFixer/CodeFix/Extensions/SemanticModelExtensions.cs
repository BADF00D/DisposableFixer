using System.Linq;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.CodeFix.Extensions
{
    public static class SemanticModelExtensions
    {
        public static string[] GetExistingNamesInMethod(this SemanticModel semanticModel, int position) => semanticModel.LookupSymbols(position)
            .Where(s => s.Kind == SymbolKind.Local || s.Kind == SymbolKind.Field || s.Kind == SymbolKind.Property)
            .Select(s => s.Name)
            .ToArray();
    }
}