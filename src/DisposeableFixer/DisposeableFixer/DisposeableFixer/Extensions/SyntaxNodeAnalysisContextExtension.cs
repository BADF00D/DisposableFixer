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
        private static readonly LocalizableString NotDisposedLocalVariableMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString FieldMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedFieldMessageFormat), Resources.ResourceManager,
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

        public static readonly DiagnosticDescriptor NotDisposedLocalVariable = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId,
            Title,
            NotDisposedLocalVariableMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor NotDisposedDescriptor = new DiagnosticDescriptor(
            DisposableFixerAnalyzer.DiagnosticId,
            Title,
            NotDisposedMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor FieldNotDisposedDescriptor = new DiagnosticDescriptor(
           DisposableFixerAnalyzer.DiagnosticId,
           Title,
           FieldMessageFormat,
           DisposableFixerAnalyzer.Category,
           DiagnosticSeverity.Warning, true, Description);


        public static void ReportNotDisposedField(this SyntaxNodeAnalysisContext context, DisposableSource source) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(FieldNotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedAssignmentToFieldOrProperty(this SyntaxNodeAnalysisContext context, DisposableSource source) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedLocalDeclaration(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedLocalVariable, location));
        }

        public static void ReportNotDisposedAnonymousObject(this SyntaxNodeAnalysisContext context, DisposableSource source)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(source == DisposableSource.InvokationExpression
                ? Diagnostic.Create(AnonymousObjectFromMethodInvokationDescriptor, location)
                : Diagnostic.Create(AnonymousObjectFromObjectCreationDescriptor, location));
        }
    }
}