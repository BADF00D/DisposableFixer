using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using DisposableFixer.Misc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class DisposeLocalVariableAfterLastUsageCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(Id.ForNotDisposedLocalVariable);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(CodeAction.Create(ActionTitle.DisposeAfterLastUsage, cancel => Apply(context, cancel)),
                context.Diagnostics);

            return Task.CompletedTask;
        }

        private static async Task<Document> Apply(CodeFixContext context, CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancel);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var variableName = FindVariableName(node);
            if (node.TryFindContainingBlock(out var parentBlock))
            {
                var lastUsage = parentBlock
                    .DescendantNodes()
                    .Last(sn =>
                    {
                        if (sn is ObjectCreationExpressionSyntax oces)
                        {
                            return oces.ArgumentList.Arguments.Any(arg =>
                                arg.Expression is SimpleNameSyntax sns && sns.Identifier.Text == variableName);
                        }

                        if (sn is InvocationExpressionSyntax ies)
                        {
                            if(ies.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax ins && ins.Identifier.Text == variableName)
                                return true;
                            if(ies.ArgumentList.Arguments.Any(arg => arg.Expression is SimpleNameSyntax sns && sns.Identifier.Text == variableName))
                                return true;
                        }

                        if (sn is VariableDeclaratorSyntax vds && vds.Identifier.Text == variableName) return true;

                        return false;
                    });


                if(lastUsage.TryFindParent<StatementSyntax>(parentBlock, out var lastUsageStatement))
                {
                    var disposeCall = SyntaxCreator.CreateDisposeCallFor(variableName);

                    var statementsBeforeLastUsage = parentBlock.Statements
                        .TakeWhile(s => s != lastUsageStatement);
                    var statementsAfterLastUsage = parentBlock.Statements
                        .SkipWhile(s => s != lastUsageStatement)
                        .Skip(1);

                    var newBlock = SyntaxFactory.Block(
                        statementsBeforeLastUsage
                            .Concat(lastUsageStatement)
                            .Concat(disposeCall)
                            .Concat(statementsAfterLastUsage)
                    ).WithoutAnnotations(Formatter.Annotation);
                    editor.ReplaceNode(parentBlock, newBlock);

                    return editor.GetChangedDocument();
                }
            }

            return context.Document;
        }

        private static string FindVariableName(SyntaxNode node)
        {
            if (node?.Parent is AwaitExpressionSyntax awaitExpression)
            {
                if (awaitExpression.Parent is AssignmentExpressionSyntax assignmentExpressionInAwait)
                {
                    return (assignmentExpressionInAwait.Left as IdentifierNameSyntax)?.Identifier.Text;
                }
                var variableDeclaration = node?.Parent?.Parent.Parent as VariableDeclaratorSyntax;
                return variableDeclaration?.Identifier.Text;
            }
            else
            {
                if (node?.Parent is AssignmentExpressionSyntax assignmentExpression)
                {
                    return (assignmentExpression.Left as IdentifierNameSyntax)?.Identifier.Text;
                }
                var variableDeclaration = node?.Parent?.Parent as VariableDeclaratorSyntax;
                return variableDeclaration?.Identifier.Text;
            }
            
        }
    }
}