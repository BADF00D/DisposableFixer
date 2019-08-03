using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class PropertyDeclarationSyntaxExtensions
    {
        public static bool IsStatic(this PropertyDeclarationSyntax pds)
        {
            return pds.Modifiers.Any(SyntaxKind.StaticKeyword);
        }
    }
}