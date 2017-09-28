using System;
using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    internal static class SyntaxNodeExtensions
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

        /// <summary>
        /// Returns true, it node is Argument within an ObjectCreationExpression.
        /// <example>new List<IDisposable>(new MemoryStream())</example>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsArgumentInObjectCreation(this SyntaxNode node)
        {
            return node?.Parent is ArgumentSyntax
                   && node.Parent?.Parent is ArgumentListSyntax
                   && node.Parent.Parent.Parent is ObjectCreationExpressionSyntax;
        }

        /// <summary>
        /// Returns true, it node part of an array initializer that is within an ObjectCreationExpression.
        /// <example>new List<IDisposable>(new []{new MemoryStream()})</example>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsPartIfArrayInitializerThatIsPartOfObjectCreation(this SyntaxNode node) {
            return node?.Parent is InitializerExpressionSyntax
                   && (node.Parent?.Parent is ImplicitArrayCreationExpressionSyntax || node.Parent?.Parent is ArrayCreationExpressionSyntax)
                   && node.Parent?.Parent?.Parent is ArgumentSyntax
                   && node.Parent?.Parent?.Parent?.Parent is ArgumentListSyntax
                   && node.Parent?.Parent?.Parent?.Parent?.Parent is ObjectCreationExpressionSyntax;
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

        public static bool IsAssignmentToProperty(this SyntaxNode node, string variableName)
        {
            var @class = node.FindContainingClass();
            if (@class == null) return false;

            return @class
                .DescendantNodes<PropertyDeclarationSyntax>()
                .Any(pd => pd.Identifier.Text == variableName);
        }

        public static bool IsDescendantOfVariableDeclarator(this SyntaxNode node)
        {
            return node.Parent?.Parent is VariableDeclaratorSyntax;
        }

        public static bool IsPartOfReturnStatementInMethod(this SyntaxNode node)
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

        public static bool IsReturnDirectlyOrLater(this SyntaxNode node)
        {
            return node.IsReturnValueInLambdaExpression()
                   || node.IsReturnedLaterWithinParenthesizedLambdaExpression()
                   || node.IsReturnedLaterWithinMethod()
                   || node.IsReturnedInProperty()
                   || node.IsPartOfReturnStatementInMethod();
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

        /// <summary>
        ///     Checks if node is part of a VariableDeclaratorSyntax that is return later within method body.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsReturnedLaterWithinParenthesizedLambdaExpression(this SyntaxNode node) {
            var localVariable = node.GetIdentifierIfIsPartOfVariableDeclarator();
            var func = node.FindParent<ParenthesizedLambdaExpressionSyntax, ClassDeclarationSyntax>();
            return func != null && func.Returns(localVariable);
        }

        public static bool Returns(this MethodDeclarationSyntax method, string name)
        {
            if (name == null) return false;
            return method.DescendantNodes<ReturnStatementSyntax>()
                .Select(rss => rss.DescendantNodes<IdentifierNameSyntax>())
                .Any(inss => inss.Any(ins => ins.Identifier.Text == name));
        }

        public static bool Returns(this ParenthesizedLambdaExpressionSyntax method, string name) {
            if (name == null) return false;
            return method.DescendantNodes<ReturnStatementSyntax>()
                .Select(rss => rss.DescendantNodes<IdentifierNameSyntax>())
                .Any(inss => inss.Any(ins => ins.Identifier.Text == name));
        }

        /// <summary>
        /// Returns true, if field/property is disposed in Dispose or a DisposingMethod.
        /// </summary>
        /// <param name="nodeInClass">A node within the class. usually the node where analysis started.</param>
        /// <param name="nameOfVariable">Name of the property/field that should be evaluated.</param>
        /// <param name="configuration"></param>
        /// <param name="semanticModel"></param>
        /// <returns></returns>
        public static bool IsDisposedInDisposingMethod(this SyntaxNode nodeInClass, string nameOfVariable, IConfiguration configuration, SemanticModel semanticModel)
        {
            var classDeclarationSyntax = nodeInClass.FindContainingClass();
            if (classDeclarationSyntax == null) return false;

            var disposeMethods = classDeclarationSyntax
                .DescendantNodes<MethodDeclarationSyntax>()
                .Where(mds => IsDisposingMethod(mds, configuration, semanticModel))
                .ToArray();
            return disposeMethods
                .SelectMany(disposeMethod => disposeMethod.DescendantNodes<InvocationExpressionSyntax>())
                .Any(mes => mes.IsCallToDisposeFor(nameOfVariable));
        }

        private static bool IsDisposingMethod(MethodDeclarationSyntax mds, IConfiguration configuration, SemanticModel semanticModel)
        {
            var parametersymbols = mds.ParameterList.Parameters.Select(p => semanticModel.GetSymbolInfo(p.Type))
                .ToArray();
            IReadOnlyCollection<MethodCall> disposeMethods;
            if (configuration.DisposingMethods.TryGetValue(mds.Identifier.Text, out disposeMethods))
            {
                if (disposeMethods.Any(dm => dm.Parameter.Length == mds.ParameterList.Parameters.Count))
                    return true;
            }


            return mds.IsDisposeMethod() 
                || mds.AttributeLists
                    .SelectMany(als => als.Attributes)
                    .Select(a => semanticModel.GetTypeInfo(a).Type)
                    .Any(attribute => configuration.DisposingAttributes.Contains(attribute.GetFullNamespace()));
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

        internal static bool IsMaybePartOfMethodChainUsingTrackingExtensionMethod(
            this ObjectCreationExpressionSyntax objectCreation)
        {
            return objectCreation?.Parent is MemberAccessExpressionSyntax
                   && objectCreation?.Parent?.Parent is InvocationExpressionSyntax;
        }
        

        internal static bool IsPartOfMethodCall(this SyntaxNode node)
        {
            return node?.Parent is ArgumentSyntax
                   && node?.Parent.Parent is ArgumentListSyntax
                   && node?.Parent?.Parent?.Parent is InvocationExpressionSyntax;
        }

        internal static bool IsMaybePartOfMethodChainUsingTrackingExtensionMethod(
            this InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression?.Parent is MemberAccessExpressionSyntax
                   && invocationExpression?.Parent?.Parent is InvocationExpressionSyntax;
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
                var conditionalAccessExpression = parent as ConditionalAccessExpressionSyntax;
                if (conditionalAccessExpression != null)
                {
                    var invocationExpressions = conditionalAccessExpression.DescendantNodes<InvocationExpressionSyntax>().ToArray();
                    return invocationExpressions.Any(ies => ies.IsCallToDispose());
                }

                parent = parent.Parent;
            }
        }

        public static bool IsPartOfAssignmentExpression(this SyntaxNode node)
        {
            var parent = node.FindParent<AssignmentExpressionSyntax, ClassDeclarationSyntax>();
            return parent != null;
        }

        public static bool IsPartOfPropertyExpressionBody(this SyntaxNode node)
        {
            return node?.Parent is ArrowExpressionClauseSyntax
                   && node?.Parent?.Parent is PropertyDeclarationSyntax;
        }

        public static bool IsDecendentOfAProperty(this SyntaxNode node)
        {
            PropertyDeclarationSyntax parent;

            return TryFindContainingPropery(node, out parent);
        }

        public static bool IsPartOfAutoProperty(this SyntaxNode node) {
            return node?.Parent is EqualsValueClauseSyntax
                   && node.Parent?.Parent is PropertyDeclarationSyntax;
        }

        public static bool IsReturnedInProperty(this SyntaxNode node) {
            return node?.Parent is ReturnStatementSyntax
                   && node.Parent?.Parent?.Parent?.Parent?.Parent is PropertyDeclarationSyntax;
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

        public static bool TryFindContainingParenthesizedLambda(this SyntaxNode node, out ParenthesizedLambdaExpressionSyntax method) {
            method = node.FindParent<ParenthesizedLambdaExpressionSyntax, ClassDeclarationSyntax>();

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
            //todo refactor this. most of this branches are not in use
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
            ParenthesizedLambdaExpressionSyntax lambda;
            if (node.TryFindContainingParenthesizedLambda(out lambda))
            {
                parentScope = lambda;
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