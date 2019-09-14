using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class ObjectCreationExpressionSyntaxExtension
    {
        /// <summary>
        ///     Searches for inner SyntaxNode of the ArgumentSyntax at given position.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static SyntaxNode GetContentArgumentAtPosition(this ObjectCreationExpressionSyntax node, int position) {
            return node.ArgumentList.Arguments.Count < position
                ? null
                : node.ArgumentList.Arguments[position].DescendantNodes().FirstOrDefault();
        }

        public static bool HasArgumentWithName(this ObjectCreationExpressionSyntax oces, string variableName)
        {
            return oces?.ArgumentList.HasArgumentWithName(variableName) ?? false;
        }
    }
}