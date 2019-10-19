using System.Collections.Immutable;
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
                ? NotDisposed.AnonymousObject.FromMethodInvocationDescriptor
                : NotDisposed.AnonymousObject.FromObjectCreationDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }

        public static void ReportNotDisposedLocalVariable(this CustomAnalysisContext ctx, string variableName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var descriptor = NotDisposed.LocalVariable.Descriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, variableName));
        }


        public static void ReportNotDisposedField(this CustomAnalysisContext ctx, string fieldName, bool isStatic)
        {
            if (isStatic)
            {
                ReportNotDisposedStaticField(ctx, fieldName);
            }
            else
            {
                ReportNotDisposedNoneStaticField(ctx, fieldName);
            }
        }

        private static void ReportNotDisposedNoneStaticField(CustomAnalysisContext ctx, string fieldName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, fieldName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? NotDisposed.Assignment.FromMethodInvocation.ToField.OfSameTypeDescriptor
                : NotDisposed.Assignment.FromObjectCreation.ToField.OfSameTypeDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable(), fieldName));
        }

        private static void ReportNotDisposedStaticField(this CustomAnalysisContext ctx, string fieldName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, fieldName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? NotDisposed.Assignment.FromMethodInvocation.ToStaticField.OfSameTypeDescriptor
                : NotDisposed.Assignment.FromObjectCreation.ToStaticField.OfSameTypeDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable(), fieldName));
        }

        public static void ReportNotDisposedProperty(this CustomAnalysisContext ctx, string propertyName, bool isStatic)
        {
            if (isStatic)
            {
                ReportNotDisposedStaticProperty(ctx, propertyName);
            }
            else
            {
                ReportNotDisposedNoneStaticProperty(ctx, propertyName);
            }
        }

        private static void ReportNotDisposedNoneStaticProperty(CustomAnalysisContext ctx, string propertyName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, propertyName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? NotDisposed.Assignment.FromMethodInvocation.ToProperty.OfSameTypeDescriptor
                : NotDisposed.Assignment.FromObjectCreation.ToProperty.OfSameTypeDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable(), propertyName));
        }

        private static void ReportNotDisposedStaticProperty(this CustomAnalysisContext ctx, string propertyName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, propertyName);
            var descriptor = ctx.Source == DisposableSource.InvocationExpression
                ? NotDisposed.Assignment.FromMethodInvocation.ToStaticProperty.OfSameTypeDescriptor
                : NotDisposed.Assignment.FromObjectCreation.ToStaticProperty.OfSameTypeDescriptor;
            if (GetCustomSeverity(ctx, out var severity)) descriptor = ReplaceSeverity(descriptor, severity);

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable(), propertyName));
        }

        public static void ReportNotDisposedPropertyFactory(this CustomAnalysisContext ctx, string propertyName,
            bool isStatic)
        {
            if (isStatic)
            {
                ReportNotDisposedStaticPropertyFactory(ctx, propertyName);
            }
            else
            {
                ReportNotDisposedNoneStaticPropertyFactory(ctx, propertyName);
            }
        }

        private static void ReportNotDisposedStaticPropertyFactory(this CustomAnalysisContext ctx, string propertyName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, propertyName);
            var descriptor = NotDisposed.StaticFactoryProperty.Descriptor;

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor,location, properties.ToImmutable(), propertyName));
        }

        private static void ReportNotDisposedNoneStaticPropertyFactory(this CustomAnalysisContext ctx, string propertyName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var properties = ImmutableDictionary.CreateBuilder<string, string>();
            properties.Add(Constants.Variablename, propertyName);
            var descriptor = NotDisposed.FactoryProperty.Descriptor;

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties.ToImmutable(), propertyName));
        }

        public static void ReportHiddenDisposable(this CustomAnalysisContext ctx, string actualTypeName, string returnTypeName, string methodOrFuncName)
        {
            var location = ctx.OriginalNode.GetLocation();
            var descriptor = Hidden.Disposable;

            ctx.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, actualTypeName, returnTypeName, methodOrFuncName));
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