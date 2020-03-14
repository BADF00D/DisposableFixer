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
    public class IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            Id.ForAnonymousObjectFromMethodInvocation,
            Id.ForAnonymousObjectFromObjectCreation,
            Id.ForLocal.Variable
        );

        public override FixAllProvider GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic == null) return Task.CompletedTask;

            if (diagnostic.Id == Id.ForLocal.Variable)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(ActionTitle.CreateFieldAndDisposeInDisposeMethod,
                        cancel => ConvertToFieldDisposeInDisposeMethod(context, cancel)),
                    diagnostic
                );
            }else if (diagnostic.Id == Id.ForAnonymousObjectFromObjectCreation
                      || diagnostic.Id == Id.ForAnonymousObjectFromMethodInvocation)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(ActionTitle.CreateFieldAndDisposeInDisposeMethod,
                        cancel => IntroduceFieldAndDisposeInDisposeMethod(context, cancel)),
                    diagnostic
                );
            }

            return Task.CompletedTask;
        }

        private static async Task<Document> ConvertToFieldDisposeInDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancel);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var fieldName = RetrieveFieldName(context, node);
            var model = editor.SemanticModel;

            if (!node.TryFindParent<ClassDeclarationSyntax>(out var oldClass)) return editor.GetChangedDocument();
            editor.AddInterfaceIfNeeded(oldClass, IdentifierName(Constants.IDisposable));

            string oldName = null;

            if (node.Parent is AwaitExpressionSyntax awaitExpression)
            {
                var type = (model.GetAwaitExpressionInfo(awaitExpression).GetResultMethod?.ReturnType as INamedTypeSymbol)?.Name;
                editor.AddUninitializedFieldNamed(oldClass, fieldName, type);
                var assignment = ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(fieldName),
                        awaitExpression));
                editor.ReplaceNode(node.Parent.Parent.Parent.Parent.Parent, assignment);
                if (node.TryGetParentVariableDeclaratorInScope(out var vds))
                {
                    oldName = vds.Identifier.Text;
                }

            }else if (node.TryGetParentVariableDeclarator(out var vds) && node.Parent is ConditionalExpressionSyntax ces)
            {
                if (vds.Parent is VariableDeclarationSyntax _ 
                    && vds.Parent.Parent is LocalDeclarationStatementSyntax localDeclarationStatement
                    )
                {
                    var type = model.GetTypeInfo(node).Type?.Name ?? Constants.IDisposable;
                    editor.AddUninitializedFieldNamed(oldClass, fieldName, type);
                    var assignment = ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(fieldName),
                            ces
                        )
                    );
                    if (node.TryFindParent<StatementSyntax>(out var statementToReplace))
                    {
                        editor.ReplaceNode(statementToReplace, assignment);
                    }
                    else
                    {
                        throw new Exception($"Cannot find ExpressionStatement of node '{node}' to replace");
                    }

                    oldName = vds.Identifier.Text;
                }
            }
            else
            {
                var type = model.GetTypeInfo(node).Type?.Name ?? Constants.IDisposable;
                editor.AddUninitializedFieldNamed(oldClass, fieldName, type);
                var assignment = ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(fieldName),
                        node as ExpressionSyntax
                    )
                );
                if (node.TryFindParent<StatementSyntax>(out var statementToReplace))
                {
                    editor.ReplaceNode(statementToReplace, assignment);
                }
                else
                {
                    throw new Exception($"Cannot find ExpressionStatement of node '{node}' to replace");
                }
            }

            if (node.TryFindParentScope(out var scope))
            {
                var nodeWithOldName = scope.DescendantNodes<IdentifierNameSyntax>()
                    .Where(ins => ins.Identifier.Text == oldName);
                foreach (var identifierNameSyntax in nodeWithOldName)
                {
                    editor.ReplaceNode(identifierNameSyntax, IdentifierName(fieldName));
                }
            }
            var disposeMethods = oldClass.GetParameterlessMethodNamed(Constants.Dispose).ToArray();

            if (disposeMethods.Any())
                editor.AddDisposeCallToMemberInDisposeMethod(disposeMethods.First(), fieldName, false);
            else
                editor.AddDisposeMethodAndDisposeCallToMember(oldClass, fieldName, false);

            editor.AddImportIfNeeded(Constants.System);

            return editor.GetChangedDocument();
        }

        private static async Task<Document> IntroduceFieldAndDisposeInDisposeMethod(CodeFixContext context,
            CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancel);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var fieldName = RetrieveFieldName(context, node);
            var model = editor.SemanticModel;


            if (!node.TryFindParent<ClassDeclarationSyntax>(out var oldClass)) return editor.GetChangedDocument();

            editor.AddInterfaceIfNeeded(oldClass, IdentifierName(Constants.IDisposable));


            switch (node)
            {
                case ExpressionSyntax expression:
                    ReplaceExpression(model, node, editor, oldClass, fieldName, expression);
                    break;
                case ArgumentSyntax argument:
                    ReplaceArgument(model, argument, editor, oldClass, fieldName, node);
                    break;

                default:
                    throw new NotSupportedException($"Cannot wrap type '{node.GetType().FullName}'");
            }

            var disposeMethods = oldClass.GetParameterlessMethodNamed(Constants.Dispose)
                .ToArray();

            if (disposeMethods.Any())
                editor.AddDisposeCallToMemberInDisposeMethod(disposeMethods.First(), fieldName, false);
            else
                editor.AddDisposeMethodAndDisposeCallToMember(oldClass, fieldName, false);

            editor.AddImportIfNeeded(Constants.System);

            return editor.GetChangedDocument();
        }

        private static void ReplaceArgument(SemanticModel model, ArgumentSyntax argumentSyntax, DocumentEditor editor,
            ClassDeclarationSyntax oldClass, string fieldName, SyntaxNode node)
        {
            var typeInfo = model.GetTypeInfo(argumentSyntax.Expression);
            var type = typeInfo.Type?.Name ?? Constants.IDisposable;
            editor.AddUninitializedFieldNamed(oldClass, fieldName, type);
            if (!node.TryFindContainingBlock(out var block)) return;

            var assignmentExpression = ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(fieldName),
                    argumentSyntax.Expression
                )
            );
            var precedingStatements =
                block.Statements.TakeWhile(ss =>
                    ss.DescendantNodes<ArgumentSyntax>().All(@as => @as != argumentSyntax));

            var variable = Argument(IdentifierName(fieldName));
            var currentStatement = block.Statements
                .SkipWhile(ss => ss.DescendantNodes<ArgumentSyntax>().All(@as => @as != argumentSyntax))
                .FirstOrDefault()
                .ReplaceNode(node, variable);
            var trailingStatements = block.Statements
                .SkipWhile(ss => ss.DescendantNodes<ArgumentSyntax>().All(@as => @as != argumentSyntax))
                .Skip(1);
            var newBlock = Block(precedingStatements
                .Concat(assignmentExpression)
                .Concat(currentStatement)
                .Concat(trailingStatements));

            editor.ReplaceNode(block, newBlock);
        }

        private static void ReplaceExpression(SemanticModel model, SyntaxNode node, DocumentEditor editor,
            ClassDeclarationSyntax oldClass, string fieldName, ExpressionSyntax expressionSyntax)
        {
            var typeInfo = model.GetTypeInfo(node);
            var type = typeInfo.Type?.Name ?? Constants.IDisposable;
            editor.AddUninitializedFieldNamed(oldClass, fieldName, type);


            if (node.Parent is MemberAccessExpressionSyntax)
            {
                var x = ParenthesizedExpression(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(fieldName),
                        expressionSyntax)
                    );

                editor.ReplaceNode(node, x);
            }
            else
            {
                var assignment = ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(fieldName),
                        expressionSyntax
                    )
                );
                editor.ReplaceNode(node.Parent, assignment);
            }
        }

        private static string RetrieveFieldName(CodeFixContext context, SyntaxNode node)
        {
            const string defaultName = "_disposable";
            if (!IsUndisposedLocalVariable(context)) return defaultName;

            return node.TryGetParentVariableDeclaratorInScope(out var vds) 
                ? ToFieldName(vds.Identifier.Text)
                : defaultName;
        }

        private static string ToFieldName(string typeName)
        {
            return "_" + typeName.Substring(0, 1).ToLower() + typeName.Substring(1);
        }

        private static bool IsUndisposedLocalVariable(CodeFixContext context)
        {
            return context.Diagnostics.First().Id ==
                   Id.ForLocal.Variable;
        }
    }
}