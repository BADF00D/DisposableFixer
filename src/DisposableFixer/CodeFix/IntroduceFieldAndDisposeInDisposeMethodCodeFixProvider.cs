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

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnotic = context.Diagnostics.FirstOrDefault();
            if(diagnotic == null) return Task.CompletedTask;

            context.RegisterCodeFix(
                IsUndisposedLocalVariable(context)
                    ? CodeAction.Create("Create field and dispose in Dispose() method.",
                        cancel => DisposeInDisposeMethod(context, cancel))
                    : CodeAction.Create("Create field and dispose in Dispose() method.",
                        cancel => IntroduceFieldAndDisposeInDisposeMethod(context, cancel)),
                diagnotic
            );

            return Task.CompletedTask;
        }

        private static async Task<Document> DisposeInDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancel);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var fieldName = RetrieveFieldName(context, node);

            if (!node.TryFindParent<ClassDeclarationSyntax>(out var oldClass)) return editor.GetChangedDocument();

            editor.AddBaseTypeIfNeeded(oldClass, SyntaxFactory.IdentifierName(Constants.IDisposable));
            editor.AddUninitializedFieldNamed(oldClass, fieldName);

            var assignment = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(fieldName),
                    node as ExpressionSyntax
                )
            );
            editor.ReplaceNode(node.Parent.Parent, assignment);

            var disposeMethods = oldClass.GetParameterlessMethodNamed(Constants.Dispose).ToArray();

            if (disposeMethods.Any())
            {
                editor.AddDisposeCallToMemberInDisposeMethod(disposeMethods.First(), fieldName);
            }
            else
            {
                editor.AddDisposeMethodAndDisposeCallToMember(oldClass, fieldName);
            }

            editor.AddImportIfNeeded(Constants.System);

            return editor.GetChangedDocument();
        }

        private static async Task<Document> IntroduceFieldAndDisposeInDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancel);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var fieldName = RetrieveFieldName(context, node);

            if (!node.TryFindParent<ClassDeclarationSyntax>(out var oldClass)) return editor.GetChangedDocument();

            editor.AddBaseTypeIfNeeded(oldClass, SyntaxFactory.IdentifierName(Constants.IDisposable));
            editor.AddUninitializedFieldNamed(oldClass, fieldName);

            var assignment = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(fieldName),
                    node as ExpressionSyntax
                )
            );
            editor.ReplaceNode(node.Parent,assignment);
                
            var disposeMethods = oldClass.GetParameterlessMethodNamed(Constants.Dispose)
                .ToArray();

            if (disposeMethods.Any())
            {
                editor.AddDisposeCallToMemberInDisposeMethod(disposeMethods.First(), fieldName);
            }
            else
            {
                editor.AddDisposeMethodAndDisposeCallToMember(oldClass, fieldName);
            }

            editor.AddImportIfNeeded(Constants.System);

            return editor.GetChangedDocument();
        }

        private static string RetrieveFieldName(CodeFixContext context, SyntaxNode node)
        {
            const string defaultName = "_disposable";
            if (!IsUndisposedLocalVariable(context)) return defaultName;

            var variableDeclarator = node.Parent?.Parent as VariableDeclaratorSyntax;
            return variableDeclarator?.Identifier.Text ?? defaultName;
        }

        private static bool IsUndisposedLocalVariable(CodeFixContext context)
        {
            return context.Diagnostics.First().Id ==
                   SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable;
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation,
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation,
            SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable
        );
    }
}
