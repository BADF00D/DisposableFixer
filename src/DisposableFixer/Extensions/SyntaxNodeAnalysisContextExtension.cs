using System.Collections.Immutable;
using DisposableFixer.Configuration;
using DisposableFixer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Extensions
{
    internal static class SyntaxNodeAnalysisContextExtension
    {
        public static void ReportNotDisposedField(this SyntaxNodeAnalysisContext context, string variableName,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(Descriptor.AssignmentFromMethodInvocationToFieldNotDisposedDescriptor, location,
                    properties.ToImmutable())
                : Diagnostic.Create(Descriptor.AssignmentFromObjectCreationToFieldNotDisposedDescriptor, location,
                    properties.ToImmutable()));
        }

        public static void ReportNotDisposedProperty(this SyntaxNodeAnalysisContext context, string variableName,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(Descriptor.AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor, location,
                    properties.ToImmutable())
                : Diagnostic.Create(Descriptor.AssignmentFromObjectCreationToPropertyNotDisposedDescriptor, location,
                    properties.ToImmutable())
            );
        }

        public static void ReportNotDisposedLocalVariable(this SyntaxNodeAnalysisContext context)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(Descriptor.NotDisposedLocalVariableDescriptor, location));
        }

        public static void ReportNotDisposedAnonymousObject(this SyntaxNodeAnalysisContext context,
            DisposableSource source)
        {
            var location = context.Node.GetLocation();

            context.ReportDiagnostic(source == DisposableSource.InvocationExpression
                ? Diagnostic.Create(Descriptor.AnonymousObjectFromMethodInvocationDescriptor, location)
                : Diagnostic.Create(Descriptor.AnonymousObjectFromObjectCreationDescriptor, location));
        }

        public static CustomAnalysisContext CreateCustomContext(this SyntaxNodeAnalysisContext context, DisposableSource source,
            INamedTypeSymbol type, IDetector detector)
        {
            return CustomAnalysisContext.WithOriginalNode(context, source, type, detector);
        }

        #region AnonymousObjectFromMethod

        #endregion

        #region AnonymousObjectFromObjectCreation

        #endregion

        #region NotDisposedLocalVariable

        #endregion NotDisposedLocalVariable

        #region AssignmentFromObjectCreationToPropertyNotDisposedDescriptor

        #endregion

        #region AssignmentFromObjectCreationToFieldNotDisposedDescriptor

        #endregion

        #region AssignmentFromMethodInvocationToFieldNotDisposedDescriptor

        #endregion


        #region AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor

        #endregion
    }
}