using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    //todo define correct rules
    public static class SyntaxNodeAnalysisContextExtension
    {
        public const string IdForAnonymousObjectFromObjectCreation = "DF0000";
        public const string IdForAnonymousObjectFromMethodInvocation = "DF0001";
        public const string IdForNotDisposedLocalVariable = "DF0010";
        public const string IdForAssignmendFromObjectCreationToFieldNotDisposed = "DF0020";
        public const string IdForAssignmendFromMethodInvocationToFieldNotDisposed = "DF0021";
        public const string IdForAssignmendFromObjectCreationToPropertyNotDisposed = "DF0022";
        public const string IdForAssignmendFromMethodInvocationToPropertyNotDisposed = "DF0023";
        private const string Category = "Wrong Usage";

        #region AnonymousObjectFromMethod
        private static readonly LocalizableString AnonymousObjectFromMethodInvocationTitle = new LocalizableResourceString(
            nameof(Resources.AnonymousObjectFromMethodInvocationTitle), Resources.ResourceManager, typeof (Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromMethodInvocationDescriptor = new DiagnosticDescriptor(
            IdForAnonymousObjectFromMethodInvocation,
            AnonymousObjectFromMethodInvocationTitle,
            AnonymousObjectFromMethodInvocationMessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, AnonymousObjectFromMethodInvocationDescription);
        #endregion
        
        #region AnonymousObjectFromObjectCreation
        private static readonly LocalizableString AnonymousObjectFromObjectCreationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationTitle =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromObjectCreationDescriptor = new DiagnosticDescriptor(
            IdForAnonymousObjectFromObjectCreation,
            AnonymousObjectFromObjectCreationTitle,
            AnonymousObjectFromObjectCreationMessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, AnonymousObjectFromObjectCreationDescription);
        #endregion

        #region NotDisposedLocalVariable
        private static readonly LocalizableString NotDisposedLocalVariableMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableTitle =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableDescription =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor NotDisposedLocalVariableDescriptor = new DiagnosticDescriptor(
            IdForNotDisposedLocalVariable,
            NotDisposedLocalVariableTitle,
            NotDisposedLocalVariableMessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, NotDisposedLocalVariableDescription);

        #endregion NotDisposedLocalVariable

        #region AssignmendFromObjectCreationToPropertyNotDisposedDescriptor
        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmendFromObjectCreationToPropertyNotDisposedDescriptor = new DiagnosticDescriptor(
          IdForAssignmendFromObjectCreationToPropertyNotDisposed,
           AssignmendFromObjectCreationToPropertyNotDisposedTitle,
           AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat,
           Category,
           DiagnosticSeverity.Warning, true, AssignmendFromObjectCreationToPropertyNotDisposedDescription);

        #endregion

        #region AssignmendFromObjectCreationToFieldNotDisposedDescriptor
        private static readonly LocalizableString AssignmendFromObjectCreationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmendFromObjectCreationToFieldNotDisposedDescriptor = new DiagnosticDescriptor(
          IdForAssignmendFromObjectCreationToFieldNotDisposed,
           AssignmendFromObjectCreationToFieldNotDisposedTitle,
           AssignmendFromObjectCreationToFieldNotDisposedMessageFormat,
           Category,
           DiagnosticSeverity.Warning, true, AssignmendFromObjectCreationToFieldNotDisposedDescription);

        #endregion

        #region AssignmendFromMethodInvocationToFieldNotDisposedDescriptor
        private static readonly LocalizableString AssignmendFromMethodInvocationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromMethodInvocationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmendFromMethodInvocationToFieldNotDisposedDescriptor = new DiagnosticDescriptor(
          IdForAssignmendFromMethodInvocationToFieldNotDisposed,
           AssignmendFromMethodInvocationToFieldNotDisposedTitle,
           AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat,
           Category,
           DiagnosticSeverity.Warning, true, AssignmendFromMethodInvocationToFieldNotDisposedDescription);

        #endregion


        #region AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor
        private static readonly LocalizableString AssignmendFromMethodInvocationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromMethodInvocationToPropertyNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedDescription), Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor = new DiagnosticDescriptor(
          IdForAssignmendFromMethodInvocationToPropertyNotDisposed,
           AssignmendFromMethodInvocationToPropertyNotDisposedTitle,
           AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat,
           Category,
           DiagnosticSeverity.Warning, true, AssignmendFromMethodInvocationToPropertyNotDisposedDescription);

        #endregion

        public static void ReportNotDisposedField(this SyntaxNodeAnalysisContext context, string variableName, DisposableSource source)
        {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);

            context.ReportDiagnostic(source == DisposableSource.InvokationExpression
                ? Diagnostic.Create(AssignmendFromMethodInvocationToFieldNotDisposedDescriptor, location, properties.ToImmutable())
                : Diagnostic.Create(AssignmendFromObjectCreationToFieldNotDisposedDescriptor, location, properties.ToImmutable()));
        }

        public static void ReportNotDisposedProperty(this SyntaxNodeAnalysisContext context, string variableName,  DisposableSource source) {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string,string>();
            properties.Add(Constants.Variablename, variableName);
                
            context.ReportDiagnostic(source == DisposableSource.InvokationExpression
                ? Diagnostic.Create(AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor, location, properties.ToImmutable())
                : Diagnostic.Create(AssignmendFromObjectCreationToPropertyNotDisposedDescriptor, location, properties.ToImmutable())
                );
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
                ? Diagnostic.Create(AnonymousObjectFromMethodInvocationDescriptor, location)
                : Diagnostic.Create(AnonymousObjectFromObjectCreationDescriptor, location));
        }
    }
}