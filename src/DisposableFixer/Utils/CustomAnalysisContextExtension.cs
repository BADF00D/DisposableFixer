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

        public static CustomAnalysisContext NewWith(this CustomAnalysisContext ctx, SyntaxNode newNode)
        {
            return CustomAnalysisContext.WithOtherNode(ctx.Context, newNode, ctx.Source, ctx.Type, ctx.Detector);
        }
    }
}