using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    //todo define correct rules
    public static class SyntaxNodeAnalysisContextExtension
    {
        public const string IdForAnonymousObjectFromObjectCreation = "IdForAnonymousObjectFromObjectCreation";
        public const string IdForAnonymousMethodInvocation = "IdForAnonymousMethodInvocation";
        public const string IdForNotDisposedLocalVariable = "IdForNotDisposedLocalVariable";
        public const string IdForNotDisposed = "IdForNotDisposed";
        public const string IdForAssignmendFromObjectCreationToFieldNotDisposed = "IdForAssignmendFromObjectCreationToFieldNotDisposed";
        public const string IdForAssignmendFromMethodInvocationToFieldNotDisposed = "IdForAssignmendFromMethodInvocationToFieldNotDisposed";
        public const string IdForAssignmendFromObjectCreationToPropertyNotDisposed = "IdForAssignmendFromObjectCreationToPropertyNotDisposed";
        public const string IdForAssignmendFromMethodInvocationToPropertyNotDisposed = "IdForAssignmendFromMethodInvocationToPropertyNotDisposed";
        public const string IdForPropertyNotDisposed = "IdForPropertyNotDisposed";

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
            IdForAnonymousObjectFromObjectCreation,
            Title,
            AnonymousObjectFromObjectCreationMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AnonymousObjectFromMethodInvokationDescriptor = new DiagnosticDescriptor(
            IdForAnonymousMethodInvocation,
            Title,
            AnonymousObjectFromMethodInvokationMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor NotDisposedLocalVariableDescriptor = new DiagnosticDescriptor(
            IdForNotDisposedLocalVariable,
            Title,
            NotDisposedLocalVariableMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor NotDisposedDescriptor = new DiagnosticDescriptor(
            IdForNotDisposed,
            Title,
            NotDisposedMessageFormat,
            DisposableFixerAnalyzer.Category,
            DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AssignmendFromObjectCreationToFieldNotDisposedDescriptor = new DiagnosticDescriptor(
           IdForAssignmendFromObjectCreationToFieldNotDisposed,
           Title,
           FieldMessageFormat,
           DisposableFixerAnalyzer.Category,
           DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AssignmendFromMethodInvocationToFieldNotDisposedDescriptor = new DiagnosticDescriptor(
           IdForAssignmendFromMethodInvocationToFieldNotDisposed,
           Title,
           FieldMessageFormat,
           DisposableFixerAnalyzer.Category,
           DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AssignmendFromObjectCreationToPropertyNotDisposedDescriptor = new DiagnosticDescriptor(
          IdForAssignmendFromObjectCreationToPropertyNotDisposed,
          Title,
          FieldMessageFormat,
          DisposableFixerAnalyzer.Category,
          DiagnosticSeverity.Warning, true, Description);

        public static readonly DiagnosticDescriptor AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor = new DiagnosticDescriptor(
           IdForAssignmendFromMethodInvocationToPropertyNotDisposed,
           Title,
           FieldMessageFormat,
           DisposableFixerAnalyzer.Category,
           DiagnosticSeverity.Warning, true, Description);


        public static void ReportNotDisposedField(this SyntaxNodeAnalysisContext context, DisposableSource source)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(source == DisposableSource.InvokationExpression
                ? Diagnostic.Create(AssignmendFromMethodInvocationToFieldNotDisposedDescriptor, location)
                : Diagnostic.Create(AssignmendFromObjectCreationToFieldNotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedProperty(this SyntaxNodeAnalysisContext context, DisposableSource source) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(source == DisposableSource.InvokationExpression
                ? Diagnostic.Create(AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor, location)
                : Diagnostic.Create(AssignmendFromObjectCreationToPropertyNotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedAssignmentToFieldOrProperty(this SyntaxNodeAnalysisContext context, DisposableSource source) {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedDescriptor, location));
        }

        public static void ReportNotDisposedLocalDeclaration(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedLocalVariableDescriptor, location));
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