using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.CodeFix
{
    public static class NameGenerator
    {
        public static string ProposeName(string[] existingNames, INamedTypeSymbol namedType)
        {
            var initialName = namedType != null ? namedType.GetVariableName() : Constants.Disposable;
            var finalName = initialName;
            var index = 1;
            while (existingNames.Contains(finalName))
            {
                finalName = initialName + ++index;
            }

            return finalName;
        }
    }
}