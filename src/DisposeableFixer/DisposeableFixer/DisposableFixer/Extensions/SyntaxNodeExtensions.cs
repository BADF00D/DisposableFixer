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

        /// <summary>
        ///     True if is
        ///     using(memStream)
        ///     or using(new MemoryStream())
        ///     or using(var memstream = new MemoryStream()){}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsDescendantOfUsingHeader<T>(this T node) where T : SyntaxNode
        {
            return node?.Parent is UsingStatementSyntax //using(memStream) or using(new MemoryStream())
                   ||
                   (node?.Parent?.Parent?.Parent is VariableDeclarationSyntax &&
                    node?.Parent?.Parent?.Parent?.Parent is UsingStatementSyntax);
        }

        public static bool IsPartOfVariableDeclaratorInsideAUsingDeclaration<T>(this T node) where T : SyntaxNode
        {
            return node?.Parent is EqualsValueClauseSyntax
                   && node?.Parent?.Parent is VariableDeclaratorSyntax
                   && node?.Parent?.Parent?.Parent is VariableDeclarationSyntax
                   && node?.Parent?.Parent?.Parent?.Parent is UsingStatementSyntax;
        }

        public static bool IsDescendantOfVariableDeclarator(this SyntaxNode node)
        {
            return node.Parent?.Parent is VariableDeclaratorSyntax;
        }

        public static bool IsPartOfReturnStatement(this SyntaxNode node)
        {
            return node.FindParent<ReturnStatementSyntax, MethodDeclarationSyntax>() != null;
        }

        public static bool IsPartOfSimpleLambdaExpression(this SyntaxNode node)
        {
            return node?.Parent is SimpleLambdaExpressionSyntax;
        }

        public static bool IsPartOfParenthesizedExpression(this SyntaxNode node)
        {
            return node?.Parent is ParenthesizedLambdaExpressionSyntax;
        }

        public static bool IsReturnValueInLambdaExpression(this SyntaxNode node)
        {
            return node.IsPartOfSimpleLambdaExpression() || node.IsPartOfParenthesizedExpression();
        }

        public static bool IsPartOfAwaitExpression(this SyntaxNode node)
        {
            return node.Parent is AwaitExpressionSyntax;
        }

        /// <summary>
        ///     Checks if node is part of a VariableDeclaratorSyntax that is return later within method body.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsReturnedLaterWithinMethod(this SyntaxNode node)
        {
            var method = node.FindParent<MethodDeclarationSyntax, ClassDeclarationSyntax>();
            if (method?.ReturnType == null) return false; // no method or ReturnType found

            var identifier = node.GetIdentifierIfIsPartOfVariableDeclarator();
            return method.Returns(identifier);
        }

        public static bool Returns(this MethodDeclarationSyntax method, string name)
        {
            if (name == null) return false;
            return method.DescendantNodes<ReturnStatementSyntax>()
                .Select(rss => rss.DescendantNodes<IdentifierNameSyntax>())
                .Any(inss => inss.Any(ins => ins.Identifier.Text == name));
        }

        public static bool IsDisposedInDisposedMethod(this SyntaxNode nodeInClass, string nameOfVariable)
        {
            var classDeclarationSyntax = nodeInClass.FindContainingClass();
            if (classDeclarationSyntax == null) return false;

            var disposeMethods = classDeclarationSyntax
                .DescendantNodes<MethodDeclarationSyntax>()
                .Where(mds => mds.IsDisposeMethod())
                .ToArray();
            return disposeMethods
                .SelectMany(disposeMethod => disposeMethod.DescendantNodes<InvocationExpressionSyntax>())
                .Any(mes => mes.IsCallToDisposeFor(nameOfVariable));
        }

        public static bool ContainsDisposeCallFor(this SyntaxNode node, string name)
        {
            return node
                .DescendantNodes<InvocationExpressionSyntax>()
                .Any(ies => ies.IsCallToDisposeFor(name));
        }

        /// <summary>
        ///     Gets the identifier of the VariableDeclaratorSyntax, where given InvocationExpressionSyntax stores its value.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Identfier if parent is EqualsValueClauseSyntax and parent of parent is VariableDeclaratorSyntax</returns>
        public static string GetIdentifierIfIsPartOfVariableDeclarator(this SyntaxNode node)
        {
            if (!(node.Parent is EqualsValueClauseSyntax)) return null;
            var variableDeclaratorSyntax = node.Parent?.Parent as VariableDeclaratorSyntax;
            return variableDeclaratorSyntax?.Identifier.Text;
        }


        public static bool IsFieldDeclaration(this SyntaxNode node)
        {
            var parent = node.FindParent<FieldDeclarationSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsParentADisposeCallIgnoringParenthesis(this SyntaxNode node)
        {
            var parent = node.Parent;
            while (true)
            {
                if (parent == null
                    || parent is MethodDeclarationSyntax
                    || parent is PropertyDeclarationSyntax
                    || parent is ConstructorDeclarationSyntax)
                    return false;
                if (parent is ParenthesizedExpressionSyntax)
                {
                    parent = parent.Parent;
                    continue;
                }
                var memberAccessExpressionSyntax = parent as MemberAccessExpressionSyntax;
                if (memberAccessExpressionSyntax != null)
                    return memberAccessExpressionSyntax.Name.Identifier.Text == "Dispose";

                parent = parent.Parent;
            }
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
            method = node.FindParent<MethodDeclarationSyntax, ClassDeclarationSyntax>();

            return method != null;
        }

        public static bool TryFindContainingPropery(this SyntaxNode node, out PropertyDeclarationSyntax method)
        {
            method = node.FindParent<PropertyDeclarationSyntax, ClassDeclarationSyntax>();

            return method != null;
        }

        public static bool TryFindContainingCtor(this SyntaxNode node, out ConstructorDeclarationSyntax ctor)
        {
            ctor = node.FindParent<ConstructorDeclarationSyntax, ClassDeclarationSyntax>();

            return ctor != null;
        }

        public static bool HasDecendentVariableDeclaratorFor(this SyntaxNode node, string name)
        {
            return node.DescendantNodes<VariableDeclaratorSyntax>()
                .Select(vds => vds.Identifier.Value as string)
                .Any(identifier => identifier == name);
        }


        public static bool FindContainingConstructor(this SyntaxNode node, out ConstructorDeclarationSyntax ctor)
        {
            ctor = node.FindParent<ConstructorDeclarationSyntax, MethodDeclarationSyntax>();
            return ctor != null;
        }

        public static bool TryFindParentOfType<T>(this SyntaxNode start, SyntaxNode @break, out SyntaxNode scope)
            where T : SyntaxNode
        {
            while (true)
            {
                if (start is T)
                {
                    scope = start;
                    return true;
                }
                if (start == @break)
                {
                    scope = null;
                    return false;
                }
                if (start.Parent == null)
                {
                    scope = null;
                    return false;
                }
                start = start.Parent;
            }
        }

        public static bool TryFindParentScope(this SyntaxNode node, out SyntaxNode parentScope)
        {
            ConstructorDeclarationSyntax ctor;
            if (node.TryFindContainingCtor(out ctor))
            {
                if (node.TryFindParentOfType<SimpleLambdaExpressionSyntax>(ctor, out parentScope))
                {
                    return true;
                }
                if (node.TryFindParentOfType<ParenthesizedLambdaExpressionSyntax>(ctor, out parentScope))
                {
                    return true;
                }
                parentScope = ctor;
                return true;
            }
            MethodDeclarationSyntax method;
            if (node.TryFindContainingMethod(out method))
            {
                if (node.TryFindParentOfType<SimpleLambdaExpressionSyntax>(method, out parentScope))
                {
                    return true;
                }
                if (node.TryFindParentOfType<ParenthesizedLambdaExpressionSyntax>(method, out parentScope))
                {
                    return true;
                }
                parentScope = method;
                return true;
            }
            PropertyDeclarationSyntax property;
            if (node.TryFindContainingPropery(out property))
            {
                if (node.TryFindParentOfType<SimpleLambdaExpressionSyntax>(property, out parentScope))
                {
                    return true;
                }
                if (node.TryFindParentOfType<ParenthesizedLambdaExpressionSyntax>(property, out parentScope))
                {
                    return true;
                }
                parentScope = property;
                return true;
            }
            parentScope = null;
            return false;
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
        ///     Searches for inner SyntaxNode of the ArgumentSyntax at given position.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static SyntaxNode GetContentArgumentAtPosition(this ObjectCreationExpressionSyntax node, int position)
        {
            return node.ArgumentList.Arguments.Count < position
                ? null
                : node.ArgumentList.Arguments[position].DescendantNodes().FirstOrDefault();
        }
    }
}