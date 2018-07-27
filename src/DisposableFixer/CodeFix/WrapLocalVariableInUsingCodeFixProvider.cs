using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class WrapLocalVariableInUsingCodeFixProvider : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create<string>(
            SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable);
    }
}