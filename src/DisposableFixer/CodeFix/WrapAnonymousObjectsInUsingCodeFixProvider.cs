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
    public class WrapAnonymousObjectsInUsingCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                Id.ForAnonymousObjectFromObjectCreation,
                Id.ForAnonymousObjectFromMethodInvocation
            );

        public override FixAllProvider GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == Id.ForAnonymousObjectFromObjectCreation)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ActionTitle.WrapInUsing, 
                        c => WrapExpressionSyntaxInUsing(context, c),
                        Guid.NewGuid().ToString()),
                    context.Diagnostics);
            if (id == Id.ForAnonymousObjectFromMethodInvocation)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ActionTitle.WrapInUsing, 
                        c => WrapExpressionSyntaxInUsing(context, c),
                        Guid.NewGuid().ToString()),
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
                if (node.TryFindContainingBlock(out var parentBlock))
                {
                    if(node.TryFindParent<StatementSyntax>(parentBlock, out var @parentUsing))
                    {
                        var preceedingStatements = parentBlock.Statements
                            .TakeWhile(s => s != parentUsing);
                        var trailingStatements = parentBlock.Statements
                            .SkipWhile(s => s != @parentUsing)
                            .Skip(1);
                        
                        var nodeToReplace = node.DescendantNodes<ExpressionSyntax>()
                            .FirstOrDefault(dn => dn is ObjectCreationExpressionSyntax || dn is InvocationExpressionSyntax);
                        ITypeSymbol t = nodeToReplace.GetTypeSymbol(await context.Document.GetSemanticModelAsync(context.CancellationToken));
                        var typeName = t.MetadataName;
                        var variableName = t.GetVariableName();
                        var argumentList = nodeToReplace.DescendantNodes<ArgumentListSyntax>().FirstOrDefault();

                        var variableDeclaration  = SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(Constants.Var))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier(variableName))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.IdentifierName(typeName))
                                                    .WithArgumentList(argumentList)))));
                        var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
                        var newParentUsing = parentUsing.ReplaceNode(nodeToReplace, SyntaxFactory.IdentifierName(variableName));
                        var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block(newParentUsing.Concat(trailingStatements)))
                            .WithDeclaration(variableDeclaration);
                        
                        var newParentBlock =
                            SyntaxFactory.Block(preceedingStatements.Concat(@using));
                        editor.ReplaceNode(parentBlock, newParentBlock.WithoutAnnotations(Formatter.Annotation));

                        return editor.GetChangedDocument();
                    }
                    
                }
            }else if (node is InvocationExpressionSyntax ies)
            {
                if (node.TryFindContainingBlock(out var parentBlock))
                {
                    if (node.TryFindParent<StatementSyntax>(parentBlock, out var parentUsing))
                    {
                        var preceedingStatements = parentBlock.Statements
                            .TakeWhile(s => s != parentUsing);
                        var trailingStatements = parentBlock.Statements
                            .SkipWhile(s => s != @parentUsing)
                            .Skip(1);

                        var nodeToReplace = ies;
                        var returnType = ies.GetReturnType(await context.Document.GetSemanticModelAsync(context.CancellationToken));
                        var typeName = returnType.MetadataName;
                        var variableName = returnType.GetVariableName();
                        var argumentList = nodeToReplace.DescendantNodes<ArgumentListSyntax>().FirstOrDefault();

                        var variableDeclaration = SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(Constants.Var))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier(variableName))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.IdentifierName(typeName))
                                                    .WithArgumentList(argumentList)))));
                        var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
                        var newParentUsing = parentUsing.ReplaceNode(nodeToReplace, SyntaxFactory.IdentifierName(variableName));
                        var @using = SyntaxFactory.UsingStatement(SyntaxFactory.Block(newParentUsing.Concat(trailingStatements)))
                            .WithDeclaration(variableDeclaration);

                        var newParentBlock =
                            SyntaxFactory.Block(preceedingStatements.Concat(@using));
                        editor.ReplaceNode(parentBlock, newParentBlock.WithoutAnnotations(Formatter.Annotation));

                        return editor.GetChangedDocument();
                    }

                }
            }

            return context.Document;
        }
    }
}