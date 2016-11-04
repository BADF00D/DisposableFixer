using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposeableFixer.Extensions
{
    public static class VariableDeclaratorSyntaxExtensions
    {
        public static MethodDeclarationSyntax FindContainingMethod(this VariableDeclaratorSyntax node)
        {
            return node.FindParent<MethodDeclarationSyntax, ConstructorDeclarationSyntax>();
        }

        public static ConstructorDeclarationSyntax FindContainingConstructor(this VariableDeclaratorSyntax node)
        {
            return node.FindParent<ConstructorDeclarationSyntax, MethodDeclarationSyntax>();
        }

        private static TOut FindParent<TOut, TBreak>(this SyntaxNode node) where TBreak : SyntaxNode where TOut : SyntaxNode
        {
            var temp = node;
            while (true) {
                if (temp.Parent == null) return null;
                if (temp.Parent is TBreak) return null;
                var result = temp.Parent as TOut;
                if (result != null)
                    return result;

                temp = temp.Parent;
            }
        }
    }
}