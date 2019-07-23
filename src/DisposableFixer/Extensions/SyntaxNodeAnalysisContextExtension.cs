using System.Collections.Immutable;
using DisposableFixer.Configuration;
using DisposableFixer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    internal static class SyntaxNodeAnalysisContextExtension
    {
        public const string IdForAnonymousObjectFromObjectCreation = "DF0000";
        public const string IdForAnonymousObjectFromMethodInvocation = "DF0001";
        public const string IdForNotDisposedLocalVariable = "DF0010";
        public const string IdForAssignmentFromObjectCreationToFieldNotDisposed = "DF0020";
        public const string IdForAssignmentFromMethodInvocationToFieldNotDisposed = "DF0021";
        public const string IdForAssignmentFromObjectCreationToPropertyNotDisposed = "DF0022";
        public const string IdForAssignmentFromMethodInvocationToPropertyNotDisposed = "DF0023";
        private const string Category = "Wrong Usage";

        public static void ReportNotDisposedField(this SyntaxNodeAnalysisContext context, string variableName,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(AssignmentFromMethodInvocationToFieldNotDisposedDescriptor, location,
                    properties.ToImmutable())
                : Diagnostic.Create(AssignmentFromObjectCreationToFieldNotDisposedDescriptor, location,
                    properties.ToImmutable()));
        }

        public static void ReportNotDisposedProperty(this SyntaxNodeAnalysisContext context, string variableName,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor, location,
                    properties.ToImmutable())
                : Diagnostic.Create(AssignmentFromObjectCreationToPropertyNotDisposedDescriptor, location,
                    properties.ToImmutable())
            );
        }

        public static void ReportNotDisposedLocalVariable(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(NotDisposedLocalVariableDescriptor, location));
        }

        public static void ReportNotDisposedAnonymousObject(this SyntaxNodeAnalysisContext context,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(AnonymousObjectFromMethodInvocationDescriptor, location)
                : Diagnostic.Create(AnonymousObjectFromObjectCreationDescriptor, location));
        }

        public static CustomAnalysisContext CreateCustomContext(this SyntaxNodeAnalysisContext context, DisposableSource source,
            INamedTypeSymbol type, IDetector detector)
        {
            return new CustomAnalysisContext(context, source, type, detector);
        }

        #region AnonymousObjectFromMethod

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationTitle =
            new LocalizableResourceString(
                nameof(Resources.AnonymousObjectFromMethodInvocationTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromMethodInvocationDescriptor =
            new DiagnosticDescriptor(
                IdForAnonymousObjectFromMethodInvocation,
                AnonymousObjectFromMethodInvocationTitle,
                AnonymousObjectFromMethodInvocationMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AnonymousObjectFromMethodInvocationDescription);

        #endregion

        #region AnonymousObjectFromObjectCreation

        private static readonly LocalizableString AnonymousObjectFromObjectCreationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationTitle =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromObjectCreationDescriptor =
            new DiagnosticDescriptor(
                IdForAnonymousObjectFromObjectCreation,
                AnonymousObjectFromObjectCreationTitle,
                AnonymousObjectFromObjectCreationMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AnonymousObjectFromObjectCreationDescription);

        #endregion

        #region NotDisposedLocalVariable

        private static readonly LocalizableString NotDisposedLocalVariableMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableTitle =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableDescription =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor NotDisposedLocalVariableDescriptor = new DiagnosticDescriptor(
            IdForNotDisposedLocalVariable,
            NotDisposedLocalVariableTitle,
            NotDisposedLocalVariableMessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, NotDisposedLocalVariableDescription);

        #endregion NotDisposedLocalVariable

        #region AssignmentFromObjectCreationToPropertyNotDisposedDescriptor

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToPropertyNotDisposedDescription =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromObjectCreationToPropertyNotDisposedDescriptor =
            new DiagnosticDescriptor(
                IdForAssignmentFromObjectCreationToPropertyNotDisposed,
                AssignmendFromObjectCreationToPropertyNotDisposedTitle,
                AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AssignmentFromObjectCreationToPropertyNotDisposedDescription);

        #endregion

        #region AssignmentFromObjectCreationToFieldNotDisposedDescriptor

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromObjectCreationToFieldNotDisposedDescriptor =
            new DiagnosticDescriptor(
                IdForAssignmentFromObjectCreationToFieldNotDisposed,
                AssignmentFromObjectCreationToFieldNotDisposedTitle,
                AssignmentFromObjectCreationToFieldNotDisposedMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AssignmentFromObjectCreationToFieldNotDisposedDescription);

        #endregion

        #region AssignmentFromMethodInvocationToFieldNotDisposedDescriptor

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromMethodInvocationToFieldNotDisposedDescriptor =
            new DiagnosticDescriptor(
                IdForAssignmentFromMethodInvocationToFieldNotDisposed,
                AssignmentFromMethodInvocationToFieldNotDisposedTitle,
                AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AssignmentFromMethodInvocationToFieldNotDisposedDescription);

        #endregion


        #region AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedDescription =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor =
            new DiagnosticDescriptor(
                IdForAssignmentFromMethodInvocationToPropertyNotDisposed,
                AssignmentFromMethodInvocationToPropertyNotDisposedTitle,
                AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat,
                Category,
                DiagnosticSeverity.Warning, true, AssignmentFromMethodInvocationToPropertyNotDisposedDescription);

        #endregion
    }
}