using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposableFixerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DisposableFixer";
        public const string Category = "Wrong Usage";

        private const string DisposeMethod = "Dispose";
        private const string DisposableInterface = "IDisposable";

	    private static IDetector _detector = new Detector();

	    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(SyntaxNodeAnalysisContextExtension.AnonymousObjectFromObjectCreationDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyseInvokationExpressionStatement, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyseObjectCreationExpressionStatement,
                SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyseObjectCreationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            try
            {
				
                var node = context.Node as ObjectCreationExpressionSyntax;
                if (node == null) return; //something went wrong

                var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
                var type = (symbolInfo.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;
                if (type == null) { }
                else if (IsIgnoredTypeOrImplementsIgnoredInterface(type)) {}
                else if (node.IsPartOfReturn()) {}
                else if (!IsDisposeableOrImplementsDisposable(type)) { }
                else if (node.IsArgumentInObjectCreation()) AnalyseNodeInArgumentList(context, node, DisposableSource.ObjectCreation);
                else if (node.IsDescendantOfUsingDeclaration()) { }//this have to be checked after IsArgumentInObjectCreation
                else if (node.IsDescendantOfVariableDeclarator()) AnalyseNodeWithinVariableDeclarator(context, node, DisposableSource.ObjectCreation);
                else if (node.IsPartOfAssignmentExpression()) AnalyseNodeInAssignmentExpression(context, node, DisposableSource.ObjectCreation);
                else context.ReportNotDisposedAnonymousObject(DisposableSource.ObjectCreation); //new MemoryStream();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static void AnalyseNodeWithinVariableDeclarator(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            var identifier = (node.Parent.Parent as VariableDeclaratorSyntax)?.Identifier;
            if (identifier == null) return;
            if (node.IsLocalDeclaration()) //var m = new MemoryStream();
            {
                AnalyseNodeWithinLocalDeclaration(context, node, identifier, source);
            }
            else if (node.IsFieldDeclaration()) //_field = new MemoryStream();
            {
                AnalyseNodeInFieldDeclaration(context, node, identifier, source);
            }
        }

        private static void AnalyseNodeWithinLocalDeclaration(SyntaxNodeAnalysisContext context,
            SyntaxNode node, SyntaxToken? identifier, DisposableSource source)
        {
            SyntaxNode ctorOrMethod;
            if (!node.TryFindContainingConstructorOrMethod(out ctorOrMethod)) return;

            var usings = ctorOrMethod.DescendantNodes<UsingStatementSyntax>()
                .SelectMany(@using => @using.DescendantNodes<IdentifierNameSyntax>())
                .Where(id => id.Identifier.Value == identifier.Value.Value)
                .ToArray();

            if (usings.Any())
            {
                if (usings.Any(id => id.Parent is UsingStatementSyntax)) //using(mem))
                {
                    return;
                }
                var isTracked = usings
                    .Select(id => id.Parent?.Parent?.Parent)
                    .Where(parent => parent is ObjectCreationExpressionSyntax)
                    .Any(ocs =>
                    {
                        var sym = context.SemanticModel.GetSymbolInfo(ocs);
                        var type2 = (sym.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;

                        return _detector.IsTrackedType(type2, ocs as ObjectCreationExpressionSyntax, context.SemanticModel);
                    });
                if (isTracked) return;

                context.ReportNotDisposedLocalDeclaration();
                return;
            }
            if (ctorOrMethod.DescendantNodes<InvocationExpressionSyntax>().Any(ies =>
            {
                var expression = ies.Expression as MemberAccessExpressionSyntax;
                var ids = expression.Expression as IdentifierNameSyntax;
                return ids.Identifier.Text == identifier.Value.Text
                       && expression.Name.Identifier.Text == DisposeMethod;
            }))
            {
                return;
            }

            context.ReportNotDisposedLocalDeclaration();
        }

        private static void AnalyseNodeInFieldDeclaration(SyntaxNodeAnalysisContext context,
            SyntaxNode node, SyntaxToken? identifier, DisposableSource source)
        {
            var disposeMethod = node.FindContainingClass().DescendantNodes<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.Text == DisposeMethod);
            if (disposeMethod == null)
            {
                //there is no dispose method in this class
                context.ReportNotDisposedField(source);
                return;
            }
            ;
            var isDisposed = disposeMethod.DescendantNodes<InvocationExpressionSyntax>()
                .Select(invo => invo.Expression as MemberAccessExpressionSyntax)
                .Any(invo =>
                {
                    var id = invo.Expression as IdentifierNameSyntax;
                    var member = id.Identifier.Text == identifier.Value.Text;
                    var callToDispose = invo.Name.Identifier.Text == DisposeMethod;

                    return member && callToDispose;
                });
            if (isDisposed) return;
            //there is a dispose method in this class, but ObjectCreation is not disposed
            context.ReportNotDisposedField(source);
        }

        private static void AnalyseNodeInAssignmentExpression(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            var disposeMethod = node.FindContainingClass().DescendantNodes<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.Text == DisposeMethod);
            if (disposeMethod == null)
            {
                context.ReportNotDisposedAssignmentToFieldOrProperty(source); //this can also be a property
            }
        }
        
        private static void AnalyseNodeInArgumentList(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            var objectCreation = node.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
            var t = context.SemanticModel.GetReturnTypeOf(objectCreation);
            if (_detector.IsTrackedType(t, objectCreation, context.SemanticModel)) return;

            context.ReportNotDisposedAnonymousObject(source);
        }

        private static void AnalyseInvokationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            try
            {
                var node = context.Node;

                var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                var type = symbol?.ReturnType as INamedTypeSymbol;

                if (type == null) {}
                else if (IsIgnoredTypeOrImplementsIgnoredInterface(type)) {}
                else if (!IsDisposeableOrImplementsDisposable(type)) {}
                else if (node.IsPartOfReturn()) {}
                else if (node.IsArgumentInObjectCreation()) AnalyseNodeInArgumentList(context, node, DisposableSource.InvokationExpression);
                else if (node.IsDescendantOfUsingDeclaration()) {}
                else if (node.IsDescendantOfVariableDeclarator()) AnalyseNodeWithinVariableDeclarator(context, node, DisposableSource.InvokationExpression);
                else if (node.IsPartOfAssignmentExpression()) AnalyseNodeInAssignmentExpression(context, node, DisposableSource.InvokationExpression);
                //else context.ReportNotDisposed(DisposableSource.InvokationExpression);//todo
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static bool IsIgnoredTypeOrImplementsIgnoredInterface(INamedTypeSymbol type)
        {
            if (_detector.IsIgnoredType(type)) return true;

            var inter = type.AllInterfaces.Select(ai => ai);
            return inter.Any(@if => _detector.IsIgnoredInterface(@if));
        }

        private static bool IsDisposeableOrImplementsDisposable(INamedTypeSymbol typeInfo)
        {
            return IsIDisposable(typeInfo) || ImplementsIDisposable(typeInfo);
        }

        private static bool IsIDisposable(INamedTypeSymbol typeInfo)
        {
            return typeInfo.Name == DisposableInterface;
        }

        private static bool ImplementsIDisposable(INamedTypeSymbol typeInfo)
        {
            return typeInfo.AllInterfaces.Any(i => i.Name == DisposableInterface);
        }
    }
}