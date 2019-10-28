using System.Collections.Immutable;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedDisposableAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            if (node?.Identifier.Text != Constants.Dispose) return;

            if (node.ExpressionBody != null || node.Body == null) return;

            var statements = node.Body.DescendantNodes<StatementSyntax>();
            if (statements.Any()) return;

            if (node.TryFindParentClass(out var @class))
            {
                var has = @class.BaseList?.Types.Any(bts => bts.Type is NameSyntax ns && ns.IsIDisposable()) ?? false;
                if (!has) return;

                context.ReportDiagnostic(Diagnostic.Create(Unused.DisposableDescriptor, node.GetLocation()));
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Unused.DisposableDescriptor);
    }
}