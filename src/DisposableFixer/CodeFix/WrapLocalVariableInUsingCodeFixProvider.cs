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
    public class WrapLocalVariableInUsingCodeFixProvider : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable)
                context.RegisterCodeFix(
                    CodeAction.Create("Wrap in using", c => WrapLocalVariableInUsing(context, c)),
                    context.Diagnostics);
            
            return Task.CompletedTask;
        }

        private static async Task<Document> WrapLocalVariableInUsing(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            var node = oldRoot.FindNode(context.Span) as ExpressionSyntax;

            if (node == null || !node.IsDescendantOfVariableDeclarator()) return context.Document;

            var variableDeclaration = node.Parent.Parent.Parent as VariableDeclarationSyntax;
            var localDeclarationStatement = variableDeclaration.Parent as LocalDeclarationStatementSyntax;

            if (variableDeclaration.TryFindContainigBlock(out var block))
            {
                var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
                var statements = block.Statements.Where(s => s != localDeclarationStatement);
                
                var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block(statements))
                    .WithDeclaration(variableDeclaration);

                var newBlock = SyntaxFactory.Block(@using);
                editor.ReplaceNode(block, newBlock);
                var wrapLocalVariableInUsing = editor.GetChangedDocument();
                return wrapLocalVariableInUsing;
            }

            return context.Document;
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create<string>(
            SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable);
    }
}