using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposeableFixer.Extensions
{
    public static class VariableDeclaratorSyntaxExtensions
    {
        public static MethodDeclarationSyntax FindContainingMethod(this VariableDeclaratorSyntax node)
        {
            var temp = node as SyntaxNode;
            while (true) {
                if (temp.Parent == null) return null;
                if (temp.Parent is ConstructorDeclarationSyntax) return null;
                var method = temp.Parent as MethodDeclarationSyntax;
                if (method != null)
                    return method;

                temp = temp.Parent;
            }
        }

        public static ConstructorDeclarationSyntax FindContainingConstructor(this VariableDeclaratorSyntax node) {
            var temp = node as SyntaxNode;
            while (true) {
                if (temp.Parent == null) return null;
                if (temp.Parent is MethodDeclarationSyntax) return null; 
                var ctor = temp.Parent as ConstructorDeclarationSyntax;
                if (ctor != null)
                    return ctor;

                temp = temp.Parent;
            }
        }


    }
}