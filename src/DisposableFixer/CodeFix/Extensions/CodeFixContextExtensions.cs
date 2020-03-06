using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace DisposableFixer.CodeFix.Extensions
{
    public static class CodeFixContextExtensions
    {
        public static bool IsLanguageVersionAtLeast(this CodeFixContext context, LanguageVersion version)
        {
            return context.Document.Project.ParseOptions is CSharpParseOptions options
                   && options.LanguageVersion >= version;
        }
    }
}