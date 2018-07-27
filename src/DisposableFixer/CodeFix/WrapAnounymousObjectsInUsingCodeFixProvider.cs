﻿using System;
using System.Collections.Immutable;
using System.Composition;
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

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class WrapAnounymousObjectsInUsingCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation,
                SyntaxNodeAnalysisContextExtension.IdForAnonymousMethodInvocation
            );

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                context.RegisterCodeFix(
                    CodeAction.Create("Wrap in using", c => WrapExpressionSyntaxInUsing(context, c)),
                    context.Diagnostics);
            if (id == SyntaxNodeAnalysisContextExtension.IdForAnonymousMethodInvocation)
                context.RegisterCodeFix(
                    CodeAction.Create("Wrap in using", c => WrapExpressionSyntaxInUsing(context, c)),
                    context.Diagnostics
                );
            return Task.FromResult(1);
        }

        private async Task<Document> WrapExpressionSyntaxInUsing(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            var node = oldRoot.FindNode(context.Span) as ExpressionSyntax;

            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
            var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block())
                .WithExpression(node);
            editor.ReplaceNode(node.Parent, @using);

            return editor.GetChangedDocument();
        }
    }
}