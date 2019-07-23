using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Utils
{
    internal class CustomAnalysisContext
    {
        public CustomAnalysisContext(SyntaxNodeAnalysisContext context, DisposableSource source, INamedTypeSymbol type, IDetector detector)
        {
            Context = context;
            Source = source;
            Type = type;
            Detector = detector;
        }

        public SyntaxNodeAnalysisContext Context { get; }
        public DisposableSource Source { get; }
        public INamedTypeSymbol Type { get; }
        public IDetector Detector { get; }
        public SyntaxNode Node => Context.Node;
        public SemanticModel SemanticModel => Context.SemanticModel;
    }

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

        //public static bool IsTrackingMethodCall(this CustomAnalysisContext ctx)
        //{
        //    return ctx.Detector.IsTrackingMethodCall(ctx.Context.Node, ctx.Context.SemanticModel);
        //}

        public static void ReportNotDisposedAnonymousObject(this CustomAnalysisContext ctx)
        {
            ctx.Context.ReportNotDisposedAnonymousObject(ctx.Source);
        }

        public static void ReportNotDisposedLocalVariable(this CustomAnalysisContext ctx)
        {
            ctx.Context.ReportNotDisposedLocalVariable();
        }

        public static void ReportNotDisposedField(this CustomAnalysisContext ctx, string variableName)
        {
            ctx.Context.ReportNotDisposedField(variableName, ctx.Source);
        }
        public static void ReportNotDisposedProperty(this CustomAnalysisContext ctx, string proepertyName)
        {
            ctx.Context.ReportNotDisposedProperty(proepertyName, ctx.Source);
        }
    }
}