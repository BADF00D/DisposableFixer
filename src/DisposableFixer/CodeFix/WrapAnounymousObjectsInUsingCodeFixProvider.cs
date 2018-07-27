using System;
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
using Microsoft.CodeAnalysis.Formatting;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider))]
    [Shared]
    public class WrapAnounymousObjectsInUsingCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation,
                SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation
            );

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                context.RegisterCodeFix(
                    CodeAction.Create("Wrap in using", c => WrapExpressionSyntaxInUsing(context, c)),
                    context.Diagnostics);
            if (id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
                context.RegisterCodeFix(
                    CodeAction.Create("Wrap in using", c => WrapExpressionSyntaxInUsing(context, c)),
                    context.Diagnostics
                );
            return Task.FromResult(1);
        }

        private async Task<Document> WrapExpressionSyntaxInUsing(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            var node = oldRoot.FindNode(context.Span);

            if (node.Parent?.Parent is BlockSyntax)
            {
                var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
                var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block())
                    .WithExpression(node as ExpressionSyntax);
                editor.ReplaceNode(node.Parent, @using);

                return editor.GetChangedDocument();
            } if (node is ArgumentSyntax)
            {
                if (node.TryFindContainigBlock(out var parentBlock))
                {
                    if(node.TryFindParent<UsingStatementSyntax>(parentBlock, out var @parentUsing))
                    {
                        var preceedingStatements = parentBlock.Statements
                            .TakeWhile(s => s != parentUsing);
                        var trailingStatements = parentBlock.Statements
                            .SkipWhile(s => s != @parentUsing)
                            .Skip(1);
                        
                        var nodeToReplace = node.DescendantNodes()
                            .FirstOrDefault(dn => dn is ObjectCreationExpressionSyntax || dn is InvocationExpressionSyntax);

                        var variableDeclaration  = SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName("var"))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier("variable"))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.IdentifierName("MemoryStream"))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList())))));
                        var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
                        var newParentUsing = parentUsing.ReplaceNode(nodeToReplace, SyntaxFactory.IdentifierName("variable"));
                        var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block(newParentUsing))
                            .WithDeclaration(variableDeclaration);
                        
                        var newParentBlock =
                            SyntaxFactory.Block(preceedingStatements.Concat(@using).Concat(trailingStatements));
                        editor.ReplaceNode(parentBlock, newParentBlock.WithoutAnnotations(Formatter.Annotation));

                        return editor.GetChangedDocument();
                    }
                    
                }
            }
            throw new NotSupportedException();
        }
    }
}