using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
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

        public static bool IsArgumentInObjectCreation(this SyntaxNode node)
        {
            return node?.Parent is ArgumentSyntax
                   && node.Parent?.Parent is ArgumentListSyntax
                   && node.Parent.Parent.Parent is ObjectCreationExpressionSyntax;
        }

        public static IEnumerable<T> DescendantNodes<T>(this SyntaxNode node) where T : SyntaxNode
        {
            return node.DescendantNodes().OfType<T>();
        }

        public static bool IsDescendantOfUsingDeclaration<T>(this T node) where T : SyntaxNode
        {
            var parent = node.FindParent<UsingStatementSyntax, MethodDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsDescendantOfVariableDeclarator(this SyntaxNode node)
        {
            return node.Parent?.Parent is VariableDeclaratorSyntax;
        }

        public static bool IsPartOfReturn(this SyntaxNode node)
        {
            return node.FindParent<ReturnStatementSyntax, MethodDeclarationSyntax>() != null;
        }

        public static bool IsFieldDeclaration(this SyntaxNode node)
        {
            var parent = node.FindParent<FieldDeclarationSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsPartOfAssignmentExpression(this SyntaxNode node)
        {
            var parent = node.FindParent<AssignmentExpressionSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsPropertyDeclaration(this SyntaxNode node)
        {
            var parent = node.FindParent<PropertyDeclarationSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsLocalDeclaration(this SyntaxNode node)
        {
            var parent = node.FindParent<LocalDeclarationStatementSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool TryFindContainingMethod(this SyntaxNode node, out MethodDeclarationSyntax method)
        {
            method = node.FindParent<MethodDeclarationSyntax, ConstructorDeclarationSyntax>();

            return method != null;
        }

        public static bool FindContainingConstructor(this SyntaxNode node, out ConstructorDeclarationSyntax ctor)
        {
            ctor = node.FindParent<ConstructorDeclarationSyntax, MethodDeclarationSyntax>();
            return ctor != null;
        }

        public static bool TryFindContainingConstructorOrMethod(this SyntaxNode node, out SyntaxNode ctorOrMethod)
        {
            ctorOrMethod = node;
            while (ctorOrMethod != null &&
                   !(ctorOrMethod is MethodDeclarationSyntax || ctorOrMethod is ConstructorDeclarationSyntax))
            {
                ctorOrMethod = ctorOrMethod.Parent;
            }
            return ctorOrMethod != null;
        }

        private static TOut FindParent<TOut, TBreak>(this SyntaxNode node)
            where TBreak : SyntaxNode
            where TOut : SyntaxNode
        {
            var temp = node;
            while (true)
            {
                if (temp.Parent == null) return null;
                if (temp.Parent is TBreak) return null;
                var result = temp.Parent as TOut;
                if (result != null)
                    return result;

                temp = temp.Parent;
            }
        }

        public static string GetFullNamespace(this INamespaceOrTypeSymbol @namespace)
        {
            var stack = new Stack<string>();
            while (@namespace.ContainingNamespace != null)
            {
                stack.Push(@namespace.Name);
                @namespace = @namespace.ContainingNamespace;
            }
            return string.Join(".", stack);
        }

        /// <summary>
        /// Searches for inner SyntaxNode of the ArgumentSyntax at given position.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static SyntaxNode GetContentArgumentAtPosition(this ObjectCreationExpressionSyntax node, int position)
        {
            if (node.ArgumentList.Arguments.Count < position) return null;

            return node.ArgumentList.Arguments[position].DescendantNodes().FirstOrDefault();
        }
    }
}