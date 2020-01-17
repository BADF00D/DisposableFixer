using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class TypeSyntaxExtensions
    {
        public static bool IsVoid(this TypeSyntax typeSyntax)
        {
            return typeSyntax is PredefinedTypeSyntax pts
                   && pts.Keyword.Kind() == SyntaxKind.VoidKeyword;
        }
    }
}