using System.Collections.Immutable;
using System.Composition;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class IntroduceLocalVariableAndUseUsingDeclaration : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            throw new System.NotImplementedException();
        }

        private static async Task<Document> PrefixWithUsingDeclaration(CodeFixContext context, CancellationToken cancel)
        {
            return await Task.FromResult(context.Document);
        }

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Id.ForAnonymousObjectFromObjectCreation, Id.ForAnonymousObjectFromMethodInvocation);
        public override FixAllProvider GetFixAllProvider() => null;
    }
}