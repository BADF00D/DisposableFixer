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

namespace DisposeableFixer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposeableFixerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DisposeableFixer";

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

        private const string Category = "Wrong Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyseLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyseLocalDeclarationStatement, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(SimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
        }
        
        private static void AnalyseLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var symanticModel = context.SemanticModel;
            var creation = node
                .DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault(n => n?.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any() ?? false);

            if (creation == null) return; //nothing to analyse
            var identifierNameSyntax = node.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            var typeInfo = symanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (typeInfo == null) return;
            if (!typeInfo.AllInterfaces.Any(i => i.Name == "IDisposable")) return;

            var name = creation.Identifier.Text;
            var location =
                creation.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault().GetLocation();

            //find all definitions for a variable wihtin the class and all its methods
            //var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes().OfType<VariableDeclarationSyntax>();
            var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes();
            var access = usages.OfType<MemberAccessExpressionSyntax>();

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

        private static void SimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var symanticModel = context.SemanticModel;
            
            var creationSyntax = context.Node
                .DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .FirstOrDefault();

            if (creationSyntax == null) return; //nothing to analyse
            var identifierNameSyntax = context.Node.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            /*at this point we cant get the type of the variable, because its assignment to a field.
             * We have to find the field and get the name from that FieldDeclaration */
            var classDeclaration = FindContainingClass(context.Node);
            if (classDeclaration == null) return;
            var fieldDeclaration = FindFieldInClass(classDeclaration, identifierNameSyntax.Identifier.Text);
            if (fieldDeclaration == null) return;
            var identifierSyntax = fieldDeclaration.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .FirstOrDefault();

            var typeInfo = symanticModel.GetSymbolInfo(identifierSyntax).Symbol as INamedTypeSymbol;
            if (typeInfo == null) return;
            if (!typeInfo.AllInterfaces.Any(i => i.Name == "IDisposable")) return;

            var name = identifierSyntax.Identifier.Text;
            var location = creationSyntax.GetLocation();

            //find all definitions for a variable wihtin the class and all its methods
            //var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes().OfType<VariableDeclarationSyntax>();
            var usages = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes();
            var access = usages.OfType<MemberAccessExpressionSyntax>();

            var dispose = access
                .Where(a => {
                    var id = a.Expression as IdentifierNameSyntax;
                    return id?.Identifier.Text == name && a.Name.Identifier.Text == "Dispose";
                });

            if (!dispose.Any()) {
                var diagnostic = Diagnostic.Create(Rule, location);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static FieldDeclarationSyntax FindFieldInClass(ClassDeclarationSyntax classNode, string field_name)
        {
            return classNode
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(fds =>
                {
                    return fds
                            .DescendantNodes()
                            .OfType<VariableDeclarationSyntax>()
                            .Count(id => id.Variables.Any(v => v.Identifier.Text== field_name)) == 1;
                })
                .FirstOrDefault();
        }

        private static ClassDeclarationSyntax FindContainingClass(SyntaxNode node)
        {
            while (true)
            {
                var @class = node.Parent as ClassDeclarationSyntax;
                if (@class != null)
                    return @class;

                node = node.Parent;
            }
        }
    }
}
