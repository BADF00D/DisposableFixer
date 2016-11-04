using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposeableFixer.Extensions
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public static FieldDeclarationSyntax FindFieldNamed(this ClassDeclarationSyntax classDeclarationSyntax, string name)
        {
            return classDeclarationSyntax
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(fds => {
                    return fds
                            .DescendantNodes()
                            .OfType<VariableDeclarationSyntax>()
                            .Count(id => id.Variables.Any(v => v.Identifier.Text == name)) == 1;
                })
                .FirstOrDefault();
        }
    }
}