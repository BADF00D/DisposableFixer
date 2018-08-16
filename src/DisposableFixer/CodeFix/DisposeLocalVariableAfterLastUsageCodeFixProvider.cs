using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class DisposeLocalVariableAfterLastUsageCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; }

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            throw new NotImplementedException();
        }
    }
}