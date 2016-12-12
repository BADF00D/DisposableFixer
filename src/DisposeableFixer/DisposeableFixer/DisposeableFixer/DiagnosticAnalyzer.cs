using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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

        private const string Category = "Wrong Usage";

        private const string DisposeMethod = "Dispose";
        private const string DisposableInterface = "IDisposable";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof (Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof (Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof (Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                var type = symbol?.ReceiverType as INamedTypeSymbol;

                if (type != null && !IsDisposeableOrImplementsDisposable(type)) return;

                //check if instance is Disposed via Dispose() or by include it in using
                if (node.IsNodeWithinUsing()) return; //using(new MemoryStream()){}
                if (node.IsPartOfReturn()) return; //return new MemoryStream(),
                if (node.IsPartOfVariableDeclarator())
                {
                    var identifier = (node.Parent.Parent as VariableDeclaratorSyntax)?.Identifier;
                    if (identifier == null) return;
                    if (node.IsLocalDeclaration())
                    {
                        SyntaxNode ctorOrMethod;
                        if (!node.TryFindContainingConstructorOrMethod(out ctorOrMethod)) return;

                        if (ctorOrMethod.DescendantNodes<UsingStatementSyntax>()
                            .SelectMany(@using =>
                            {
                                var objectCreationExpressionSyntaxs = @using
                                    .DescendantNodes<IdentifierNameSyntax>()
                                    .ToArray();
                                return objectCreationExpressionSyntaxs;
                            })
                            .Any(id => id.Identifier.Value == identifier.Value.Value))
                        {
                            return;
                        }
                        if (ctorOrMethod.DescendantNodes<InvocationExpressionSyntax>().Any(ies =>
                        {
                            var expression = (ies.Expression as MemberAccessExpressionSyntax);
                            var ids = expression.Expression as IdentifierNameSyntax;
                            return ids.Identifier.Text == identifier.Value.Text
                                   && expression.Name.Identifier.Text == DisposeMethod;
                        }))
                        {
                            return;
                        }

                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                        return;
                    }

                    if (node.IsFieldDeclaration())
                    {
                        var disposeMethod = node.FindContainingClass().DescendantNodes<MethodDeclarationSyntax>()
                            .FirstOrDefault(method => method.Identifier.Text == DisposeMethod);
                        if (disposeMethod == null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
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
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                        return;

                    }
                }


                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static void AnalyseInvokationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            try
            {
                var node = context.Node;

                var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                var type = symbol?.ReturnType as INamedTypeSymbol;

                if (type == null || !IsDisposeableOrImplementsDisposable(type)) return;
                if (node.IsNodeWithinUsing()) return; //using(new MemoryStream()){}
                if (node.IsPartOfReturn()) return; //return new MemoryStream(),

                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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