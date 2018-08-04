using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.CodeFix.Extensions;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DisposableFixer.CodeFix
{
    public abstract class UndisposedMemberCodeFixProvider : CodeFixProvider
    {
        protected static async Task<Document> CreateDisposeCallInParameterlessDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            var node = oldRoot.FindNode(context.Span);
            var variableName = context.Diagnostics.First().Properties[Constants.Variablename];

            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);

            if (node.TryFindParent<ClassDeclarationSyntax>(out var oldClass))
            {
                var model = await context.Document.GetSemanticModelAsync(cancel);
                var containingSymbol = model.GetEnclosingSymbol(context.Span.Start, cancel).ContainingSymbol;
                var @classtype =
                    containingSymbol as INamedTypeSymbol;
                if (@classtype == null) return context.Document;


                editor.AddInterfaceIfNeeded(oldClass, SyntaxFactory.IdentifierName(Constants.IDisposable));

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
    }
}