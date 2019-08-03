using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class FieldDeclarationSyntaxExtensions
    {
        public static bool IsStatic(this FieldDeclarationSyntax fds)
        {
            return fds.Modifiers.Any(SyntaxKind.StaticKeyword);
        }
    }
}