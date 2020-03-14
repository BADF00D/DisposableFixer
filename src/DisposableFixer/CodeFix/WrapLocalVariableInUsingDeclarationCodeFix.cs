using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.CodeFix.Extensions;
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
            if (!context.IsLanguageVersionAtLeast(LanguageVersion.CSharp8)) return Task.CompletedTask;

            var diagnostics = context.Diagnostics.Where(d => d.Id == Id.ForLocal.Variable);

            context.RegisterCodeFix(CodeAction.Create(ActionTitle.UseUsingDeclaration, c => PrefixWithUsingDeclaration(context, c), Guid.Empty.ToString()), diagnostics);

            return Task.CompletedTask;
        }

        private static async Task<Document> PrefixWithUsingDeclaration(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            if (!(oldRoot.FindNode(context.Span) is ExpressionSyntax node)) return context.Document;
            if (!node.TryFindParent<LocalDeclarationStatementSyntax>(out var localDeclaration)) return context.Document;

            var newLocalDeclaration = localDeclaration.WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
            editor.ReplaceNode(localDeclaration, newLocalDeclaration);

            return editor.GetChangedDocument();
        }

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Id.ForLocal.Variable);
        public override FixAllProvider GetFixAllProvider() => null;
    }
}