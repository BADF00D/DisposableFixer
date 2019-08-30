using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Configuration;
using DisposableFixer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static ClassDeclarationSyntax FindContainingClass(this SyntaxNode node)
        {
            while (true)
            {
                if (node.Parent == null) return null;
                if (node.Parent is ClassDeclarationSyntax @class)
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
        /// <example>new List&lt;IDisposable&gt;(new MemoryStream())</example>
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
        /// <example>new List&lt;IDisposable&gt;(new []{new MemoryStream()})</example>
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
        ///     of using(flag ? new MemoryStream() : new MemoryStream())
        ///     of using(var x = flag ? new MemoryStream() : new MemoryStream())
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsDescendantOfUsingHeader<T>(this T node) where T : SyntaxNode
        {
            return node?.Parent is UsingStatementSyntax //using(memStream) or using(new MemoryStream())
                   ||
                   (node?.Parent?.Parent?.Parent is VariableDeclarationSyntax &&
                    node.Parent?.Parent?.Parent?.Parent is UsingStatementSyntax)
                   || (node?.Parent is ConditionalExpressionSyntax &&
                       node?.Parent?.Parent is UsingStatementSyntax
                   ) //using(flag ? new MemotyStream() : new MemoryStream())
                   || (node?.Parent is ConditionalExpressionSyntax &&
                       node.Parent?.Parent?.Parent?.Parent is VariableDeclarationSyntax &&
                       node.Parent?.Parent?.Parent?.Parent?.Parent is UsingStatementSyntax
                   ) //using(var x = flag ? new MemotyStream() : new MemoryStream())
                ;
        }

        public static bool IsPartOfVariableDeclaratorInsideAUsingDeclaration<T>(this T node) where T : SyntaxNode
        {
            return node?.Parent is EqualsValueClauseSyntax
                   && node.Parent?.Parent is VariableDeclaratorSyntax
                   && node.Parent?.Parent?.Parent is VariableDeclarationSyntax
                   && node.Parent?.Parent?.Parent?.Parent is UsingStatementSyntax;
        }

        public static bool IsAssignmentToProperty(this SyntaxNode node, string variableName, out bool isStatic)
        {
            var @class = node.FindContainingClass();
            isStatic = false;

            var propertyDeclaration =  @class?.DescendantNodes<PropertyDeclarationSyntax>()
                .FirstOrDefault(pd => pd.Identifier.Text == variableName);
            if (propertyDeclaration == null)
            {
                return false;
            }

            isStatic = propertyDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
            return true;
        }

        public static bool IsAssignmentToStaticField(this SyntaxNode node, string fieldName)
        {
            var @class = node.FindContainingClass();

            return @class?
                       .DescendantNodes<FieldDeclarationSyntax>()
                       .Any(fds => fds.DescendantNodes<VariableDeclaratorSyntax>().Any(vds =>
                           vds.Identifier.Text == fieldName && fds.Modifiers.Any(SyntaxKind.StaticKeyword)))
                   ?? false;
        }

        public static bool IsDescendantOfVariableDeclarator(this SyntaxNode node)
        {
            return node.Parent?.Parent is VariableDeclaratorSyntax;
        }
        public static bool IsDescendantOfAwaitingVariableDeclarator(this SyntaxNode node)
        {
            return node.Parent is AwaitExpressionSyntax && 
                node.Parent?.Parent?.Parent is VariableDeclaratorSyntax;
        }
        public static bool IsDescendantOfAssignmentExpressionSyntax(this SyntaxNode node)
        {
            return node.Parent is AssignmentExpressionSyntax;
        }

        public static bool IsPartOfReturnStatementInMethod(this SyntaxNode node)
        {
            return node.FindParent<ReturnStatementSyntax, MethodDeclarationSyntax>() != null;
        }
        public static bool IsPartOfReturnStatementInBlock(this SyntaxNode node)
        {
            return node.FindParent<ReturnStatementSyntax, BlockSyntax>() != null;
        }

        public static bool IsPartOfYieldReturnStatementInBlock(this SyntaxNode node)
        {
            return node.FindParent<YieldStatementSyntax, BlockSyntax>() != null;
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

        public static bool IsArrowExpressionClauseOfMethod(this SyntaxNode node)
        {
            return node?.Parent is ArrowExpressionClauseSyntax
                   && node.Parent?.Parent is MethodDeclarationSyntax;
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

            return classDeclarationSyntax
                .DescendantNodes<MethodDeclarationSyntax>()
                .Where(mds => mds.IsDisposeMethod(configuration, semanticModel))
                .SelectMany(disposeMethod => disposeMethod.DescendantNodes<InvocationExpressionSyntax>())
                .Any(ies => ies.IsCallToDisposeFor(nameOfVariable, semanticModel, configuration));
        }

        public static bool ContainsDisposeCallFor(this SyntaxNode node, string name, SemanticModel semanticModel, IConfiguration configuration)
        {
            return node
                .DescendantNodes<InvocationExpressionSyntax>()
                .Any(ies => ies.IsCallToDisposeFor(name, semanticModel, configuration));
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
                   && objectCreation.Parent?.Parent is InvocationExpressionSyntax;
        }

        internal static bool IsTrackedViaTrackingMethod(this SyntaxNode node, CustomAnalysisContext context, BaseMethodDeclarationSyntax scope,
            string variableName)
        {
            return scope
                .DescendantNodes<InvocationExpressionSyntax>()
                .Any(ies => ies.IsInvocationExpressionSyntaxOn(variableName) && context.Detector.IsTrackingMethodCall(ies, context.SemanticModel));
        }


        

        internal static bool IsPartOfMethodCall(this SyntaxNode node)
        {
            return node?.Parent is ArgumentSyntax
                   && node.Parent.Parent is ArgumentListSyntax
                   && node.Parent?.Parent?.Parent is InvocationExpressionSyntax;
        }

        public static bool IsParentADisposeCallIgnoringParenthesis(this SyntaxNode node)
        {
            var parent = node.Parent;
            while (true)
            {
                if (parent == null
                    || parent is MethodDeclarationSyntax
                    || parent is PropertyDeclarationSyntax
                    || parent is ConstructorDeclarationSyntax
                    || parent is ArgumentSyntax)
                    return false;
                if (parent is ParenthesizedExpressionSyntax)
                {
                    parent = parent.Parent;
                    continue;
                }

                if (parent is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                    return memberAccessExpressionSyntax.IsDisposeCall();
                if (parent is ConditionalAccessExpressionSyntax conditionalAccessExpression)
                {
                    return conditionalAccessExpression
                        .DescendantNodes<InvocationExpressionSyntax>()
                        .Any(ies => ies.IsCallToDispose());
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
                   && node.Parent?.Parent is PropertyDeclarationSyntax;
        }

        public static bool IsPartOfAutoProperty(this SyntaxNode node) {
            return node?.Parent is EqualsValueClauseSyntax
                   && node.Parent?.Parent is PropertyDeclarationSyntax;
        }

        public static bool IsReturnedInProperty(this SyntaxNode node) {
            return node?.Parent is ReturnStatementSyntax
                   && node.Parent?.Parent?.Parent?.Parent?.Parent is PropertyDeclarationSyntax;
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

        public static bool TryFindContainingProperty(this SyntaxNode node, out PropertyDeclarationSyntax method)
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
        public static bool TryFindParentClass(this SyntaxNode node, out ClassDeclarationSyntax ctor)
        {
            ctor = node.FindParent<ClassDeclarationSyntax, CompilationUnitSyntax>();

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

        public static bool TryFindParent<T>(this SyntaxNode start, out T scope) where T : SyntaxNode
        {
            return TryFindParent(start, default(SyntaxNode), out scope);
        }

        public static bool TryFindParent<T>(this SyntaxNode start, SyntaxNode @break, out T scope)
            where T : SyntaxNode
        {
            var tmp = start;
            while (true)
            {
                if (start is T result)
                {
                    scope = result;
                    return true;
                }
                if (tmp == @break)
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
            if (node.TryFindContainingCtor(out var ctor))
            {
                if (node.TryFindParent<SimpleLambdaExpressionSyntax>(ctor, out var sles))
                {
                    parentScope = sles;
                    return true;
                }
                if (node.TryFindParent<ParenthesizedLambdaExpressionSyntax>(ctor, out var ples))
                {
                    parentScope = ples;
                    return true;
                }
                parentScope = ctor;
                return true;
            }

            if (node.TryFindContainingMethod(out var method))
            {
                if (node.TryFindParent<SimpleLambdaExpressionSyntax>(method, out var sles))
                {
                    parentScope = sles;
                    return true;
                }
                if (node.TryFindParent<ParenthesizedLambdaExpressionSyntax>(method, out var ples))
                {
                    parentScope = ples;
                    return true;
                }
                parentScope = method;
                return true;
            }

            if (node.TryFindContainingProperty(out var property))
            {
                if (node.TryFindParent<SimpleLambdaExpressionSyntax>(property, out var sles))
                {
                    parentScope = sles;
                    return true;
                }
                if (node.TryFindParent<ParenthesizedLambdaExpressionSyntax>(property, out var ples))
                {
                    parentScope = ples;
                    return true;
                }
                parentScope = property;
                return true;
            }

            if (node.TryFindContainingParenthesizedLambda(out var lambda))
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

        public static bool TryFindContainingBlock(this SyntaxNode node, out BlockSyntax block)
        {
            block = default(BlockSyntax);
            var result = node;
            while (result != null && !(result is BlockSyntax))
            {
                result = result.Parent;
            }

            block = result as BlockSyntax;
            return block != null;

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
    }
}