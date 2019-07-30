﻿using System.Collections.Immutable;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.Utils
{
    internal static class CustomAnalysisContextExtension
    {
        public static bool CouldDetectType(this CustomAnalysisContext ctx)
        {
            return ctx.Type != null;
        }

        public static bool IsDisposableOrImplementsDisposable(this CustomAnalysisContext ctx)
        {
            return ctx.Type.IsDisposableOrImplementsDisposable();
        }

        public static bool IsTypeIgnoredOrImplementsIgnoredInterface(this CustomAnalysisContext ctx)
        {
            return ctx.Detector.IsIgnoredTypeOrImplementsIgnoredInterface(ctx.Type);
        }

        public static void ReportNotDisposedAnonymousObject(this CustomAnalysisContext ctx)
        {
            var location = ctx.OriginalNode.GetLocation();
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? Descriptor.AnonymousObjectFromMethodInvocationDescriptor
                : Descriptor.AnonymousObjectFromObjectCreationDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }

        public static void ReportNotDisposedLocalVariable(this CustomAnalysisContext ctx)
        {
            var location = ctx.OriginalNode.GetLocation();
            var descriptor = Descriptor.NotDisposedLocalVariableDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }


        public static void ReportNotDisposedField(this CustomAnalysisContext ctx, string variableName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, variableName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? Descriptor.AssignmentFromMethodInvocationToFieldNotDisposedDescriptor
                : Descriptor.AssignmentFromObjectCreationToFieldNotDisposedDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable()));
        }

        public static void ReportNotDisposedProperty(this CustomAnalysisContext ctx, string propertyName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, propertyName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? Descriptor.AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor
                : Descriptor.AssignmentFromObjectCreationToPropertyNotDisposedDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable()));
        }

        public static CustomAnalysisContext NewWith(this CustomAnalysisContext ctx, SyntaxNode newNode)
        {
            return CustomAnalysisContext.WithOtherNode(ctx.Context, newNode, ctx.Source, ctx.Type, ctx.Detector,
                ctx.Configuration);
        }

        private static DiagnosticDescriptor ReplaceSeverity(DiagnosticDescriptor descriptor,
            DiagnosticSeverity severity)
        {
            return new DiagnosticDescriptor(descriptor.Id, descriptor.Title, descriptor.MessageFormat,
                descriptor.Category, severity, descriptor.IsEnabledByDefault, descriptor.Description);
        }

        private static bool GetCustomSeverity(CustomAnalysisContext context, out DiagnosticSeverity severity)
        {
            var fullName = context.Type.GetFullNamespace();
            return context.Configuration.TypeWithCustomSeverity.TryGetValue(fullName, out severity);
        }
    }
}