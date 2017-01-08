using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    //todo define correct rules
    public static class SyntaxNodeAnalysisContextExtension
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof (Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat), Resources.ResourceManager,
                typeof (Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvokationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvokationMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof (Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromObjectCreationDescriptor = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId,
            Title,
            AnonymousObjectFromObjectCreationMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AnonymousObjectFromMethodInvokationDescriptor = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId,
            Title,
            AnonymousObjectFromMethodInvokationMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor NotDisposedDescriptor = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId,
            Title,
            AnonymousObjectFromMethodInvokationMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);


        public static void ReportNotDisposed(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedFieldFromObjectCreation(this SyntaxNodeAnalysisContext context) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedLocalDeclaration(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedLocalObjectFromObjectCreation(this SyntaxNodeAnalysisContext context) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedInvokationExpression(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedObjectCreation(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedAnonymousObjectFromObjectCreation(this SyntaxNodeAnalysisContext context) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }
    }
}