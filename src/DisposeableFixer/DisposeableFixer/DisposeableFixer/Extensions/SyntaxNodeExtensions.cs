using System.Collections.Generic;
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
                .Any(us =>
                {
                    //analysis for this using end when the BlockSyntax begins
                    var descendantNodes = us.DescendantNodes().TakeWhile(dn => !(dn is BlockSyntax)).ToArray();
                    if (descendantNodes.Length == 1)
                    {
                        /* matches when
                         * var memstream = new MemoryStream();
                         * using(memstream){}
                         */
                        return descendantNodes
                            .OfType<IdentifierNameSyntax>()
                            .Any(ins => ins.Identifier.Text == variableName);
                    }
                    /* matches when
                     * using(var memstream = new MemoryStream()){}
                     */
                    return descendantNodes
                        .TakeWhile(dn => !(dn is BlockSyntax))
                        .OfType<VariableDeclaratorSyntax>()
                        .Any(variableDeclaratorSyntax => variableDeclaratorSyntax.Identifier.Text == variableName);
                });
        }

        public static IEnumerable<T> DescendantNodes<T>(this SyntaxNode node) where T : SyntaxNode
        {
            return node.DescendantNodes().OfType<T>();
        }
    }
}