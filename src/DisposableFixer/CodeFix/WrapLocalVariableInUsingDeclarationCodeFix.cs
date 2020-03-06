using System;
using System.Collections.Immutable;
using System.Composition;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class WrapLocalVariableInUsingDeclarationCodeFix : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostics = context.Diagnostics.Where(d => d.Id == Id.ForNotDisposedLocalVariable);

            context.RegisterCodeFix(CodeAction.Create(ActionTitle.UseUsingDeclaration, c => PrefixWithUsingDeclaration(context, c), Guid.Empty.ToString()), diagnostics);

            return Task.CompletedTask;
        }

        private static async Task<Document> PrefixWithUsingDeclaration(CodeFixContext context, CancellationToken cancel)
        {
            return await Task.FromResult(context.Document);
        }

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Id.ForNotDisposedLocalVariable);
        public override FixAllProvider GetFixAllProvider() => null;
    }
}