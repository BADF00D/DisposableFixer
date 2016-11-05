using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposeableFixer.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static ClassDeclarationSyntax FindContainingClass(this SyntaxNode node)
        {
            while (true)
            {
                if (node.Parent == null) return null;
                var @class = node.Parent as ClassDeclarationSyntax;
                if (@class != null)
                    return @class;

                node = node.Parent;
            }
        }

        public static bool ContainsUsingsOfVariableNamed(this SyntaxNode node, string variableName)
        {
            return node.DescendantNodes()
                .OfType<UsingStatementSyntax>()
                .Any(us => {
                    var descendantNodes = us.DescendantNodes().ToArray();
                    return descendantNodes.OfType<IdentifierNameSyntax>().Count(id => id.Identifier.Text == variableName) >
                           0;
                });
        }
    }
}