using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
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
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            if (node?.Identifier.Text != Constants.Dispose) return;

            if (node.ExpressionBody != null) return;

            context.ReportDiagnostic(Diagnostic.Create(Unused.DisposableDescriptor, node.GetLocation()));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Unused.DisposableDescriptor);
    }
}