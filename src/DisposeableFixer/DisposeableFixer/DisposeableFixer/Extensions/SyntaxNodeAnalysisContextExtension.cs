using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    //todo define correct rules
    public static class SyntaxNodeAnalysisContextExtension
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor NotDisposed = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId, 
            Title, 
            MessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);


        public static void ReportNotDisposed(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposed, location));
        }
        public static void ReportNotDisposedLocalDeclaration(this SyntaxNodeAnalysisContext context) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposed, location));
        }

        public static void ReportNotDisposedInvokationExpression(this SyntaxNodeAnalysisContext context) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposed, location));
        }
        

    }
}