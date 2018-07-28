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

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnotic = context.Diagnostics.FirstOrDefault();
            if(diagnotic == null) return Task.CompletedTask;

            if (diagnotic.Id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation ||
                diagnotic.Id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
            {
                context.RegisterCodeFix(
                    CodeAction.Create("Create field and dispose in Dispose() method.", cancel => IntroduceFieldAndDisposeInDisposeMethod(context, cancel)),
                    diagnotic
                );
            }

            return Task.CompletedTask;
        }

        private static async Task<Document> IntroduceFieldAndDisposeInDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
            var node = editor.OriginalRoot.FindNode(context.Span);
            var variableName = "_disposable";
            if (node.TryFindParent<ClassDeclarationSyntax>(out var oldClass))
            {
                var model = await context.Document.GetSemanticModelAsync(cancel);
                var containingSymbol = model.GetEnclosingSymbol(context.Span.Start, cancel).ContainingSymbol;
                var @classtype =
                    containingSymbol as INamedTypeSymbol;
                if (@classtype == null) return context.Document;


                editor.AddBaseTypeIfNeeded(oldClass, SyntaxFactory.IdentifierName(Constants.IDisposable));
                editor.AddUninitializedFieldNamed(oldClass, variableName);

                var assignment = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(variableName),
                        node as ExpressionSyntax
                    )
                );
                editor.ReplaceNode(node.Parent,assignment);
                
                var disposeMethods = oldClass.GetParameterlessMethodNamed(Constants.Dispose);

                if (disposeMethods.Any())
                {
                    editor.AddDisposeCallToMemberInDisposeMethod(disposeMethods.First(), variableName);
                }
                else
                {
                    editor.AddDisposeMethodAndDisposeCallToMember(oldClass, variableName);
                }

                editor.AddImportIfNeeded(Constants.System);
            }

            return editor.GetChangedDocument();
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation,
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation,
            SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable
        );
    }
}
