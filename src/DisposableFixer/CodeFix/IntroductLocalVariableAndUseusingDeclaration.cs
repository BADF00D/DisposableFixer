using System;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class IntroduceLocalVariableAndUseUsingDeclaration : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostics = context.Diagnostics.Where(d => d.Id == Id.ForAnonymousObjectFromObjectCreation || d.Id == Id.ForAnonymousObjectFromMethodInvocation);

            context.RegisterCodeFix(CodeAction.Create(ActionTitle.DeclareLocalVariableAndUseUsingDeclaration, c => PrefixWithUsingDeclaration(context, c), Guid.Empty.ToString()), diagnostics);

            return Task.CompletedTask;
        }

        private static async Task<Document> PrefixWithUsingDeclaration(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            if (!(oldRoot.FindNode(context.Span) is ExpressionSyntax node)) return context.Document;
            if (!node.TryFindParent<ExpressionStatementSyntax>(out var oldStatement)) return context.Document;

            var semanticModel = await context.Document.GetSemanticModelAsync(cancel);
            var existingNames = semanticModel.GetExistingNamesInMethod(context.Span.Start);

            var returnType = semanticModel.GetReturnTypeOrDefaultOf(oldStatement);
            var newVariableName = NameGenerator.ProposeName(existingNames, returnType);

            var newStatement = oldStatement.WrapInLocalDeclarationStatement(newVariableName)
                .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));

            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
            editor.ReplaceNode(oldStatement, newStatement);

            return editor.GetChangedDocument();
        }

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Id.ForAnonymousObjectFromObjectCreation, Id.ForAnonymousObjectFromMethodInvocation);
        public override FixAllProvider GetFixAllProvider() => null;
    }
}