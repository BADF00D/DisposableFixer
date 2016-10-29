using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposeableFixer {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposeableFixerAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "DisposeableFixer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Wrong Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context) {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
            //context.RegisterSyntaxTreeAction(AnalyseTree);
            //context.RegisterCodeBlockAction(AnalyseCodeBock);
            //context.RegisterSyntaxTreeAction(AnalyseSyntaxTreeAction);
            context.RegisterSyntaxNodeAction(AnalyseSyntaxNode,SyntaxKind.LocalDeclarationStatement);
            
        }

        private static void AnalyseSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var tree = node.SyntaxTree;
            var test = new StringBuilder();
            var newNode = node.DescendantNodes().FirstOrDefault(n =>
            {
                var vardecl = n as VariableDeclaratorSyntax;
                return vardecl?.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any() 
                    ?? false;
            }) as VariableDeclaratorSyntax;
            if (newNode != null)
            {
                //find variablename
                var name = newNode.Identifier.Text;
                var location =
                    newNode.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault().GetLocation();



                //find all definitions for a variable wihtin the class and all its methods
                //var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes().OfType<VariableDeclarationSyntax>();
                var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes();
                var access = usages.OfType<MemberAccessExpressionSyntax>();


                //var dispose = access
                //    .Where(a => a.Expression is IdentifierNameSyntax)
                //    .Select(a => a.Expression as IdentifierNameSyntax)
                //    .Select(a => a.Identifier.Text)
                //    .ToList();

                var dispose = access
                    .Where(a =>
                    {
                        var id = a.Expression as IdentifierNameSyntax;
                        return id?.Identifier.Text == name && a.Name.Identifier.Text == "Dispose";
                    });

                if (!dispose.Any())
                {
                    var diagnostic = Diagnostic.Create(Rule, location);
                    context.ReportDiagnostic(diagnostic);
                }

            }
        }
        

        private static void AnalyseCodeBock(CodeBlockAnalysisContext context)
        {
            var block = context.CodeBlock;
            
            var fds = block as FieldDeclarationSyntax;
            var declaration = fds?.Declaration;
            if (declaration?.Type is IdentifierNameSyntax)
            {
                var ns = (IdentifierNameSyntax) declaration.Type;
                if (ns.Identifier.ValueText == "IDisposable")
                {
                    var diagnostic = Diagnostic.Create(Rule,block.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyseTree(SyntaxTreeAnalysisContext context)
        {
            var tree = context.Tree;

            var field = tree.GetRoot(context.CancellationToken)
                .ChildNodes()
                .Where(d =>
                {
                    var data = d is FieldDeclarationSyntax;
                    return data;
                })
                .ToArray();

            Debug.WriteLine("");
        }


        private static void AnalyzeSymbol(SymbolAnalysisContext context) {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var fieldsymbol = (IFieldSymbol)context.Symbol;
            
            // Find just those named type symbols with names containing lowercase letters.
            if (fieldsymbol.Name.ToCharArray().Any(char.IsLower)) {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, fieldsymbol.Locations[0], fieldsymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public class SomeSyntaxWalker : SyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            if (node is MethodDeclarationSyntax)
            {
                Debug.WriteLine("Do something");
            }
        }
    }
}
