using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Utils
{
    internal class CustomAnalysisContext
    {
        public CustomAnalysisContext(SyntaxNodeAnalysisContext context, DisposableSource source, INamedTypeSymbol type)
        {
            Context = context;
            Source = source;
            Type = type;
        }

        public SyntaxNodeAnalysisContext Context { get; }
        public DisposableSource Source { get; }
        public INamedTypeSymbol Type { get; }
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
    }
}